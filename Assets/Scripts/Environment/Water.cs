using Utils;
using System.Linq;
using UnityEngine;

namespace Utils {
    [RequireComponent(typeof(WaterPropertyBlockSetter))]
    public class Water : MonoBehaviour {
        #region editor
        [SerializeField] private GameObject splashPS = default;
        [SerializeField] private int blocksCount = 10;
        [SerializeField] private MinMaxValue blocksHeightOffset = default;
        [SerializeField] private MinMaxValue blocksXRotation = default;
        [SerializeField] private MinMaxValue blocksYRotation = default;
        [SerializeField] private MinMaxValue blocksZRotation = default;
        [SerializeField] private MinMaxValue blocksScaleDelta = default;
        [SerializeField] private Transform blocksHolder = default;
        [SerializeField] private BoxCollider blocksBoxBounds = default;
        [SerializeField] private GameObject[] blocksPrefab = default;
        #endregion

        #region private
        private void OnTriggerEnter(Collider other) {
            if(splashPS != null && Utility.maybe) {
                GameObject splash = Instantiate(splashPS);
                splash.transform.position = new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z);
                Destroy(splash, 2f);
            }
            
            if(other.GetComponent<PoolObject>() != null) other.GetComponent<PoolObject>().returnToPool();
            else Destroy(other.gameObject);
        }
        #endregion

        #region public 
        public void generateBlocks(){
            clearBlocks();

            WaterPropertyBlockSetter waterProperty = GetComponent<WaterPropertyBlockSetter>();
            Bounds bounds = blocksBoxBounds.bounds;
            for(int i = 0; i < blocksCount; i++){
                GameObject blockPrefab = blocksPrefab[Random.Range(0, blocksPrefab.Length)];

                Vector3 position = new Vector3(Random.Range(bounds.min.x, bounds.max.x), waterProperty.waterHeight + (blocksHeightOffset.random), Random.Range(bounds.min.z, bounds.max.z));
                Quaternion rotation = Quaternion.AngleAxis(-90f, Vector3.right) * Quaternion.Euler(blocksXRotation.random, blocksYRotation.random, blocksZRotation.random);
                GameObject block = Instantiate(blockPrefab, position, rotation, blocksHolder);
                block.transform.localScale *= blocksScaleDelta.random;
            }

            waterProperty.findBlocks();
        }

        public void clearBlocks() {
            blocksHolder.Cast<Transform>().ToList().ForEach(t => DestroyImmediate(t.gameObject));
            GetComponent<WaterPropertyBlockSetter>().findBlocks();
        }
        #endregion
    }
}