using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class UIRestartLevelMenu : MonoBehaviour, IUIElement {
    #region editor    
    [SerializeField] private Button restartButton = default;
    [SerializeField] private TextMeshProUGUI levelProgressText = default;
    #endregion

    private Animator animator;

    #region private
    private void Awake() {
        animator = GetComponent<Animator>();

        restartButton.onClick.AddListener(onRestartButtonClick);
    }

    private void onRestartButtonClick() => GameController.instance.restartLevel();
    #endregion

    #region IUIElement
    public void show() {
        if(!gameObject.activeSelf) gameObject.SetActive(true);

        levelProgressText.text = $"{GameController.instance.gameSettings.savedProgress}% completed!";

        animator.SetTrigger("show");
    }

    public void hide() { 
        
    }

    public void reset() => levelProgressText.text = "";
    #endregion
}
