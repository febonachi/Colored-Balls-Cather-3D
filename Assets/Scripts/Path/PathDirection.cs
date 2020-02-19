using Utils;
using System;
using System.Linq;
using UnityEngine;
using PathCreation;
using System.Collections;
using System.Collections.Generic;

public class PathDirection : MonoBehaviour {
    #region editor
    [Header("Path Settings")]
    [SerializeField] private bool showPath = true;
    [SerializeField] private PathCreator pathCreatorPrefab = default;
    [SerializeField] private float pathAutoControlLength = .35f;
    [Tooltip("Middle point offset in %")]
    [Range(0f, 1f)][SerializeField] private float percentile = .5f;
    [Range(1, 100)] [SerializeField] private int pointsCount = 24;
    [Range(0f, 2f)] [SerializeField] private float pointsOffset = .25f;
    [SerializeField] private PathHighlighter pointPrefab = default;
    [SerializeField] private MinMaxValue middlePointHeightGap = new Tuple<float, float>(18f, 24f);  
    [SerializeField] private MinMaxValue horizontalOffset = default;
    [Tooltip("Works with pathController horizontalOffset")]
    [SerializeField] private bool middlePointSmartMoving = false;
    public Transform startPoint = default;
    public Transform stopPoint = default;
    #endregion

    #region public properties
    public VertexPath path => pathCreator.path;
    public Vector3 firstAnchor => pathCreator.bezierPath.GetPoint(1);
    public Vector3 middleAnchor => pathCreator.bezierPath.GetPoint(3);
    public Vector3 lastAnchor => pathCreator.bezierPath.GetPoint(5);
    #endregion

    private const float minSpacing = .1f;
    private const int middlePointIndex = 3;

    private float offset = 0f;
    private bool initialized = false;
    private float middlePointHeight = 0f;
    private Color startUpColor = Color.white;
    private bool canHighlightPoints = true;

    private Transform pointsHolder;
    private PathCreator pathCreator;
    private List<PathHighlighter> pathHighlighters;

    #region private
    private void initializeBezierPath() {
        pathCreator = Instantiate(pathCreatorPrefab, transform);

        BezierPath bp = pathCreator.bezierPath;
        bp.AutoControlLength = pathAutoControlLength;
        bp.MovePoint(0, startPoint.position);
        bp.MovePoint(bp.NumPoints - 1, stopPoint.position);
        Vector3 anchorPoint = pathCreator.path.GetPointAtTime(percentile);
        bp.SplitSegment(anchorPoint, 0, 0);

        Vector3 pointPosition = bp.GetPoint(middlePointIndex); 
        bp.MovePoint(middlePointIndex, new Vector3(pointPosition.x, middlePointHeight, pointPosition.z));

        pathCreator.InitializeEditorData(false);
    }

    private void initializePoints() {
        GameObject pointsHolderObject = new GameObject("points");
        pointsHolderObject.transform.SetParent(transform);
        pointsHolder = pointsHolderObject.transform ?? transform;

        pathHighlighters = new List<PathHighlighter>(pointsCount);
        for (int i = 0; i < pointsCount; i++) {
            PathHighlighter point = Instantiate(pointPrefab, pointsHolder);
            point.gameObject.SetActive(showPath);
            pathHighlighters.Add(point);
        }
    }

    private void updateBezierPath(float direction) {      
        if((offset + direction) < horizontalOffset.min || (offset + direction) > horizontalOffset.max) direction = 0f;
        offset += direction;

        BezierPath bp = pathCreator.bezierPath;
        bp.MovePoint(0, startPoint.position);
        bp.MovePoint(bp.NumPoints - 1, stopPoint.position);
        Vector3 crossDirection = Vector3.Cross(startPoint.position - stopPoint.position, Vector3.up).normalized;
        Vector3 middlePointPosition = pathCreator.path.GetPointAtTime(percentile);
        middlePointPosition.y = middlePointHeight;
        Vector3 nextPos = middlePointPosition + (crossDirection * direction);            
        bp.MovePoint(middlePointIndex, nextPos);
    }

    private void updatePointsPosition() {
        VertexPath path = pathCreator.path;
        float maxSpacing = (path.length - (pointsOffset * 2)) / pointsCount;
        float spacing = Mathf.Max(minSpacing, maxSpacing);

        float step = pointsOffset;
        foreach (PathHighlighter go in pathHighlighters) {
            go.transform.position = path.GetPointAtDistance(step);
            step += spacing;
        }
    }

