using UnityEngine;

public class UIGameMenu : MonoBehaviour, IUIElement {
    #region editor
    [SerializeField] private UIScore uiScore = default;
    [SerializeField] private UIDiamonds uiDiamonds = default;
    [SerializeField] private UIProgressBar uiProgressBar = default;
    #endregion

    #region public properties
    public UIScore score => uiScore;
    public UIDiamonds diamonds => uiDiamonds;
    public UIProgressBar progressBar => uiProgressBar;
    #endregion

    #region IUIElement
    public void show() {
        if(!gameObject.activeSelf) gameObject.SetActive(true);
        uiScore.show();
        uiDiamonds.show();
        uiProgressBar.show();
    }

    public void hide() {
        uiScore.hide();
        uiDiamonds.hide();
        uiProgressBar.hide();
    }

    public void reset() {
        uiScore.reset();
        uiDiamonds.reset();
        uiProgressBar.reset();
    }
    #endregion
}
