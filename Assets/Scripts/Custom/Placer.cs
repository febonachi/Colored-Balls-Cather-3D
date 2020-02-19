using Utils;
using System;
using UnityEngine;
using PathCreation;

public class Placer : MonoBehaviour {
    #region editor
    public PathCreator pathCreator;
    public IgnoredAxis ignoredAxis;
    [SerializeField] private bool placeByClosestPoint = true;
    [SerializeField] private float time = 0f;
    [SerializeField] private Follower.FollowerRotation rotateTowards = Follower.FollowerRotation.None;
    [SerializeField] private Transform lookAt = default;
    [SerializeField] private IgnoredAxis lookAtAxis = default;
    #endregion

    #region public events
    public Action<float> objectPlaced;
    #endregion

    #region private
    private void Start() {
        placeToPath();
        rotateTransform();
    }
    #endregion

    #region public
    public void placeToPath(){
        if(pathCreator == null) return;
        
        Vector3 currentPosition = transform.position;
        Vector3 pointOnPath = placeByClosestPoint ? pathCreator.path.GetClosestPointOnPath(currentPosition) : pathCreator.path.GetPointAtTime(time);

        transform.position = new Vector3(
            ignoredAxis.x ? currentPosition.x : pointOnPath.x,
            ignoredAxis.y ? currentPosition.y : pointOnPath.y,
            ignoredAxis.z ? currentPosition.z : pointOnPath.z
        );
        
        objectPlaced?.Invoke(pathCreator.path.GetClosestDistanceAlongPath(transform.position));
    }

    public void rotateTransform() {
        if (pathCreator == null) return;

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

            transform.rotation = nextRotation;
        }
    }
    #endregion
}
