using Utils;
using System;
using UnityEngine;

public class ToyBall : MonoBehaviour {
    #region editor
    [SerializeField] private GameObject initializePS = default;
    [SerializeField] private GameObject destroyPS = default;
    [SerializeField] private LayerMask platformLayerMask = default;
    #endregion

    #region public properties
    public Color color {get; private set;}
    #endregion

    private MeshRenderer mesh;

    #region private
    private void Awake() => mesh = GetComponent<MeshRenderer>();

    private void OnCollisionEnter(Collision other) { // remove toyball if collision bug happens
        if(Utility.checkoutLayer(platformLayerMask, other.gameObject.layer)) remove();
    }
    #endregion

    #region public
    public void initialize(Color color){
        this.color = color;

        mesh.material.color = color;

        if (initializePS != null) {
            GameObject pickupPS = Instantiate(initializePS, transform.position, Quaternion.identity);
            Array.ForEach(pickupPS.GetComponentsInChildren<ParticleSystem>(), ps => {
                ParticleSystem.MainModule main = ps.main;
                main.startColor = color;
            });
            Destroy(pickupPS, 2f);
        }
    }

    public void remove(){
        if(destroyPS != null) Destroy(Instantiate(destroyPS, transform.position, destroyPS.transform.rotation), 2f);

        Destroy(gameObject);
    }
    #endregion
}
