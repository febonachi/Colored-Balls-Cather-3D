using UnityEngine;

public class ToyBulletTrigger : MonoBehaviour {
    private Toy toy;

    #region private
    private void Awake() => toy = GetComponentInParent<Toy>();

    private void OnTriggerEnter(Collider other) => other.GetComponent<BulletBase>()?.pickup(toy);
    #endregion
}
