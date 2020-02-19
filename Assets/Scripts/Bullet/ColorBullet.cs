using UnityEngine;
using DG.Tweening;

public class ColorBullet : BulletBase {
    #region editor
    #endregion

    #region private
    private new void Awake() {
        base.Awake();

        tag = nameof(ColorBullet);
    }
    #endregion

    #region protected
    protected override void onPlatformTrigger(Collision other){
        base.onPlatformTrigger(other);

        ContactPoint point = other.GetContact(0);

        explodeCrumble();
        
        transform.DOScale(.85f, .075f).SetLoops(2, LoopType.Yoyo);
        Vector3 directionToSpawner = point.point - followSettings.path.GetPointAtTime(0f);
        Vector3 ricochetDirection = Vector3.Reflect(directionToSpawner, Vector3.up).normalized;
        rb.AddForce(ricochetDirection * forceSpeed.randomizedValue, ForceMode.VelocityChange);

        returnToPoolSequence.Play();
    }
    #endregion

    #region public
    public override void initialize(Color color) {
        base.initialize(color);
        
        mesh.material.color = color;
    }

    public override bool pickup(Toy toy){
        if(!base.pickup(toy)) return false;
        
        toy.colorBulletBehavior(color); // toy interaction

        returnToPool();

        return true;
    }
    #endregion
}
