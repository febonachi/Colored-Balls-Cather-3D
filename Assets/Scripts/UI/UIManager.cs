using UnityEngine;
using System.Collections.Generic;

public class UIManager : Manager {
    #region editor
    [SerializeField] private UIMainMenu uiMainMenu = default;
    [SerializeField] private UIGameMenu uiGameMenu = default;
    [SerializeField] private UIEndLevelMenu uiEndLevelMenu = default;
    [SerializeField] private UIRestartLevelMenu uiRestartLevelMenu = default;
    #endregion

    #region public properties
    public UIMainMenu mainMenu => uiMainMenu;
    public UIGameMenu gameMenu => uiGameMenu;
    public UIEndLevelMenu endLevelMenu => uiEndLevelMenu;
    public UIRestartLevelMenu restartLevelMenu => uiRestartLevelMenu;
    #endregion

    private List<GameObject> uiElements = new List<GameObject>();

    #region private
    private void hideUIElements() => uiElements.ForEach(uiElement => uiElement.GetComponent<IUIElement>()?.hide());

    private void hideUIElementsImmediately() => uiElements.ForEach(uiElement => uiElement.SetActive(false));
    #endregion

    #region public
    public override void initialize() {
        base.initialize();

        // make list of all ui elements
        uiElements.Add(uiMainMenu.gameObject);
        uiElements.Add(uiGameMenu.gameObject);
        uiElements.Add(uiEndLevelMenu.gameObject);
        uiElements.Add(uiRestartLevelMenu.gameObject);

        status = Status.Ready;
    }

    public void resetUIElements() {
        uiElements.ForEach(uiElement => {
            uiElement.GetComponent<IUIElement>()?.reset();
            uiElement.SetActive(false);
        });
    }

    public void showMainMenu(){
        hideUIElementsImmediately();

        uiMainMenu.show();
    }

    public void showGameMenu(){
        uiMainMenu.hide();
        uiGameMenu.show();
    }

    public void showEndLevelMenu(){
        uiGameMenu.hide();
        uiEndLevelMenu.show();
    }

    public void showRestartLevelMenu(){
        uiGameMenu.hide();
        uiRestartLevelMenu.show();
    }
    #endregion
}
