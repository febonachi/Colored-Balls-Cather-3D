using System;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using System.Collections;

using static UnityEngine.Random;

public class CrumbleSpawner : MonoBehaviour {
    #region editor
    public Color color = Color.white;
    [SerializeField] private string poolID = default;
    [SerializeField] private GameObject[] prefabs = default;
    [SerializeField] private LayerMask overlapMask = default;
    [SerializeField] private bool randomizeRotation = false;
    public Vector3Int dimension = new Vector3Int(2, 2, 2);
    [SerializeField] private Vector3 scale = new Vector3(.25f, .25f, .25f);
    [SerializeField] private float spacing = .025f;
    [SerializeField] private float explosionForce = 35f;
    [SerializeField] private float explosionUpForce = .1f;
    [SerializeField] private float explosionRadius = 1f;
    [SerializeField] private ForceMode explosionForceMode = ForceMode.Force;
    #endregion

    private bool initialized = false;
    private Vector3 pivotOffset = Vector3.zero;
    private PoolManager poolManager = default;

    #region private    
    private void instantiatePrefabs() {
        for (int i = 0; i < dimension.x; i++) {
            for (int j = 0; j < dimension.y; j++) {
                for (int k = 0; k < dimension.z; k++) {
                    instantiatePrefab(i, j, k);
                }
            }
        }
    }

    private void instantiatePrefab(int i, int j, int k) {
        GameObject crumbleGameObject = null;

        Quaternion rotation = randomizeRotation ? UnityEngine.Random.rotation : Quaternion.identity;

        if(poolID != ""){
            Pool crumblesPool = poolManager.getPool(poolID);
            crumbleGameObject = crumblesPool.getObject(transform.position, rotation);
            CrumbleObject crumbleObject = crumbleGameObject.GetComponent<CrumbleObject>();
            crumbleObject.initialize(scale, color);
        } else {
            crumbleGameObject = Instantiate(prefabs[Range(0, prefabs.Length)], transform.position, rotation);
            crumbleGameObject.transform.localScale = new Vector3(crumbleGameObject.transform.localScale.x * scale.x, 
                                                                 crumbleGameObject.transform.localScale.y * scale.y,
                                                                 crumbleGameObject.transform.localScale.z * scale.z);
        }

        crumbleGameObject.transform.position = transform.position + new Vector3((i * (scale.x + spacing)), 
                                                                                (j * (scale.y + spacing)), 
                                                                                (k * (scale.z + spacing))) - pivotOffset;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);

        Vector3 dimensionOffset = new Vector3((dimension.x * (scale.x + spacing)) / 2f,
                                              (dimension.y * (scale.y + spacing)) / 2f,
                                              (dimension.z * (scale.z + spacing)) / 2f);
        Vector3 scaleOffset = new Vector3(scale.x + spacing, scale.y + spacing, scale.z + spacing) / 2f;
        pivotOffset = dimensionOffset - scaleOffset;

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(dimension.x * (scale.x + spacing),
                                                            dimension.y * (scale.y + spacing),
                                                            dimension.z * (scale.z + spacing)));

        Color yellowColor = Color.yellow;
        yellowColor.a = .5f;
        Gizmos.color = yellowColor;
        for (int i = 0; i < dimension.x; i++) {
            for (int j = 0; j < dimension.y; j++) {
                for (int k = 0; k < dimension.z; k++) {
                    Gizmos.DrawWireCube(transform.position + new Vector3((i * (scale.x + spacing)), 
                                                                         (j * (scale.y + spacing)), 
                                                                         (k * (scale.z + spacing))) - pivotOffset, scale);
                }
            }
        }                                                           
    }

    private IEnumerator _explode() {
        if(!initialized) initialize();

        yield return new WaitUntil(() => initialized == true);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, overlapMask);
        Array.ForEach(colliders, collider => {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if(rb != null){
                rb.isKinematic = false;
                float randomExplosionRadius = Range(explosionRadius * .75f, explosionRadius);
                float randomExplosionForce = Range(explosionForce * .75f, explosionForce);
                float randomExplosionUpForce = Range(explosionUpForce * .75f, explosionUpForce);
                rb.AddExplosionForce(randomExplosionForce, transform.position, randomExplosionRadius, randomExplosionUpForce, explosionForceMode);
            }
        });
    }
    #endregion

    #region public
    public void initialize() {
        clear();
        Vector3 dimensionOffset = new Vector3((dimension.x * (scale.x + spacing)) / 2f,
                                              (dimension.y * (scale.y + spacing)) / 2f,
                                              (dimension.z * (scale.z + spacing)) / 2f);
        Vector3 scaleOffset = new Vector3(scale.x + spacing, scale.y + spacing, scale.z + spacing) / 2f;
        pivotOffset = dimensionOffset - scaleOffset;

        poolManager = PoolManager.instance;
        instantiatePrefabs();

        initialized = true;
    }

    public void explode() => StartCoroutine(_explode());

    public void activate() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, overlapMask);
        Array.ForEach(colliders, collider => {
            Rigidbody crumbleObject = collider.GetComponent<Rigidbody>();
            if(crumbleObject != null) crumbleObject.isKinematic = false;
        });
    }

    public void addForce(Vector3 force, ForceMode forceMode = ForceMode.Impulse) {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, overlapMask);
        Array.ForEach(colliders, collider => {
            Rigidbody crumbleObject = collider.GetComponent<Rigidbody>();
            if(crumbleObject != null){
                crumbleObject.isKinematic = false;
                Vector3 randomForce = force * Range(.5f, 1f);
                crumbleObject.AddForce(randomForce, forceMode);
            }
        });
    }

    public void clear() {
        transform.Cast<Transform>().ToList().ForEach(t => {
            if (Application.isPlaying) Destroy(t.gameObject);
            else DestroyImmediate(t.gameObject);
        });

        initialized = false;
    }
    #endregion
}
