using UnityEngine;

public class UIMainMenu : MonoBehaviour, IUIElement {
    #region editor
    [SerializeField] private RectTransform help = default;
    [SerializeField] private RectTransform loadingRect = default;
    [SerializeField] private UISettingsPanel uiSettingsPanel = default;
    #endregion

    #region private
    private void Awake() => loadingRect.gameObject.SetActive(true);
    #endregion

    #region IUIElement
    public void show() {
        if (loadingRect.gameObject.activeSelf) loadingRect.gameObject.SetActive(false);
        if(!gameObject.activeSelf) gameObject.SetActive(true);
        help.gameObject.SetActive(true);
        uiSettingsPanel.show();
    }

    public void hide() {
        help.gameObject.SetActive(false);
        uiSettingsPanel.hide();
    }

    public void reset() { 
        help.gameObject.SetActive(true);
        uiSettingsPanel.reset();
    }
    #endregion
}
