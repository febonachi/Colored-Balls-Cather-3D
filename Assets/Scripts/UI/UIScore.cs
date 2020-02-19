using Utils;
using TMPro;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class UIScore : MonoBehaviour, IUIElement {
    #region editor
    [SerializeField] private TextMeshProUGUI scoreText = default;
    [SerializeField] private UIScoreBonusText scoreBonusText = default;
    [SerializeField] private TextMeshProUGUI textPrefab = default;
    [SerializeField] private float textPrefabDelay = .2f;
    #endregion

    #region public properties
    public int score => currentScore;
    #endregion

    private float elapsed = 0f;
    private int currentScore = 0;
    private bool canInstantiateTextPrefab = true;

    private Animator animator;

    #region private
    private void Awake() => animator = GetComponent<Animator>();

    private void Update() {
        if(canInstantiateTextPrefab) return;

        elapsed += Time.deltaTime;
        if(elapsed >= textPrefabDelay){
            elapsed = 0f;
            canInstantiateTextPrefab = true;
        }
    }
    #endregion

    #region public
    public void addScore(int value, int ballsInARow, bool fever){
        if(currentScore + value < 0) return;

        currentScore += value;
        scoreText.text = currentScore.ToString();
        animator.SetTrigger("refresh");

        scoreBonusText.addScore(value, ballsInARow, fever);

        if (canInstantiateTextPrefab){
            Vector3 randomPosition = new Vector3(Random.Range(-50f, 50f), Random.Range(-100f, -125f), 0f);
            TextMeshProUGUI text = Instantiate(textPrefab, transform);
            text.rectTransform.anchoredPosition = randomPosition;
            text.text = value >= 0 ? $"+{value}" : $"{value}";

            text.DOColor(Utility.transparentColor, 1.25f);
            text.rectTransform.DOAnchorPos(scoreText.rectTransform.anchoredPosition, 1f).OnComplete(() => Destroy(text.gameObject));

            canInstantiateTextPrefab = false;
        }
    }
    #endregion

    #region IUIElement
    public void show() {
        if(!gameObject.activeSelf) gameObject.SetActive(true);
        scoreBonusText.show();
    }

    public void hide() {
        gameObject.SetActive(false);
        scoreBonusText.hide();
    }

    public void reset() {
        currentScore = 0;
        scoreBonusText.reset();
        scoreText.text = currentScore.ToString();
    }
    #endregion
}
