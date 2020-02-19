using UnityEngine;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour {
    #region singleton
    public static PoolManager instance;
    #endregion

    private Dictionary<string, Pool> pools = new Dictionary<string, Pool>();

    #region private
    private void Awake() {
        if(instance == null) instance = this;
        else Destroy(gameObject);
    }
    #endregion

    #region public
    public void addPool(Pool pool){
        string id = pool.poolID;
        if(!pools.ContainsKey(id)) pools.Add(id, pool);
        else Debug.LogWarning($"Pool:{id} is already created");
    }

    public Pool getPool(string id) {
        if(pools.ContainsKey(id)) return pools[id];
        return null;
    }

    public bool canGetPoolObject(string pool){
        if(!pools.ContainsKey(pool)) {
            Debug.LogWarning($"Can't find pool with name: {pool}");
            return false;
        }

        return pools[pool].canGetObject;
    }

    public GameObject getPoolObject(string pool, Vector3 position, Quaternion rotation){
        if(!pools.ContainsKey(pool)) {
            Debug.LogWarning($"Can't find pool with key: {pool}");
            return null;
        }

        return pools[pool].getObject(position, rotation);
    }

    public void returnPoolObject(string pool, GameObject poolObject) {
        if (pools.ContainsKey(pool)) pools[pool].returnObject(poolObject);
    }
    #endregion
}
