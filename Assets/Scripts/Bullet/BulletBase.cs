using Utils;
using System;
using UnityEngine;
using DG.Tweening;
using PathCreation;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
public abstract class BulletBase : PoolObject {
    public enum FollowType {Static, IncreaseSpeed, DecreaseSpeed};
    public enum FollowStepType {Lerp, Smooth, MoveTowards, Curve};

    public class FollowSettings {
        public VertexPath path;
        public float speed;
        public FollowType type;
        public FollowStepType stepType;
        public float delta;
        public AnimationCurve curve;
    }

    #region editor
    [Range(0f, 1f)][SerializeField] private float startSize = .25f;
    [SerializeField] protected CrumbleSpawner crumbleSpawner = default;
    [SerializeField] private LayerMask platformLayerMask = default;
    [SerializeField] private GameObject toyPickupPS = default;
    [SerializeField] protected RandomizedFloat forceSpeed = 10f;
    [SerializeField] private Blob blobPrefab = default;
    [SerializeField] protected GameObject platformTriggerPS = default;
    #endregion

    #region public properties
    [HideInInspector] public Color color = Color.white;
    [HideInInspector] public FollowSettings followSettings;
    #endregion

    protected bool pickedUp = false;
    protected bool calculateCollision = true;
    private Vector3 cachedScale = Vector3.one;
    
    protected Rigidbody rb;
    protected MeshRenderer mesh;
    protected Coroutine followCoroutine;
    protected Sequence returnToPoolSequence;
    
    #region private
    private void OnCollisionEnter(Collision other) {
        if(calculateCollision && Utility.checkoutLayer(platformLayerMask, other.gameObject.layer)){
            stopCalculateCollision();

            ContactPoint contact = other.GetContact(0);

            // blob sprite
            Vector3 blobPosition = new Vector3(contact.point.x, .5f, contact.point.z);
            Blob blob = Instantiate(blobPrefab, blobPosition, Quaternion.identity);
            blob.setColor(color);

            onPlatformTrigger(other);
        }
    }

    private IEnumerator _follow() {
        if (followSettings.path == null) yield break;

        float startSpeed = followSettings.speed;
        float stopSpeed = followSettings.speed;
        float targetSpeed = followSettings.speed - (followSettings.speed * followSettings.delta);

        if (followSettings.type == FollowType.IncreaseSpeed) startSpeed = targetSpeed;
        else if (followSettings.type == FollowType.DecreaseSpeed) stopSpeed = targetSpeed;

        float distance = 0f;
        float delta = startSpeed;
        while (distance < followSettings.path.length) {
            distance += delta * Time.fixedDeltaTime;
            rb.MovePosition(followSettings.path.GetPointAtDistance(distance, EndOfPathInstruction.Stop));
            rb.MoveRotation(followSettings.path.GetRotationAtDistance(distance, EndOfPathInstruction.Stop) * Quaternion.AngleAxis(-90, Vector3.up));

            float deltaStep = Mathf.InverseLerp(0f, followSettings.path.length, distance);
            if(followSettings.stepType == FollowStepType.Lerp) delta = Mathf.Lerp(startSpeed, stopSpeed, deltaStep);
            else if(followSettings.stepType == FollowStepType.Smooth) delta = Mathf.SmoothStep(startSpeed, stopSpeed, deltaStep);
            else if(followSettings.stepType == FollowStepType.MoveTowards) delta = Mathf.MoveTowards(startSpeed, stopSpeed, deltaStep);
            else if (followSettings.stepType == FollowStepType.Curve) delta = followSettings.speed * followSettings.curve.Evaluate(deltaStep);

            // clamp only if not curve
            if (followSettings.stepType != FollowStepType.Curve) delta = Mathf.Clamp(delta, .02f, followSettings.speed);

            yield return new WaitForFixedUpdate();
        }
    }

    private void OnDestroy() => returnToPoolSequence?.Kill();
    #endregion

    #region protected
    protected virtual void Awake() {
        rb = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshRenderer>();

        followSettings = new FollowSettings();

        returnToPoolSequence = DOTween.Sequence();
        returnToPoolSequence.SetAutoKill(false);
        returnToPoolSequence.AppendInterval(3f);
        returnToPoolSequence.OnComplete(returnToPool);
        returnToPoolSequence.Pause();

        cachedScale = transform.localScale * UnityEngine.Random.Range(.75f, 1.25f);
    }

    protected void stopCalculateCollision(){
        calculateCollision = false;
        if(followCoroutine != null) StopCoroutine(followCoroutine);
    }

    protected void explodeCrumble(){
        CrumbleSpawner crumble = Instantiate(crumbleSpawner, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);
        crumble.color = color;
        crumble.explode();

        Destroy(crumble.gameObject, 1f);
    }

    protected virtual void onPlatformTrigger(Collision other) {
        if (platformTriggerPS != null) Destroy(Instantiate(platformTriggerPS, transform.position, platformTriggerPS.transform.rotation), 2f);
    }
    #endregion

    #region public
    public virtual bool pickup(Toy toy) {
        if(!calculateCollision || pickedUp) return false;

        pickedUp = true;
        stopCalculateCollision();
        returnToPoolSequence.Rewind();

        if (toyPickupPS != null) Destroy(Instantiate(toyPickupPS, transform.position, Quaternion.identity), 2f);

        return pickedUp;
    }

    public virtual void initialize(Color color) {
        transform.localScale = cachedScale * startSize;
        transform.DOScale(cachedScale, .75f);

        this.color = color;
    }

    public void follow() {
        gameObject.SetActive(true);

        followCoroutine = StartCoroutine(_follow());
    }   
    #endregion

    #region PoolObject
    public override void reuse(){
        pickedUp = false;
        calculateCollision = true;
        rb.velocity = Vector3.zero;
        transform.localScale = cachedScale;
        returnToPoolSequence.Rewind();
    }
    #endregion
}