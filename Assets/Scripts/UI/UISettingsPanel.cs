using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

[RequireComponent(typeof(Animator))]
public class UISettingsPanel : MonoBehaviour, IUIElement {
    #region editor
    [SerializeField] private Button settingsButton = default;
    [SerializeField] private Button soundButton = default;
    [SerializeField] private Button vibrationButton = default;

    [Space(20)]
    [SerializeField] private Image soundButtonImage = default;
    [SerializeField] private Sprite soundButtonOnSprite = default;
    [SerializeField] private Sprite soundButtonOffSprite = default;

    [Space(20)]
    [SerializeField] private Image vibrationButtonImage = default;
    [SerializeField] private Sprite vibrationButtonOnSprite = default;
    [SerializeField] private Sprite vibrationButtonOffSprite = default;
    #endregion

    private bool closed = true;

    private Animator animator;

    private GameSettings gameSettings;

    #region private
    private void Awake() {
        animator = GetComponent<Animator>();

        settingsButton.onClick.AddListener(() => {
            if(closed) openPanel();
            else closePanel();
        });

        soundButton.onClick.AddListener(onSoundButtonClicked);
        vibrationButton.onClick.AddListener(onVibrationButtonClicked);
    }

    private void Start() => gameSettings = GameController.instance.gameSettings;

    private void openPanel(){
        closed = false;

        animator.SetTrigger("open");

        soundButton.gameObject.SetActive(true);
        vibrationButton.gameObject.SetActive(true);
    }

    private void closePanel() {
        closed = true;

        animator.SetTrigger("close");
    }

    private void onSoundButtonClicked(){
        bool sound = gameSettings.sound;
        Sprite soundSprite = sound ? soundButtonOffSprite : soundButtonOnSprite;
        soundButtonImage.sprite = soundSprite;
        
        gameSettings.setSound(!sound);
    }

    private void onVibrationButtonClicked(){
        bool vibration = gameSettings.vibration;
        Sprite vibrationSprite = vibration ? vibrationButtonOffSprite : vibrationButtonOnSprite;
        vibrationButtonImage.sprite = vibrationSprite;

        gameSettings.setVibration(!vibration);
    }
    #endregion

    #region IUIElement
    public void show() {
        if(!gameObject.activeSelf) gameObject.SetActive(true);
        animator.SetTrigger("show");
    }

    public async void hide() {
        if(!closed) {
            closePanel();

            await Task.Delay(500);

            soundButton.gameObject.SetActive(false);
            vibrationButton.gameObject.SetActive(false);
        }

        animator.SetTrigger("hide");
    }

    public void reset() { 
        closed = true;

        bool soundOn = gameSettings.sound;
        Sprite soundSprite = soundOn ? soundButtonOnSprite : soundButtonOffSprite;
        soundButtonImage.sprite = soundSprite;
        soundButton.gameObject.SetActive(false);

        bool vibrationOn = gameSettings.vibration;
        Sprite vibrationSprite = vibrationOn ? vibrationButtonOnSprite : vibrationButtonOffSprite;
        vibrationButtonImage.sprite = vibrationSprite;
        vibrationButton.gameObject.SetActive(false);
    }
    #endregion
}
