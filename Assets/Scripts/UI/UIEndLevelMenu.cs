using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

[RequireComponent(typeof(Animator))]
public class UIEndLevelMenu : MonoBehaviour, IUIElement {
    #region editor
    [SerializeField] private Button claimButton = default;
    [SerializeField] private UIDiamonds uiDiamonds = default;
    [SerializeField] private int diamondsToClaim = 25;
    [SerializeField] private float claimRadius = 150f;
    #endregion

    private Animator animator;

    #region private
    private void Awake() {
        animator = GetComponent<Animator>();

        claimButton.onClick.AddListener(claimDiamonds);
    }

    private async void claimDiamonds(){
        claimButton.gameObject.SetActive(false);

        for(int i = 0; i < diamondsToClaim; i++){
            Vector2 randomPosition = Random.insideUnitCircle * claimRadius;
            Image diamond = Instantiate(uiDiamonds.diamondPrefab, claimButton.transform.position, Quaternion.identity, transform);

            Sequence diamondSequence = DOTween.Sequence();
            diamondSequence.Append(diamond.rectTransform.DOAnchorPos(diamond.rectTransform.anchoredPosition + randomPosition, Random.Range(.1f, .4f)));
            diamondSequence.AppendInterval(Random.Range(0f, .3f));
            diamondSequence.Append(diamond.rectTransform.DOMove(uiDiamonds.imagePosition, 1f).SetEase(Ease.InQuad));

            diamondSequence.OnComplete(() => {
                Destroy(diamond.gameObject);

                uiDiamonds.addDiamondUISpace();
            });
        }

        await Task.Delay(2000);

        GameController.instance.nextLevel();
    }
    #endregion

    #region IUIElement
    public void show() {
        if(!gameObject.activeSelf) gameObject.SetActive(true);
        animator.SetTrigger("show");
        uiDiamonds.show();
    }

    public void hide() {
        uiDiamonds.hide();
        gameObject.SetActive(false);
    }

    public void reset() { 
        uiDiamonds.reset();
        claimButton.gameObject.SetActive(true);
    }
    #endregion
}
