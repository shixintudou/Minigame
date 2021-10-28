using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BEnemy : MonoBehaviour
{
    protected bool hurting = false;
    // Start is called before the first frame update
    protected void Start()
    {
        ;
    }
    //子类采用 protected new void Start(),并调用base.Start()执行这个函数

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void BulletSpawnStraight(Vector3 pos, Vector3 v, float life, float r, int d, Vector3 knock, float power,
                                         bool trigger = false, float angle = 0, bool destroyable = false, bool hitBackable = false, bool haveDir = false, int imageNum = -1) {
        if(hurting)
            return;
        BBullet newBullet = BBulletPool.Instance.NewBullet(imageNum)?.GetComponent<BBullet>();
        newBullet.Set(life, r, d, power, trigger, destroyable, hitBackable);
        newBullet.SetStraight(pos, v, knock, haveDir, angle);
    }

    protected void BulletSpawnSpiral(Vector3 pos, Vector3 target, float dAngle, float t, float life, float r, int d, float power,
                                         bool trigger = false, bool destroyable = false, bool hitBackable = false, int imageNum = -1) {
        if(hurting)
            return;
        BBullet newBullet = BBulletPool.Instance.NewBullet(imageNum)?.GetComponent<BBullet>();
        newBullet.Set(life, r, d, power, trigger, destroyable, hitBackable);
        newBullet.SetSpiral(pos, target, t, dAngle);
    }

    public abstract bool Hurt();
}
