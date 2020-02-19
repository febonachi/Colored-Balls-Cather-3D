using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SphereCollider))]
public class Diamond : Collectable {
    #region editor
    [SerializeField] private ParticleSystem triggerPS = default;
    [SerializeField] private MeshRenderer mesh = default;
    #endregion

    private Vector3 cachedScale = Vector3.one;

    private SphereCollider sphereCollider;

    private UIDiamonds uiDiamonds;

    #region private
    private void Awake() {
        cachedScale = transform.localScale;
        sphereCollider = GetComponent<SphereCollider>();

        uiDiamonds = GameController.instance.uiManager.gameMenu.diamonds;
    }
    #endregion

    #region public
    public override void trigger() {
        sphereCollider.enabled = false;

        triggerPS?.Play();
        mesh.enabled = false;
        uiDiamonds.addDiamondWorldSpace(transform.position);
    }
    #endregion

    #region PoolObject
    public override void reuse() {
        mesh.enabled = true;
        sphereCollider.enabled = true;
        transform.localScale = cachedScale;
    }
    #endregion
}
