using System;
using UnityEngine;
using DG.Tweening;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Toy : MonoBehaviour {
    #region editor
    [Header("Toy Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float scaleFactor = 2f;
    [SerializeField] private float scaleDuration = 2f;
    [SerializeField] private MeshRenderer floorMesh = default;
    [SerializeField] private ToyBallsHolder ballsHolder = default;
    [SerializeField] private ToyCrumblePicker crumblePicker = default;
    [SerializeField] private CrumbleSpawner crumbleSpawner = default;
    [SerializeField] private ParticleSystem trailPS = default;

    [Space]
    [Header("Score settings")]
    [SerializeField] private int bombPrice = 10;
    [SerializeField] private int bulletPrice = 25;
    [SerializeField] private int crumblePrice = 1;
    #endregion

    #region public events
    public Action brokeDown;
    public Action<int, int, int, bool> scoreUpdated; // totalScore, scoreValue, ballsInARow, fever
    #endregion

    #region public properties
    [HideInInspector] public int score = 0;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public ToyPlatform platform;
    #endregion

    private float forward = 0f;
    private float horizontal = 0f;    
    private bool mousePressed = false;
    private bool scaledUp = false;
    private bool available = true; // prevent situation when stage completed but one more bullet trigger with toy
    private Vector3 cachedScale = Vector3.one;

    private Rigidbody rb;

    private CameraManager cameraManager;
    private GameController gameController;

    #region private
    private void Awake() {
        rb = GetComponent<Rigidbody>();

        gameController = GameController.instance;
        cameraManager = gameController.cameraManager;

        cachedScale = transform.localScale;

        crumblePicker.crumblePicked += onCrumblePicked;
    }

    private void Start() {
        // trail ps settings
        Color[] validBulletColors = ColorScheme.instance.bulletColors;
        ParticleSystem.MainModule trailPSMainModule = trailPS.main;
        Gradient trailPsColor = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[validBulletColors.Length];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[validBulletColors.Length];
        for (int i = 0; i < validBulletColors.Length; i++) {
            float time = i * (1f / validBulletColors.Length);
            colorKeys[i].color = validBulletColors[i];
            colorKeys[i].time = time;
            
            alphaKeys[i].alpha = 1f;
            alphaKeys[i].time = time;

        }
        trailPsColor.SetKeys(colorKeys, alphaKeys);
        ParticleSystem.MinMaxGradient minMaxGradiend = new ParticleSystem.MinMaxGradient(trailPsColor);
        minMaxGradiend.mode = ParticleSystemGradientMode.RandomColor;
        trailPSMainModule.startColor = minMaxGradiend;
    }

    private void Update() {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) mousePressed = true;
        if (Input.GetMouseButtonUp(0)) mousePressed = false;
        if (mousePressed) {
            forward = moveSpeed * Input.GetAxisRaw("Mouse Y");
            horizontal = moveSpeed * Input.GetAxisRaw("Mouse X");
        }
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) mousePressed = true;
            else if (touch.phase == TouchPhase.Ended) mousePressed = false;
            else if (touch.phase == TouchPhase.Moved) {
                forward = 10f * touch.deltaPosition.y;
                horizontal = 10f * touch.deltaPosition.x;
            }
        }
#endif
    }

    private void FixedUpdate() {
        if(mousePressed && canMove) {
// #if UNITY_EDITOR
//             const float maxSpeed = 50f;
// #elif UNITY_ANDROID || UNITY_IOS
//             const float maxSpeed = 10f;
// #endif
//             forward = Mathf.Clamp(forward, -maxSpeed, maxSpeed);
//             horizontal = Mathf.Clamp(horizontal, -maxSpeed, maxSpeed);

            Vector3 velocity = new Vector3(horizontal * Time.deltaTime, 0f, forward * Time.deltaTime);
            rb.AddForce(velocity, ForceMode.VelocityChange);
        } else rb.velocity = Vector3.zero;
    }

    private void scaleUp() {
        if(!canMove || scaledUp) return;

        trailPS.transform.localScale *= scaleFactor;

        canMove = false;
        scaledUp = true;

        crumblePicker.scaleUp(scaleFactor);
        Vector3 scale = new Vector3(cachedScale.x * scaleFactor, cachedScale.y, cachedScale.z * scaleFactor);
        transform.DOScale(scale, scaleDuration).OnComplete(onScaleFinished);
    }

    private void scaleDown(){
        if(!canMove || !scaledUp) return;

        trailPS.transform.localScale /= scaleFactor;

        canMove = false;
        scaledUp = false;

        crumblePicker.scaleDown();
        transform.DOScale(cachedScale, scaleDuration).OnComplete(onScaleFinished);
    }

    private void addScore(int value){
        score += value;
        if (score < 0) score = 0;
        
        scoreUpdated?.Invoke(score, value, ballsHolder.ballsCatchedInARow, ballsHolder.ballsFever);
    }

    private void onScaleFinished() => canMove = true;

    private void onCrumblePicked() => addScore(crumblePrice);

    private void OnDestroy() => crumblePicker.crumblePicked -= onCrumblePicked;
    #endregion

    #region public
    public IEnumerator _reset() {
        available = false;
        transform.DOKill();

        score = 0;
        canMove = false;
        scaledUp = false;
        crumblePicker.scaleDown();
        ballsHolder.removeAllBalls();
        trailPS.transform.localScale = Vector3.one;

        yield return transform.DOScale(cachedScale, scaleDuration).WaitForCompletion();

        available = true;
    }

    public void colorBulletBehavior(Color color) {
        if(!available) return;

        trailPS.Play();

        Color floorColor = color;
        // if (floorMesh.material.color == color) floorColor = Color.white;
        floorMesh.material.DOColor(floorColor, 1f); // change floor color
        //platform.setFloorColor(color);

        ballsHolder.addBalls(color, 1);

        if(ballsHolder.ballsFever) scaleUp(); // fever effect

        addScore(bulletPrice); // update UI
    }

    public void bombBulletBehavior() {
        if(!available) return;

        addScore(-bombPrice); // update UI

        gameController.vibrate();

        // forgiving system
        if (ballsHolder.ballsCount > 0) {
            cameraManager.shakeOnce(3f, 3f);

            ballsHolder.removeAllBalls();
        } else if (scaledUp) {
            cameraManager.shakeOnce(3f, 3f);

            scaleDown();
        } else {
            cameraManager.shakeOnce(25f, 1f, true);

            gameObject.SetActive(false);

            CrumbleSpawner spawner = Instantiate(crumbleSpawner, new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), Quaternion.identity);
            spawner.explode();

            brokeDown?.Invoke();
        }
    }

    public IEnumerator _moveTowards(Vector3 point, float speed){
        rb.isKinematic = true;
        point.y = rb.position.y; // move only along X and Z axis
        while (rb.position != point) {
            transform.position = Vector3.MoveTowards(transform.position, point, speed * Time.deltaTime);
            yield return null;
        }
        rb.isKinematic = false;
    }
    #endregion
}
