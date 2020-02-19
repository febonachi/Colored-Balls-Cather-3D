using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(MeshRenderer))]
public class GunPlatform : MonoBehaviour {
    private Material material;

    #region private
    private void Awake() => material = GetComponent<MeshRenderer>().material;
    #endregion

    #region public
    public void setColor(Color color) => material.DOColor(color, 1f);
    #endregion
}
