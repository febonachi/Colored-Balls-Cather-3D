using Utils;
using System;
using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {
    #region editor
    [Header("Gun Settings")]
    [SerializeField] private bool paused = false;
    [Tooltip("Startup shoot delay")]
    [SerializeField] private float startShootDelay = 0f;
    [SerializeField] private RandomizedFloat activationDelay = 0f;

    [Header("Shoot Settings")]
    [SerializeField] private GunCannon.CannonSettings shootSettings = default;
    
    [Header("Cannon Settings")]
    [SerializeField] private GunWalls walls = default;

    [Header("Goal Settings")]
    [SerializeField] private GunGoal goal = default;
    [SerializeField] private float goalMoveSpeed = 10f;
    [SerializeField] private float goalRotationSpeed = 5f;

    [Header("Path Settings")]
    [SerializeField] private PathController pathController = default;

    [Header("Other Settings")]
    [SerializeField] private GunPlatform platform = default;
    #endregion

    #region public properties
    #endregion

    private bool idling = true;

    #region private
    private void Awake() {
        goal.gameObject.SetActive(false);

        pathController.initialize(walls.cannon.spawnPoint, goal.transform);
        pathController.hidePath();

        walls.cannon.cannonSettings = shootSettings;
    }

    private void Update() => walls.cannon.lookAt(pathController.direction.firstAnchor);

    private IEnumerator _moveGoalToNextPoint(Vector3 nextPosition){
        float goalRotSpeed = goalRotationSpeed * (goal.transform.position.z < nextPosition.z ? 1f : -1f);
        while(goal.transform.position != nextPosition){
            goal.transform.Rotate(0f, goalRotSpeed, 0f);
            goal.transform.position = Vector3.MoveTowards(goal.transform.position, nextPosition, goalMoveSpeed * Time.deltaTime);
            pathController.resetPath();
            yield return null;
        }

        if(!idling) {
            yield return new WaitWhile(() => paused == true);
            yield return new WaitForSeconds(startShootDelay);
        }

        Color nextColor = idling ? walls.cannon.nextRandomColor : walls.cannon.nextColor;

        platform.setColor(nextColor);
        goal.setColor(nextColor, cachedAlpha: false); // set nextColor immediately
        yield return StartCoroutine(pathController.direction._highlightPointsOnce(nextColor));

        goal.shakeScale();

        if(!idling) startShootDelay = walls.cannon.shoot(pathController); // update next shoot delay (randomly)

        goal.setColor(nextColor);

        // calculate next goal position
        Vector3 randomBoundPosition = goal.boundary.randomPosition();
        randomBoundPosition.y = goal.transform.position.y;

        float coroutineDelay = idling ? .2f : UnityEngine.Random.Range(1f, 1.25f);
        yield return new WaitForSeconds(coroutineDelay);

        // restart coroutine
        StartCoroutine(_moveGoalToNextPoint(randomBoundPosition));
    }

    private IEnumerator _preparePathAndGunCannon(){
        float activationDelayValue = activationDelay.value;
        if (activationDelayValue != 0f) {
            yield return new WaitUntil(() => idling == false);
            yield return new WaitForSeconds(activationDelayValue);
        }

        yield return StartCoroutine(walls._moveUpFromWater());
        
        yield return new WaitForSeconds(.2f);

        Color startUpColor = walls.cannon.nextRandomColor;
        platform.setColor(startUpColor);
        goal.setColor(startUpColor, cachedAlpha: false); // set color immediately
        pathController.direction.highlightPointsImmediately(startUpColor); // set color immediately

        goal.gameObject.SetActive(true);
        Vector3 randomBoundPosition = goal.boundary.randomPosition();
        randomBoundPosition.y = goal.transform.position.y;

        StartCoroutine(_moveGoalToNextPoint(randomBoundPosition));
    }
    #endregion

    #region public
    public void start() => paused = false;

    public void stop() => paused = true;

    public void prepareCannonSettings(GunCannon.CannonSettings cannonSettings, RandomizedFloat delay){
        activationDelay = delay;
        shootSettings = cannonSettings;
        walls.cannon.cannonSettings = shootSettings;
    }

    public void prepare() => StartCoroutine(_preparePathAndGunCannon());

    public void activate(bool state) {
        if (!state) { // hide guns, path and stop all coroutines
            StopAllCoroutines();

            pathController.hidePath();
            goal.setColor(Utility.transparentColor, cachedAlpha: false);

            walls.moveDownToWater();
        } else idling = false; // move goal && shoot (activated state)
    }
    #endregion
}
