using System.Linq;
using UnityEngine;
using System.Collections;

public class PathController : MonoBehaviour {
    #region editor
    public PathDirection direction = default;
    [Range(-1f, 1f)][SerializeField] private float offset = 0f;
    #endregion

    #region public properties
    
    #endregion

    #region public
    public void initialize(Transform start = null, Transform stop = null){
        clear();

        direction.initialize(offset, start, stop);
    }

    public void resetPath() => direction.reset();

    public void showPath() => direction.showPoints();

    public void hidePath() => direction.hidePoints();

    public void moveMiddleAnchor(float dir = 0f) => direction.moveMiddleAnchor(dir);

    public void clear() => direction.transform.Cast<Transform>().ToList().ForEach(t => DestroyImmediate(t.gameObject));
    #endregion
}
