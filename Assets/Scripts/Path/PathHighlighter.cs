using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(MeshRenderer))]
public class PathHighlighter : MonoBehaviour {
    #region public properties
    public bool readyToHighlight {get; private set;} = true;
    #endregion

    private float cachedAlpha = 1f;

    private MeshRenderer mesh;

    #region private
    private void Awake() {
        mesh = GetComponent<MeshRenderer>();

        cachedAlpha = mesh.material.color.a;
    }
    #endregion

    #region public
    public void highlight(Color color){
        color.a = cachedAlpha;
        readyToHighlight = false;

        float tweenDuration = .4f;
        transform.DOScale(transform.localScale * 1.75f, tweenDuration).SetLoops(2, LoopType.Yoyo);
        mesh.material.DOColor(color, tweenDuration).OnComplete(() => readyToHighlight = true);
    }

    public void highlightImmediately(Color color) {
        color.a = cachedAlpha;
        mesh.material.color = color;
    }
   #endregion
}
