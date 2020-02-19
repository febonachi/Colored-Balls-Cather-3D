using UnityEngine;
using DG.Tweening;
using System.Collections;

public class ToyPlatform : MonoBehaviour {
    #region editor
    [SerializeField] private Transform pipe = default;
    [SerializeField] private Transform floor = default;
    [SerializeField] private Transform border = default;
    #endregion

    private float cachedBorderY = 0f;
    private MeshRenderer floorMesh;

    #region private
    private void Awake() {
        cachedBorderY = border.position.y;

        floorMesh = floor.GetComponent<MeshRenderer>();
    }
    #endregion

    #region public
    public void setFloorColor(Color color) {
        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);
        floorMesh.material.DOColor(Color.HSVToRGB(h, .2f, v), 1f);
    }

    public IEnumerator _openPlatformDoors(){
        Tweener tweener = border.DOMoveY(-cachedBorderY, 1f);
        yield return tweener.WaitForCompletion();
    }

    public IEnumerator _closePlatformDoors(){
        Tweener tweener = border.DOMoveY(cachedBorderY, 1f);
        yield return tweener.WaitForCompletion();
    }
    #endregion
}
