using Utils;
using UnityEngine;
using DG.Tweening;

public class Blob : MonoBehaviour {
    #region editor
    [SerializeField] private SpriteRenderer sprite = default;
    #endregion

    private Sequence delaySequence;

    #region private
    private void Awake() {
        sprite.sprite = ColorScheme.instance.randomBlobSprite;
        transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        delaySequence = DOTween.Sequence();
        delaySequence.AppendInterval(1f);
        delaySequence.OnComplete(() => {
            transform.DOScale(.5f, 2f);
            sprite.DOColor(Utility.transparentColor, 3f).OnComplete(() => Destroy(gameObject));
        });
        delaySequence.Pause();
    }

    private void OnDestroy() => delaySequence?.Kill();
    #endregion

    #region public
    public void setColor(Color color) {
        sprite.color = color;
        delaySequence?.Play();
    }
    #endregion
}
