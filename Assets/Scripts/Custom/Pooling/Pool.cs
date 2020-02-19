using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class Pool : MonoBehaviour {
    #region editor
    [SerializeField] private string id = "pool";
    [SerializeField] private int maxCount = 1;
    [SerializeField] private GameObject prefab = default;
    [SerializeField] private bool extendable = true;
    [SerializeField] private int readonlyCount = 0; // readonly
    #endregion    

    #region public properties
    public string poolID => id;
    public bool canGetObject => (poolObjects.Count > 0 || extendable);
    #endregion

    private Queue<GameObject> poolObjects = new Queue<GameObject>();

    #region private
    public void Awake() {
        addObjects(maxCount);

        PoolManager.instance.addPool(this); // script execution order must be (PoolManager -> Pool)
    }

    private void addObjects(int count){
        for(int i = 0; i < count; i++){
            GameObject poolObject = Instantiate(prefab, transform);
            poolObject.SetActive(false);

            poolObjects.Enqueue(poolObject);
        }

        readonlyCount += count;
    }
    #endregion

    #region public 
    public GameObject getObject(Vector3 position, Quaternion rotation, bool makeVisible = true){
        if(poolObjects.Count == 0){
            if(extendable) addObjects(1);
            else return null;
        }

        GameObject poolObject = poolObjects.Dequeue();

        poolObject.transform.position = position;
        poolObject.transform.rotation = rotation;
        poolObject.SetActive(makeVisible);

        PoolObject poolObjectInterface = poolObject.GetComponent<PoolObject>();
        if(poolObjectInterface != null) {
            poolObjectInterface.poolID = id;
            poolObjectInterface.reuse();
        }

        readonlyCount--;

        return poolObject;
    }

    public void returnObject(GameObject poolObject) {
        poolObject.SetActive(false);
        poolObjects.Enqueue(poolObject);

        readonlyCount++;
    }

    public bool objectIsInThePool(GameObject poolObject) => poolObjects.Any(po => po.GetInstanceID() == poolObject.GetInstanceID());
    #endregion
}
