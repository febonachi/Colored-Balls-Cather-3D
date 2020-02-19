using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public static GameController instance;

    #region editor
    [SerializeField] private int levelToLoad = 0;

    [Header("Debug Settings (Set to <False> in release version)")]
    [SerializeField] private bool debugMode = false;
    [SerializeField] private bool firstLaunch = false;

    [Space]

    [SerializeField] private Transform managersHolder = default;
    #endregion

    #region public properties
    public GameSettings gameSettings => settings;
    public UIManager uiManager => this["ui"] as UIManager;
    public CameraManager cameraManager => this["camera"] as CameraManager;
    public Manager this[string id] => managers.FirstOrDefault(m => m.id == id);
    #endregion

    private bool allManagersLoaded = false;

    private GameSettings settings;
    private Manager[] managers;

    #region private
    private async void Awake() {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        settings = new GameSettings(debugMode, firstLaunch);

        await loadManagers();

        print("All managers loaded");

        SceneManager.sceneLoaded += onSceneLoaded;
        SceneManager.LoadScene(levelToLoad != 0 ? levelToLoad : settings.randomLevel());
    }

    private void Update() {
        if (gameSettings.gameStarted) return;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) startLocalGame();
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) startLocalGame();
#endif
    }

    private async Task loadManagers() {
        managers = managersHolder.GetComponentsInChildren<Manager>();
        foreach (Manager manager in managers) manager.initialize();

        bool managersLoaded = false;
        while (!managersLoaded) {
            managersLoaded = managers.All(m => m.status == Manager.Status.Ready);

            await Task.Delay(25);
        }

        allManagersLoaded = true;
    }

    private void onSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "main") return;

        //print($"Level loaded: {scene.name}");

        settings.saveLevel(scene.buildIndex);

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 235;
        RenderSettings.fogEndDistance = 400;
        RenderSettings.fogColor = ColorScheme.instance.randomFogColor;

        uiManager.resetUIElements();
        uiManager.showMainMenu();
    }

    private void startLocalGame() {
        //print("Start Game (Activate First Stage)");
        settings.startLocalGame();

        uiManager.showGameMenu();

        FindObjectOfType<StageTransition>().activateStages();
    }
    #endregion

    #region public
    public void vibrate() {
        if (settings.vibration) Handheld.Vibrate();
    }

    public void restartLevel() {
        SceneManager.LoadScene(settings.savedLevel);
    }

    public void nextLevel() {
        settings.levelPassed();

        int nextSceneIndex = settings.randomLevel();
        SceneManager.LoadScene(nextSceneIndex < SceneManager.sceneCountInBuildSettings ? nextSceneIndex : 1);
    }
    #endregion
}
