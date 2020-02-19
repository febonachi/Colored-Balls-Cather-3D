using Utils;
using System;
using UnityEngine;
using PathCreation;
using System.Collections;

[RequireComponent(typeof(Placer))]
public class Follower : MonoBehaviour {
    public enum FollowerRotation {None, Path, Transform};

    #region editor
    [SerializeField] private EndOfPathInstruction endInstruction = EndOfPathInstruction.Loop;
    [SerializeField] private RandomizedFloat forwardSpeed = 10f;
    [SerializeField] private AnimationCurve forwardSpeedCurve = default;
    [SerializeField] private RandomizedFloat backwardSpeed = 10f;
    [SerializeField] private AnimationCurve backwardSpeedCurve = default;
    [SerializeField] private RandomizedFloat startDelay = default;
    [SerializeField] private RandomizedFloat stopDelay = default;
    [SerializeField] private FollowerRotation rotateTowards = FollowerRotation.None;
    public Transform lookAt = default;
    [SerializeField] private IgnoredAxis lookAtAxis = default;
    [SerializeField] private bool destroyOnStop = default;
    [SerializeField] private bool unscaledDeltaTime = default;
    #endregion

    #region public events
    public Action readyToFollow;
    #endregion

    #region public properties
    [HideInInspector] public PathCreator pathCreator = default;
    #endregion

    private int currentLap => Mathf.FloorToInt(Mathf.Abs(totalDistanceTravelled) / pathCreator.path.length);

    private const float rotationLerpingSpeed = 10f;

    private int lapsPassed = 0;
    private bool canFollow = false;
    private float distanceTravelled = 0f;
    private float totalDistanceTravelled = 0f;
    
    private IgnoredAxis ignoredAxis = default;
    private Placer placer;

    #region private
    private void Awake() {
        placer = GetComponent<Placer>();
        placer.objectPlaced += onObjectPlacedOnPath;

        pathCreator = placer.pathCreator;
        ignoredAxis = placer.ignoredAxis;

        if(endInstruction == EndOfPathInstruction.Loop && pathCreator != null && !pathCreator.path.isClosedLoop) {
            endInstruction = EndOfPathInstruction.Reverse;
            backwardSpeed = forwardSpeed;
        }        
    }

    private void OnDestroy() => placer.objectPlaced -= onObjectPlacedOnPath;

    private void Update() {
        if (!canFollow) return;

        float speed = forwardSpeed.value;
        float maxLength = pathCreator.path.length;

        AnimationCurve curve = forwardSpeedCurve;
        if (endInstruction == EndOfPathInstruction.Reverse) {
            if (lapsPassed % 2 == 1) {
                speed = backwardSpeed.value;
                maxLength = pathCreator.path.length * 2f;
                curve = backwardSpeedCurve;
            }
        }

        float deltaTime = unscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime;
        float delta = speed * curve.Evaluate(Mathf.InverseLerp(0f, maxLength, distanceTravelled)) * deltaTime;
        if ((distanceTravelled + delta) - maxLength >= 0.05f) delta = Mathf.Clamp(maxLength - distanceTravelled, .01f, delta);
        
        distanceTravelled += delta;
        totalDistanceTravelled += delta;

        if (rotateTowards != Follower.FollowerRotation.None) {
            float distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
            Vector3 normal = pathCreator.path.GetNormalAtDistance(distanceTravelled).normalized;
            Quaternion nextRotation = transform.rotation;

            if (rotateTowards == Follower.FollowerRotation.Path) {
                nextRotation = pathCreator.path.GetRotationAtDistance(distanceTravelled);

                // by default transform.up look at target
                if (lookAtAxis == IgnoredAxis.right) nextRotation *= Quaternion.AngleAxis(90f, Vector3.forward);
                else if (lookAtAxis == IgnoredAxis.forward) nextRotation *= Quaternion.AngleAxis(90f, Vector3.left);

            } else if (rotateTowards == Follower.FollowerRotation.Transform && lookAt != null) {
                nextRotation = Quaternion.LookRotation(lookAt.position - transform.position);

                // by default transform.forward look at target
                if (lookAtAxis == IgnoredAxis.right) nextRotation *= Quaternion.AngleAxis(90f, Vector3.down);
                else if (lookAtAxis == IgnoredAxis.up) nextRotation *= Quaternion.AngleAxis(90f, Vector3.right);
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, nextRotation, (speed / 2f) * deltaTime);
        }

        Vector3 pointOnPath = pathCreator.path.GetPointAtDistance(distanceTravelled, endInstruction);

        transform.position = new Vector3(
            ignoredAxis.x ? transform.position.x : pointOnPath.x,
            ignoredAxis.y ? transform.position.y : pointOnPath.y,
            ignoredAxis.z ? transform.position.z : pointOnPath.z
        );

        if (lapsPassed < currentLap) {
            lapsPassed = currentLap;

            if (endInstruction != EndOfPathInstruction.Reverse) distanceTravelled = 0f;
            else distanceTravelled = lapsPassed % 2 == 1 ? pathCreator.path.length : 0f;

            if (stopDelay.value > 0f) StartCoroutine(_delay(stopDelay.value));
            if(destroyOnStop) Destroy(gameObject);
        }
    }

    private void onObjectPlacedOnPath(float distance){
        canFollow = true;
        distanceTravelled = distance;
        totalDistanceTravelled = distanceTravelled;        

        if (startDelay.value > 0f) StartCoroutine(_delay(startDelay.value));

        readyToFollow?.Invoke();
    }

    private IEnumerator _delay(float duration) {
        canFollow = false;
        float elapsed = 0f;
        while ((elapsed += Time.fixedDeltaTime) < duration) yield return null;
        canFollow = true;
    }

    private IEnumerator _followByDistance(float followDistance){
        canFollow = true;
        while(distanceTravelled < followDistance) yield return null;
        canFollow = false;
    }
    #endregion

    #region public
    public void startFollow() => canFollow = true;

    public void stopFollow() => canFollow = false;

    public void followByTime(float followTime) {
        float followDistance = pathCreator.path.GetClosestDistanceAlongPath(pathCreator.path.GetPointAtTime(followTime));
        StartCoroutine(_followByDistance(followDistance));
    }

    public void followByDistance(float followDistance) => StartCoroutine(_followByDistance(followDistance));
    #endregion
}
