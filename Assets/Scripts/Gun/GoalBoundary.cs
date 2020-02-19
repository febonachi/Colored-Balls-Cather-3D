using UnityEngine;

public class GoalBoundary : MonoBehaviour {
    private abstract class BoundaryStrategy {
        protected Collider collider;

        public BoundaryStrategy(Collider collider) {
            this.collider = collider;
            this.collider.isTrigger = true;
        }

        public abstract Vector3 position(Collider collider);
    }

    private class BoxBoundaryStrategy : BoundaryStrategy {
        private BoxCollider box;

        public BoxBoundaryStrategy(Collider collider) : base(collider) => box = collider as BoxCollider;

        public override Vector3 position(Collider collider){
            if(box == null) return Vector3.zero;
            Bounds bounds = box.bounds;
            return new Vector3(Random.Range(bounds.min.x, bounds.max.x), 
                               Random.Range(bounds.min.y, bounds.max.y), 
                               Random.Range(bounds.min.z, bounds.max.z));
        }
    }

    private class SphereBoundaryStratery : BoundaryStrategy {
        private SphereCollider sphere;

        public SphereBoundaryStratery(Collider collider) : base(collider) => sphere = collider as SphereCollider;

        public override Vector3 position(Collider collider){
            if(sphere == null) return Vector3.zero;
            Bounds bounds = sphere.bounds;
            return bounds.center + (Random.insideUnitSphere * sphere.radius);
        }
    }

    #region editor
    [SerializeField] private Collider boundaryCollider = default;
    #endregion

    private BoundaryStrategy strategy;

    #region private
    private void Awake() {
        if(boundaryCollider == null) return;

        if(boundaryCollider is BoxCollider) strategy = new BoxBoundaryStrategy(boundaryCollider);
        else if(boundaryCollider is SphereCollider) strategy = new SphereBoundaryStratery(boundaryCollider);
    }
    #endregion

    #region public
    public Vector3 randomPosition() => strategy?.position(boundaryCollider) ?? Vector3.zero;
    #endregion
}
