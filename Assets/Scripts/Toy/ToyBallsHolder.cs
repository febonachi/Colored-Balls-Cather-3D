using Utils;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshCollider))]
public class ToyBallsHolder : MonoBehaviour {
    #region editor
    [SerializeField] private ToyBall ballPrefab = default;
    [Tooltip("Need to collect <count> balls in a row to rich fever effect")]
    [SerializeField] private int feverEffectBallsCount = 5;
    [SerializeField] private SphereCollider spawnZone = default;
    #endregion

    #region properties
    public bool ballsFever => collectedBallsInARow == feverEffectBallsCount;
    public int ballsCatchedInARow => collectedBallsInARow;
    public int ballsCount => collectedBalls.Count;
    #endregion

    private int collectedBallsInARow = 0;

    private Vector3 offset;

    private Toy toy;
    private Rigidbody rb;
    private MeshCollider meshCollider;

    private List<ToyBall> collectedBalls = new List<ToyBall>();

    #region private
    private void Awake() {
        toy = GetComponentInParent<Toy>();

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        spawnZone.enabled = false;

        meshCollider = GetComponent<MeshCollider>();

        offset = transform.position - toy.transform.position;
    }

    private void FixedUpdate() => rb.MovePosition(toy.transform.position + offset);

    private void addBall(Color color){
        if(ballPrefab == null) return;

        Vector3 randomSpawnPosition = meshCollider.bounds.center + (Utility.randomHorizontalDirection() * Random.Range(0f, spawnZone.radius));
        ToyBall ball = Instantiate(ballPrefab, randomSpawnPosition, Quaternion.identity);
        collectedBalls.Add(ball);
        ball.initialize(color);
    }
    #endregion

    #region public
    public void addBalls(Color color, int count = 1){
        collectedBallsInARow += count;
        for(int i = 0; i < count; i++) addBall(color);
    }

    public void removeBalls(int count = 1){
        collectedBallsInARow = 0;
        collectedBalls.RemoveAll(ball => ball == null);
        foreach(ToyBall ball in collectedBalls.Take(count).ToArray()){
            collectedBalls.Remove(ball);
            ball.remove();
        }
    }

    public void removeAllBalls() => removeBalls(ballsCount);
    #endregion
}
