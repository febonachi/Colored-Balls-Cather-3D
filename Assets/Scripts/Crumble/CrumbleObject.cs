using System;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
public class CrumbleObject : PoolObject {
    #region editor
    [SerializeField] private ParticleSystem triggerPS = default;
    #endregion

    #region public properties
    [HideInInspector] public Rigidbody rb;
    public bool activated { get; private set; } = false;
    #endregion


    private float elapsed = 0f;
    private Sequence hideTweener;

    private MeshRenderer mesh;
    private Collider triggerCollider;

    #region private
    private void Awake() {
        tag = nameof(CrumbleObject);

        rb = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshRenderer>();
        triggerCollider = GetComponent<Collider>();


        // move crumbles up to the sky =)
        float hideTweenerDelay = 5f;
        hideTweener = DOTween.Sequence();
        hideTweener.SetAutoKill(false);
        hideTweener.AppendInterval(hideTweenerDelay);
        hideTweener.OnComplete(() => {
            if(transform.position.y >= 0f){
                float randomScaleDuration = UnityEngine.Random.Range(1f, 3f); 
                float randomUpwardsSpeed = UnityEngine.Random.Range(10f, 30f);

                rb.isKinematic = true;
                triggerCollider.enabled = false;
                transform.DOScale(.2f, randomScaleDuration).OnUpdate(() => {
                    transform.Translate(Vector3.up * randomUpwardsSpeed * Time.deltaTime, Space.World);
                }).OnComplete(returnToPool);
            }
        });
        hideTweener.Pause();
    }

    private void OnDestroy() => hideTweener?.Kill();
    #endregion

    #region public
    public async void initialize(Vector3 scale, Color color) {        
        ParticleSystem.MainModule mainModule = triggerPS.main;
        mainModule.startColor = color;
        mesh.material.color = color;

        transform.localScale = scale;

        transform.DOPunchScale(scale * .75f, 1f, 5);

        await Task.Delay(TimeSpan.FromSeconds(.5f));

        hideTweener.Play();

        activated = true;
    }

    public void moveTowards(Transform target, float force){
        if(!activated) return;
        
        Vector3 direction = (target.position - transform.position).normalized;
        rb.AddForce(direction * force);
    }

    public async void trigger() {
        if(!activated) return;
        activated = false;
        
        hideTweener.Rewind();

        mesh.enabled = false;
        rb.isKinematic = true;
        triggerCollider.enabled = false;

        triggerPS.Play(); // vfx
        
        await Task.Delay(TimeSpan.FromSeconds(triggerPS.main.startLifetime.constantMax));
        
        returnToPool();
    }
    #endregion

    #region PoolObject
    public override void reuse() {
        elapsed = 0f;
        activated = false;
        mesh.enabled = true;
        rb.isKinematic = true;
        triggerCollider.enabled = true;
        hideTweener.Rewind();
    }
    #endregion
}
