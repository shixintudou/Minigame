using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBullet : MonoBehaviour
{
    public float Lifelength = 3;
    public int damage = 1;
    public Vector3 knockback;
    public float KnockbackPower = 1;
    public Collider2D Collider;
    public bool Destroyable = false;
    public bool HitBackable = false;
    public bool HaveKnockDir = false;
    public float RotateVelo;
    

    private bool Backing = false;
    private bool moving;
    //straight part
    private Vector3 velocity;
    //spiral part
    private Vector3 originPos;
    private Vector3 originDir;
    private float lineVelo, angleVelo;
    private float dt = 0;
    private Animator AM;
    public SpriteRenderer SR;

    public enum MoveType {
        straight,
        spiral,
    }
    public MoveType Type = MoveType.straight;

    // Start is called before the first frame update
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        AM = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!moving)
            return;
        Lifelength -= Time.deltaTime;
        if(Lifelength <= 0) {
            Recycle();
        }
        switch (Type) {
            case MoveType.straight:
                transform.position += velocity * Time.deltaTime;
                break;
            case MoveType.spiral://阿基米德螺线
                dt += Time.deltaTime;
                Quaternion rotateQ = Quaternion.AngleAxis(angleVelo * 180 / Mathf.PI * dt, new Vector3(0, 0, 1f));
                transform.position = originPos + (rotateQ * originDir) * lineVelo * dt;
                transform.localRotation = Quaternion.Euler(0, 0, RotateVelo * dt);
                break;
            default:
                break;
        }
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        OnTriggerStay2D(collision.collider);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<BPlayer>())
        {
            HurtPlayer(other.gameObject.GetComponent<BPlayer>());
        }
        if (other.gameObject.GetComponent<BEnemy>()) //打到敌人
        {
            if(Backing) { //已经被反弹
                other.gameObject.GetComponent<BEnemy>().Hurt();
            }
        }
        /*else if (other.gameObject.GetComponent<BObstacle>()) //撞上障碍物
        {
            Recycle();
        }*/
    }

    public bool Hurt() {
        //Debug.Log("Hurt!");
        if(Destroyable) {
            Recycle();
            return true;
        }
        else {
            return false;
        }
    }

    private void HurtPlayer(BPlayer player) {
        Vector3 distance = player.transform.position - transform.position;
        if(Type == MoveType.straight && HaveKnockDir) {
            if(Vector3.Dot(knockback, distance) > 0) {
                player.Hurt(damage, knockback * KnockbackPower);
            }
            else {
                player.Hurt(damage, -knockback * KnockbackPower);
            }
        }
        else {
            distance.Normalize();
            player.Hurt(damage, distance * KnockbackPower);
        }
    }

    private void Recycle()
    {
        GetComponent<Collider2D>().enabled = false;
        moving = false;
        if(AM) {
            StartCoroutine(BoomCoroutine());
        }
        else {
            gameObject.SetActive(false);
            BBulletPool.Instance.Insert(gameObject);
        }
    }
    IEnumerator BoomCoroutine() {
        float t = Time.time;
        Color col = Color.white;
        AM.Play("End");
        while(Time.time - t < 0.2f) {
            col.a = 1 - (Time.time - t) / 0.2f;
            SR.color = col;
            yield return null;
        }
        gameObject.SetActive(false);
        BBulletPool.Instance.Insert(gameObject);
    }

    public void Set(float life, float r, int d, float power, bool trigger, bool destroyable = false, bool hitBackable = false) {
        moving = true;
        GetComponent<Collider2D>().enabled = true;
        dt = 0;
        Lifelength = life;
        transform.localScale = new Vector3(r * 2, r * 2, 1);
        damage = d;
        KnockbackPower = power;
        Collider.isTrigger = trigger;
        Destroyable = destroyable;
        HitBackable = hitBackable;
    }

    public void SetStraight(Vector3 pos, Vector3 v, Vector3 knock, bool haveDir, float angle) {
        Type = MoveType.straight;
        transform.position = pos;
        velocity = v;
        /*if(Collider is BoxCollider2D) {
            transform.localRotation = Quaternion.Euler(0, 0, angle / Mathf.PI * 180);
        }*/
        transform.localRotation = Quaternion.Euler(0, 0, angle / Mathf.PI * 180);
        knockback = knock;
        HaveKnockDir = haveDir;
    }

    public void SetSpiral(Vector3 pos, Vector3 target, float t, float dAngle) {
        Type  = MoveType.spiral;
        originPos = pos;
        originDir = target - pos;
        lineVelo = originDir.magnitude / t;
        angleVelo = -dAngle / t;
        originDir.Normalize();
        originDir = Quaternion.AngleAxis(dAngle * 180 / Mathf.PI, new Vector3(0, 0, 1f)) * originDir;
    }

    public static float GetAngleFormDir(Vector3 dir) {
        float angle = Mathf.Atan(dir.y / dir.x);
        if(dir.x < 0)
            angle = angle + Mathf.PI;
        return angle;
    }
}
