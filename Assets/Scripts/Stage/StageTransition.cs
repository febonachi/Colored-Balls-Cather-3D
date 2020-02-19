using Utils;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class StageTransition : MonoBehaviour {
    #region editor
    [SerializeField] private Toy toy = default;
    [SerializeField] private CameraTarget cameraTarget = default;

    [Space][Header("VFX")]
    [SerializeField] private GameObject stageCompletedPS = default;
    #endregion

    private int stageIndex = 0;

    private Stage[] stages;
    private Stage currentStage;
    private UIScore uiScore;
    private UIProgressBar uiProgressBar;    
    private ColorScheme colorScheme;
    private GameController gameController;

    #region private
    private void Awake() {
        stages = GetComponentsInChildren<Stage>();

        gameController = GameController.instance;
        uiScore = gameController.uiManager.gameMenu.score;
        uiProgressBar = gameController.uiManager.gameMenu.progressBar;
        uiProgressBar.initialize(gameController.gameSettings.savedLevel);

        currentStage = stages[stageIndex];
    }

    private void Start() {
        colorScheme = ColorScheme.instance;

        addStageDelegates(0); // prevent initialization errors
        
        cameraTarget.initializeWaypoints(stages.Select(s => s.platform.transform.position).ToArray());

        // dbg
        //for(int i = 0; i < stages.Length; i++) print($"stage: {i + 1}, guns: {stages[i].gunsCount}, targetScore: {stages[i].goalScore}");
    }

    private void onStageCompleted(){
        if (currentStage) removeStageDelegates();

        int nextStageIndex = stageIndex + 1;
        if (nextStageIndex < stages.Length) StartCoroutine(_onStageCompletedTransition(nextStageIndex)); // activate next stage
        else StartCoroutine(_onAllStagesCompletedTransition()); // show End Level UI -> activate next level
    }

    private void onStageFailed() => StartCoroutine(_onStageFailedTransition());

    private void onStageScoreUpdated(float scoreInPercent, int value, int ballsInARow, bool fever) {
        uiProgressBar.setProgress(scoreInPercent);
        uiScore.addScore(value, ballsInARow, fever);
    }

    private void addStageDelegates(int nextStageIndex){
        currentStage.failed += onStageFailed;
        currentStage.completed += onStageCompleted;
        currentStage.scoreUpdated += onStageScoreUpdated;
        currentStage.prepare(toy); // show stage guns

        uiProgressBar.nextStage(nextStageIndex);
    }

    private void removeStageDelegates(){
        currentStage.failed -= onStageFailed;
        currentStage.completed -= onStageCompleted; 
        currentStage.scoreUpdated -= onStageScoreUpdated;
    }

    private IEnumerator _onAllStagesCompletedTransition(){
        toy.gameObject.SetActive(false);

        if (stageCompletedPS != null) Destroy(Instantiate(stageCompletedPS, currentStage.platform.transform.position, stageCompletedPS.transform.rotation), 5f);

        currentStage.activateGuns(false); // hide guns

        yield return StartCoroutine(toy._reset()); // toy scale down && remove toyBalls from holder

        gameController.uiManager.showEndLevelMenu();

        yield return null;
    }

    private IEnumerator _onStageCompletedTransition(int nextStageIndex){
        //print("---Next Stage Transition Started---");

        if(stageCompletedPS != null) Destroy(Instantiate(stageCompletedPS, currentStage.platform.transform.position, stageCompletedPS.transform.rotation), 5f);

        currentStage.activateGuns(false); // hide guns

        yield return StartCoroutine(toy._reset()); // toy scale down && remove toyBalls from holder
        
        DOTween.To(() => RenderSettings.fogColor, color => RenderSettings.fogColor = color, colorScheme.randomFogColor, 15f); // change fog color

        Vector3 closestPointOnPath = cameraTarget.closestPoint(toy.transform.position);
        yield return StartCoroutine(toy._moveTowards(closestPointOnPath, 10f)); // small speed amount (mobile bugs)

        currentStage.bridge.openBridge();

        yield return StartCoroutine(currentStage.platform._openPlatformDoors());
        StartCoroutine(stages[nextStageIndex].platform._openPlatformDoors());

        // gameController.cameraManager.zoomIn(.5f);

        Vector3 nextWaypointPosition = cameraTarget.nextWaypoint();

        string coroutineGroupKey = "nextStageTransition";
        cameraTarget._moveToNextWaypoint().parallel(this, coroutineGroupKey);
        toy._moveTowards(nextWaypointPosition, cameraTarget.speed).parallel(this, coroutineGroupKey);

        yield return new WaitWhile(() => CoroutineExtension.inProcess(coroutineGroupKey)); // moving camera && toy

        // gameController.cameraManager.zoomOut(1f);

        currentStage.bridge.closeBridge();

        stageIndex = nextStageIndex; // update stage index
        currentStage = stages[stageIndex];

        yield return StartCoroutine(currentStage.platform._closePlatformDoors());

        // prepare next stage
        addStageDelegates(nextStageIndex); // preparing stage guns
        currentStage.activateGuns(true); // activate stage guns (shoot enabled)

        //print("---Next Stage Transition Done---");
    }

    private IEnumerator _onStageFailedTransition(){
        //print("---Fail Stage Transition Started---");

        // calculate total progress in %
        int maxStagesScore = stages.Sum(stage => stage.goalScore);
        float currentStagesScore = uiScore.score;
        gameController.gameSettings.saveProgress((int)currentStagesScore.map(0f, maxStagesScore, 0f, 99f)); // 99 because this is for restart menu

        if (stageCompletedPS != null) Instantiate(stageCompletedPS, currentStage.platform.transform.position, stageCompletedPS.transform.rotation);

        currentStage.activateGuns(false); // hide guns

        yield return new WaitForSeconds(.5f);

        gameController.uiManager.showRestartLevelMenu();
        
        //print("---Fail Stage Transition Done---");
    }
    #endregion

    #region public
    public void activateStages() {
        DOTween.To(() => RenderSettings.fogColor, color => RenderSettings.fogColor = color, colorScheme.randomFogColor, 5f); // change fog color

        currentStage.activateGuns(true); // gameController activation
    }
    #endregion
}
