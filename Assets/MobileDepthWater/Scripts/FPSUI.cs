namespace Assets.MobileOptimizedWater.Scripts
{
    using TMPro;
    using Assets.Scripts.Helpers;
    using UnityEngine;

    public class FPSUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI fpsText;

        private FPSCounter fpsCounter;

        public void Awake()
        {
            fpsCounter = new FPSCounter();
        }

        public void Update()
        {
            fpsCounter.Update(Time.deltaTime);
            fpsText.text = fpsCounter.GetAverageFps(1f).ToString("###");
        }
    }
}
