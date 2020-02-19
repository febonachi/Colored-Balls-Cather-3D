using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UIScoreBonusText : MonoBehaviour, IUIElement {
    #region editor
    [SerializeField] private TextMeshProUGUI bonusText = default;
    [SerializeField] private Color[] bonusTextColors = default;
    [SerializeField] private string[] bonusTextList = default;
    #endregion

    private Animator animator;
    private RectTransform rectTransform;

    #region private
    private void Awake() {
        animator = GetComponent<Animator>();
        rectTransform = GetComponent<RectTransform>();
    }
    #endregion

    #region public
    public void addScore(int value, int ballsInARow, bool fever){
        if(value <= 1 || ballsInARow <= 1) return;

        bonusText.color = bonusTextColors[Random.Range(0, bonusTextColors.Length)];

        bonusText.text = bonusTextList[Random.Range(0, bonusTextList.Length)];

        rectTransform.anchoredPosition = new Vector3(Random.Range(-100f, 100f), Random.Range(-150f, -200f), 0f);
        rectTransform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-10f, 10f));

        bonusText.gameObject.SetActive(true);

        animator.SetTrigger("show"); // play animation
    }
    #endregion

    #region IUIElement
    public void show() {
        if(!gameObject.activeSelf) gameObject.SetActive(true);
    }

    public void hide() => gameObject.SetActive(false);

    public void reset() {
        bonusText.text = "";
        bonusText.color = Color.white;
        rectTransform.rotation = Quaternion.identity;
        rectTransform.anchoredPosition = Vector3.zero;
        bonusText.gameObject.SetActive(false);
    }
    #endregion
}
