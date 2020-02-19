using UnityEngine;
using System.Collections;

public class SlowMotionManager : MonoBehaviour {
    public static SlowMotionManager instance;

    #region private
    private void Awake() {
        if (instance != null) Destroy(gameObject);
        else instance = this;

        reset();
    }

    private void Update() {
        Time.timeScale = Mathf.Clamp01(Time.timeScale);
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }

    private IEnumerator _slowDown(float duration, float slowDownFactor) {
        while(Time.timeScale > slowDownFactor) {
            Time.timeScale -= (1f / duration) * Time.unscaledDeltaTime;
            yield return null;
        }
    }

    private IEnumerator _slowUp(float duration) {
        while (Time.timeScale < 1f) {
            Time.timeScale += (1f / duration) * Time.unscaledDeltaTime;
            yield return null;
        }
    }
    
    private void reset() {
        StopAllCoroutines();
        Time.timeScale = 1f;
    }
    #endregion

    #region public
    public void startSlowMotion(float duration = 1f, float slowDownFactor = 0.05f) {
        StopAllCoroutines();
        StartCoroutine(_slowDown(duration, slowDownFactor));        
    }

    public void quitSlowMotion(float duration = 1f) {
        StopAllCoroutines();
        StartCoroutine(_slowUp(duration));    
    }

    public void oneShotSlowMotion(float duration = 1f, float slowDownFactor = 0.05f) {
        Time.timeScale = slowDownFactor;
        quitSlowMotion(duration);
    }
    #endregion
}
