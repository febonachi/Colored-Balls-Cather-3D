using UnityEngine;
using DG.Tweening;
using System.Collections;

public class GunWalls : MonoBehaviour {
    #region editor
    [SerializeField] private GunCannon gunCannon = default;
    #endregion

    #region public properties
    public GunCannon cannon => gunCannon;
    #endregion

    private const float waterDeep = -50f;

    private float cachedHeight = 0;

    #region private
    private void Awake() {
        transform.position = new Vector3(transform.position.x, Random.Range(12f, 24f), transform.position.z); // randomize walls height

        cachedHeight = transform.position.y;
        gunCannon.gameObject.SetActive(false);
    }

    private void Start() => transform.position = new Vector3(transform.position.x, waterDeep, transform.position.z); // <Start> because gun initialized path first
    #endregion

    #region public
    public IEnumerator _moveUpFromWater(){
        Tweener tween = transform.DOMoveY(cachedHeight, 2f);
        yield return tween.WaitForPosition(1.5f);
        gunCannon.moveUpFromPlatform();
        yield return null;
    }

    public void moveDownToWater(){
        gunCannon.moveDownToPlatform();
        transform.DOMoveY(waterDeep, 3f).OnComplete(() => gameObject.SetActive(false));
    }
    #endregion
}
