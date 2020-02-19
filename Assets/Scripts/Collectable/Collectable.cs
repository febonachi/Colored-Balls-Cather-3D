using UnityEngine;

public abstract class Collectable : PoolObject {
    #region public
    public virtual void trigger() => returnToPool();
    #endregion
}
