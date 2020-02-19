using System;
using Utils;
using UnityEngine;
using DG.Tweening;

public class GunCannon : MonoBehaviour {
    [Serializable] public struct CannonSettings {
        public float speedDelta;
        public RandomizedFloat speed;
        public RandomizedFloat shootDelay;
        public RandomizedFloat bombSpawnChance;
    }

    #region editor
    [Header("Bullets Settings")]
    [SerializeField] private string bombsPoolID = default;
    [SerializeField] private string bulletsPoolID = default;
    [SerializeField] private Transform spawner = default;
    [SerializeField] private BulletBase.FollowType followType = BulletBase.FollowType.IncreaseSpeed;
    [SerializeField] private BulletBase.FollowStepType followStepType = BulletBase.FollowStepType.Smooth;
    [SerializeField] private AnimationCurve followCurve = default;

    [Space]
    [Header("Cannon Settings")]
    [SerializeField] private Transform cannon = default;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float punchStrength = 1f;
    [SerializeField] private float punchDuration = 1f;

    [Space]
    [Header("Effects Settings")]
    [SerializeField] private ParticleSystem explosionPS = default;
    #endregion

    #region public properties
    public Transform spawnPoint => spawner;
    public Color nextColor => nextBulletColor;
    public Color nextRandomColor => colorScheme.randomBulletColor;

    [HideInInspector] public CannonSettings cannonSettings;
    #endregion

    private float cachedHeight = 0f;
    private BulletBase nextBullet;
    private Color nextBulletColor = Color.white;

    private ColorScheme colorScheme;
    private PoolManager poolManager;

    #region private
    private void Awake() {
        cachedHeight = transform.localPosition.y;
        transform.localPosition = new Vector3(transform.localPosition.x, -cachedHeight, transform.localEulerAngles.z);
    }

    private void Start() {
        colorScheme = ColorScheme.instance;
        poolManager = PoolManager.instance;

        // first bullet must be color bullet
        GameObject bulletFromPool = poolManager.getPool(bulletsPoolID).getObject(spawnPoint.position, Quaternion.identity, makeVisible: false);
        nextBullet = bulletFromPool?.GetComponent<BulletBase>();
        nextBulletColor = colorScheme.randomBulletColor;
    }

    private BulletBase randomBullet() {
        Pool pool = null;
        float bombChance = cannonSettings.bombSpawnChance.randomizedValue;
        if (UnityEngine.Random.value < bombChance) { // bombs
            pool = poolManager.getPool(bombsPoolID);
            nextBulletColor = colorScheme.randomBombBulletColor;
        } else { // color bullets
            pool = poolManager.getPool(bulletsPoolID);
            nextBulletColor = colorScheme.randomBulletColor;
        }
        GameObject bulletFromPool = pool.getObject(spawnPoint.position, Quaternion.identity, makeVisible: false);
        return bulletFromPool?.GetComponent<BulletBase>();
    }
    #endregion

    #region public
    public void lookAt(Vector3 point, bool lerp = true) {
        Vector3 cannonDirection = point - transform.position;
        Quaternion cannonRotation = Quaternion.LookRotation(cannonDirection.normalized);
        if (lerp) transform.rotation = Quaternion.Slerp(transform.rotation, cannonRotation, rotationSpeed * Time.deltaTime);
        else transform.rotation = cannonRotation;
    }

    public void moveUpFromPlatform() {
        if(!gameObject.activeSelf) gameObject.SetActive(true);
        transform.DOLocalMoveY(cachedHeight, .5f);
    }

    public void moveDownToPlatform() => transform.DOLocalMoveY(-cachedHeight, .5f);

    public float shoot(PathController path) {
        if (nextBullet == null) {
            nextBullet = randomBullet();
            return cannonSettings.shootDelay.randomizedValue;
        }
         
        BulletBase bullet = nextBullet;
        bullet.followSettings.path = path.direction.path;
        bullet.followSettings.speed = cannonSettings.speed.randomizedValue;
        bullet.followSettings.type = followType;
        bullet.followSettings.stepType = followStepType;
        bullet.followSettings.delta = cannonSettings.speedDelta;
        bullet.followSettings.curve = followCurve;
        bullet.initialize(nextBulletColor);
        bullet.follow();

        nextBullet = randomBullet();

        cannon.DOScaleY(cannon.localScale.y / 2f, punchDuration / 2f).From();
        transform.DOPunchPosition(-transform.forward * punchStrength, punchDuration, 5);

        explosionPS?.Play();

        return cannonSettings.shootDelay.randomizedValue;
    }
    #endregion
}
