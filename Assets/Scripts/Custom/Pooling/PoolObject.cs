using UnityEngine;

public abstract class PoolObject : MonoBehaviour {
    #region public properties
    public string poolID {
        get => pool.poolID;
        set => pool = PoolManager.instance.getPool(value);
    }
    #endregion

    private Pool pool;

    #region public
    public abstract void reuse();
    public void returnToPool() {
        if (pool.objectIsInThePool(gameObject)) return;

        pool?.returnObject(gameObject);
    }
    #endregion
}
