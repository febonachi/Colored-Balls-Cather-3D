using UnityEngine;
using PathCreation;
using System.Collections;

public class CameraTarget : MonoBehaviour {
    #region editor
    [SerializeField] private float transitionSpeed = 1f;
    [SerializeField] private PathCreator waypointsPathCreator = default;
    #endregion

    #region public properties
    public float speed => transitionSpeed;
    #endregion

    private int waypointIndex = 0;
    private BezierPath waypointsPath;

    #region private
    private void Awake() {
        waypointsPath = waypointsPathCreator.bezierPath;

        transform.position = waypointsPathCreator.path.GetPoint(0);

        CameraManager cameraManager = GameController.instance.cameraManager;
        cameraManager.virtualCamera.Follow = transform;
        cameraManager.virtualCamera.LookAt = transform;

        waypointsPathCreator.InitializeEditorData(false);
    }

    //private void OnDrawGizmos() {
    //    if(waypointsPathCreator == null) return;

    //    Gizmos.color = Color.green;
    //    for(int i = 0; i < waypointsPathCreator.bezierPath.NumAnchorPoints; i++) Gizmos.DrawSphere(waypointsPathCreator.bezierPath.GetPoint(i * 3), 2f);
    //}
    #endregion

    #region public
    public void initializeWaypoints(Vector3[] points){
        transform.position = points[0];
        for(int i = 0; i < points.Length; i++) waypointsPath.MovePoint(i * 3, points[i]);
    }

    public IEnumerator _moveToNextWaypoint(){
        if(++waypointIndex == waypointsPath.NumAnchorPoints) waypointIndex = 0;

        Vector3 waypointPosition = waypointsPath.GetPoint(waypointIndex * 3);

        while(transform.position != waypointPosition){
            transform.position = Vector3.MoveTowards(transform.position, waypointPosition, transitionSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public Vector3 closestPoint(Vector3 target) => waypointsPathCreator.path.GetClosestPointOnPath(target);

    public Vector3 nextWaypoint() {
        int localWaypointIndex = waypointIndex + 1;
        if(localWaypointIndex == waypointsPath.NumAnchorPoints) localWaypointIndex = 0;

        Vector3 nextWaypointPosition = waypointsPath.GetPoint(localWaypointIndex * 3);

        waypointsPath.MovePoint(waypointIndex * 3, nextWaypointPosition);

        return nextWaypointPosition;
    }
    #endregion
}
