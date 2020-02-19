using Utils;
using DG.Tweening;
using UnityEngine;

public class GunGoal : MonoBehaviour {
    #region editor
    [SerializeField] private SpriteRenderer sprite = default;
    [SerializeField] private GoalBoundary goalBoundary = default;
    #endregion

    #region public properties
    public GoalBoundary boundary => goalBoundary;
    #endregion

    private float alpha = 1f;

    #region private
    private void Awake() => alpha = sprite.color.a;
    #endregion

    #region public
    public void setColor(Color color, bool cachedAlpha = true) {
        Color targetColor = color;
        sprite.DOKill();
        if(cachedAlpha) {
            targetColor.a = alpha;
            sprite.DOColor(targetColor, .75f);
        } else sprite.DOColor(color, .5f);
    }

    public void shakeScale() => transform.DOShakeScale(.5f, new Vector3(.5f, 0f, .5f));
    #endregion
}
