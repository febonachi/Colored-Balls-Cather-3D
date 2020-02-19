using Utils;
using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ToyCrumblePicker : MonoBehaviour {
    #region editor
    [SerializeField] private float overlapRadius = 1f;
    [SerializeField] private float pickForce = 1f;
    [SerializeField] private LayerMask overlapMask = default;
    #endregion

    #region public events
    public Action crumblePicked;
    #endregion

    private float cachedOverlapRadius = 1f;
    private float cachedSphereRadius = 1f;

    private Toy toy;
    private SphereCollider sphereCollider;

    #region private
    private void Awake() {
        toy = GetComponentInParent<Toy>();
        sphereCollider = GetComponent<SphereCollider>();

        cachedOverlapRadius = overlapRadius;
        cachedSphereRadius = sphereCollider.radius;
    }

    private void Update() {
        Collider[] colliders = Physics.OverlapSphere(toy.transform.position, overlapRadius, overlapMask);
        foreach (Collider collider in colliders) collider.GetComponent<CrumbleObject>()?.moveTowards(transform, pickForce);
    }

    //private void OnTriggerEnter(Collider other) => trigger(other);
       
    private void OnTriggerStay(Collider other) => trigger(other);

    private void trigger(Collider other){
        if(Utility.checkoutLayer(overlapMask, other.gameObject.layer)) {
            if(other.CompareTag("Collectable")) {
                other.GetComponent<Collectable>().trigger();
            }else if(other.CompareTag("CrumbleObject")) {
                CrumbleObject co = other.GetComponent<CrumbleObject>();
                if (co.activated) {
                    other.GetComponent<CrumbleObject>().trigger();
                    crumblePicked?.Invoke();
                }
            }
        }
    }

    private void OnDrawGizmos() => Gizmos.DrawWireSphere(transform.position, overlapRadius);
    #endregion

    #region public
    public void scaleUp(float scaleFactor) {
        overlapRadius = cachedOverlapRadius * scaleFactor;
        sphereCollider.radius = cachedSphereRadius * scaleFactor;
    }

    public void scaleDown() {
        overlapRadius = cachedOverlapRadius;
        sphereCollider.radius = cachedSphereRadius;
    }
    #endregion
}