    private IEnumerator _highlightPoints(Color color, bool once = false){
        yield return new WaitUntil(() => pathHighlighters.All(ph => ph.readyToHighlight));

        if(!canHighlightPoints) yield break;

        foreach (PathHighlighter ph in pathHighlighters) {
            ph.highlight(color);
            yield return new WaitForSeconds(.01f);
        }

        if(!once) {
            yield return new WaitForSeconds(.5f);

            StartCoroutine(_highlightPoints(color, once));
        }
    }

    private void OnDrawGizmos() {
        if(pathCreator == null) return;

        BezierPath bp = pathCreator.bezierPath;

        Gizmos.color = Color.magenta;
        Vector3 middlePointPosition = bp.GetPoint(middlePointIndex);
        middlePointPosition.y = middlePointHeight;
        Gizmos.DrawSphere(middlePointPosition, .4f);

        Gizmos.color = Color.green;
        Vector3 crossDirection = Vector3.Cross(startPoint.position - stopPoint.position, Vector3.up).normalized;
        Vector3 leftPoint = middlePointPosition + (crossDirection * (horizontalOffset.min - offset));
        Vector3 rightPoint = middlePointPosition + (crossDirection * (horizontalOffset.max - offset));
        Gizmos.DrawSphere(leftPoint, .4f);
        Gizmos.DrawSphere(rightPoint, .4f);
    }
    #endregion

    #region public
    public void initialize(float horOffset = 0f, Transform start = null, Transform stop = null) {
        offset = 0f;

        if(start != null) startPoint = start;
        if(stop != null) stopPoint = stop;

        middlePointHeight = middlePointHeightGap.random;
        
        initializePoints();
        initializeBezierPath();

        initialized = pathCreator != null;

        moveMiddleAnchor(horOffset.map(-1f, 1f, horizontalOffset.min, horizontalOffset.max));
    }

    public void moveMiddleAnchor(float direction = 0f) {
        if(!initialized) return;

        pointsHolder.gameObject.SetActive(showPath);
        updateBezierPath(direction);
        updatePointsPosition();
    }

    public void reset() {
        offset = 0f;

        if(!initialized) return;

        pointsHolder.gameObject.SetActive(showPath);

        BezierPath bp = pathCreator.bezierPath;
        bp.MovePoint(0, startPoint.position);
        bp.MovePoint(bp.NumPoints - 1, stopPoint.position);
        if(middlePointSmartMoving){
            Vector3 crossDirection = Vector3.Cross(startPoint.position - stopPoint.position, Vector3.up).normalized;
            Vector3 middlePointPosition = pathCreator.path.GetPointAtTime(percentile);
            Vector3 nextPos = middlePointPosition + (crossDirection * offset);            
            middlePointPosition.y = middlePointHeight;
            bp.MovePoint(middlePointIndex, nextPos);
        }else{
            float distance = Vector3.Distance(startPoint.position, stopPoint.position) * percentile;
            Vector3 direction = (stopPoint.position - startPoint.position).normalized;
            Vector3 middlePointPosition = startPoint.position + (direction * distance);
            middlePointPosition.y = middlePointHeight;
            bp.MovePoint(middlePointIndex, middlePointPosition);
        }

        updatePointsPosition();
    }

    public void showPoints() {
        if(!initialized) return;

        pointsHolder.gameObject.SetActive(true);
    }

    public void hidePoints() {
        if(!initialized) return;

        pointsHolder.gameObject.SetActive(false);
    }

    public void highlightPointsImmediately(Color color) {
        startUpColor = color;

        pathHighlighters.ForEach(ph => ph.highlightImmediately(color));
    }

    public IEnumerator _highlightPoints(Color color) {
        canHighlightPoints = true;

        yield return StartCoroutine(_highlightPoints(color, once: false)); // fcking overloading... 
    }

    public IEnumerator _highlightPointsOnce(Color color) {
        canHighlightPoints = true;

        yield return StartCoroutine(_highlightPoints(color, once: true));
    }

    public void stopHighlightPoints() => canHighlightPoints = false;
    #endregion
}
