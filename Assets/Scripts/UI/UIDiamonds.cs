using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class UIDiamonds : MonoBehaviour, IUIElement {
    #region editor
    [SerializeField] private Image diamondImage = default;
    [SerializeField] private TextMeshProUGUI diamondsCountText = default;
    [SerializeField] private Image uiDiamondPrefab = default;
    #endregion

    #region public properties
    public Image diamondPrefab => uiDiamondPrefab;
    public Vector3 imagePosition => diamondImage.rectTransform.position;
    #endregion

    private Animator animator;
    public RectTransform canvasRect;

    private Camera mainCamera;
    private GameSettings settings;

    #region private
    private void Awake() => animator = GetComponent<Animator>();

    private void Start() {
        settings = GameController.instance.gameSettings;
        mainCamera = GameController.instance.cameraManager.mainCamera;

        diamondsCountText.text = settings.diamondsCollected.ToString();
    }

    public Vector2 worldToCanvasPosition(Vector3 worldPosition){
        Vector2 tmp = mainCamera.WorldToViewportPoint(worldPosition);
        Vector2 pos = new Vector2((tmp.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * .5f),
                                  (tmp.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * .5f));
        return pos;
    }

    public void instantiateDiamond(Vector3 worldPosition){
        Vector2 canvasPosition = worldToCanvasPosition(worldPosition);
        Image go = Instantiate(uiDiamondPrefab, Vector3.zero, Quaternion.identity, canvasRect);
        go.rectTransform.anchoredPosition = canvasPosition;
        go.rectTransform.DOMove(diamondImage.rectTransform.position, 1f).OnComplete(() => {
            int diamonds = settings.addDiamond(); // save diamonds count
            diamondsCountText.text = diamonds.ToString();

            animator.SetTrigger("refresh");

            Destroy(go.gameObject);
        });
    }
    #endregion

    #region public
    public void addDiamondWorldSpace(Vector3 worldPosition) => instantiateDiamond(worldPosition);

    public void addDiamondUISpace(){
        int diamonds = settings.addDiamond();
        diamondsCountText.text = diamonds.ToString();

        animator.SetTrigger("refresh");
    }
    #endregion

    #region IUIElement
    public void show() {
        if(!gameObject.activeSelf) gameObject.SetActive(true);
        animator.SetTrigger("show");
    }

    public void hide() => animator.SetTrigger("hide");

    public void reset() => diamondsCountText.text = settings?.diamondsCollected.ToString() ?? "0";
    #endregion
}
