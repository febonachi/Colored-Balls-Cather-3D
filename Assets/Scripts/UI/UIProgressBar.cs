using TMPro;
using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class UIProgressBar : MonoBehaviour {
    #region editor
    [Tooltip("Because of UI canvas render order system")]
    [SerializeField] private Transform[] backgroundImagesHolder = default;
    [SerializeField] private Image currentLevelImage = default;
    [SerializeField] private TextMeshProUGUI currentLevelText = default;
    [SerializeField] private Image nextLevelImage = default;
    [SerializeField] private TextMeshProUGUI nextLevelText = default;
    [SerializeField] private Transform lineImagesHolder = default;
    [Tooltip("Stage image must have only one child as type Image (show when stage fillAmount = 100%)")]
    [SerializeField] private Transform stageImagesHolder = default;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color fillColor = Color.white;
    [SerializeField] private Color backgroundColor = Color.white;
    [SerializeField] private float fillSpeed = 1f;
    #endregion

    private float fillAmount = 0f;
    private bool inProgress = false;
    private int currentStageIndex = 0;

    private List<Image> lineImages;
    private List<Image> stageImages;
    private List<Image> backgroundImages = new List<Image>();

    private Coroutine fillCoroutine;
    private Animator animator;

    #region private
    private void Awake() {
        animator = GetComponent<Animator>();

        lineImages = lineImagesHolder.GetComponentsInChildren<Image>().ToList();
        stageImages = stageImagesHolder.GetComponentsInChildren<Image>(includeInactive: false).ToList();
        backgroundImages = backgroundImagesHolder.SelectMany(holder => holder.GetComponentsInChildren<Image>()).ToList();
    }

    private IEnumerator _fillStage(){
        inProgress = true;

        Image stageImage = stageImages[currentStageIndex];
        stageImage.color = fillColor;
        while(stageImage.fillAmount < fillAmount){
            stageImage.fillAmount += fillSpeed * Time.deltaTime;
            yield return null;
        }

        if(stageImage.fillAmount >= 1f) stageFilled();

        inProgress = false;
    }

    private void stageFilled(){
        fillAmount = 0f;

        Image currentStageImage = stageImages[currentStageIndex];
        currentStageImage.fillAmount = 1f;
        getStageChildImage(currentStageImage).gameObject.SetActive(true); // show filled stage image

        if(currentStageIndex == stageImages.Count - 1){ // last stage completed
            Image lineImage = lineImages[currentStageIndex + 1];
            lineImage.color = fillColor;
            lineImage.DOFillAmount(1f, .5f);

            nextLevelImage.DOColor(fillColor, .5f);
        }
    }

    private Image getStageChildImage(Image stage) => stage.transform.GetChild(0).GetComponent<Image>();
    #endregion

    #region public
    public void initialize(int currentLevel){
        reset();

        currentLevelImage.DOColor(fillColor, .5f);
        Image lineImage = lineImages.First();
        lineImage.DOColor(fillColor, .1f);
        lineImage.DOFillAmount(1f, .5f);

        currentLevelText.text = currentLevel.ToString();
        nextLevelText.text = (currentLevel + 1).ToString();
    }

    public void setProgress(float value){
        if(currentStageIndex >= stageImages.Count) return;

        fillAmount = Mathf.Clamp01(value);

        if(!inProgress) fillCoroutine = StartCoroutine(_fillStage());
    }

    public void nextStage(int stage){
        currentStageIndex = stage;

        Image lineImage = lineImages[currentStageIndex];
        lineImage.color = fillColor;
        lineImage.DOFillAmount(1f, .5f);
    }
    #endregion

    #region IUIElement
    public void show() {
        if(!gameObject.activeSelf) gameObject.SetActive(true);
        animator.SetTrigger("show");
    }

    public void hide() => animator.SetTrigger("hide");

    public void reset(){
        currentLevelImage.color = defaultColor;
        currentLevelText.text = "0";
        nextLevelImage.color = fillColor;
        nextLevelText.text = "1";

        lineImages.ForEach(line => {
            line.fillAmount = 0f;
            line.color = defaultColor;
        });

        stageImages.ForEach(stage => {
            stage.fillAmount = 0f;
            stage.color = defaultColor;

            Image check = getStageChildImage(stage);
            check.gameObject.SetActive(false);
        });
        
        backgroundImages.ForEach(image => image.color = backgroundColor);
    }
    #endregion
}
