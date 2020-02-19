using UnityEngine;
using DG.Tweening;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class ToyBridge : MonoBehaviour {
    #region editor
    [SerializeField] private string poolID = "pool";
    #endregion

    private float cachedScaleZ = 1f;

    private BoxCollider box;

    private Pool pool;

    #region private
    private void OnEnable() {
        cachedScaleZ = transform.localScale.z;

        box = GetComponent<BoxCollider>();

        pool = PoolManager.instance.getPool(poolID);
    }

    private IEnumerator _spawnCollectables(){        
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 0f);

        int lines = ((int)cachedScaleZ / 10) * 2;
        int lineStep = (int)(cachedScaleZ / lines);

        Bounds bounds = box.bounds;

        Tweener scaleTween = transform.DOScaleZ(cachedScaleZ, 1f);

        for(int i = 1; i < lines; i++){
            float xOffset = box.size.x / (i % 2 == 0 ? 4 : 8);
            float zOffset = i * lineStep;

            Vector3 leftPosition = transform.TransformDirection(new Vector3(-xOffset, bounds.extents.y, zOffset));
            Vector3 rightPosition = transform.TransformDirection(new Vector3(xOffset, bounds.extents.y, zOffset));

            pool.getObject(transform.position + leftPosition, Quaternion.identity);
            pool.getObject(transform.position + rightPosition, Quaternion.identity);

            yield return null;
        }

        yield return scaleTween.WaitForCompletion();
    }
    #endregion

    #region public
    public void openBridge() {
        gameObject.SetActive(true);

        StartCoroutine(_spawnCollectables());
    }

    public void closeBridge() => transform.DOScaleZ(0, 2f).OnComplete(() => gameObject.SetActive(false));

    #endregion
}
