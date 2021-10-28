using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BPlayer : MonoBehaviour
{
    public static BPlayer Instance;

    public Collider2D AttackRange;
    public BSwordLight SwordLight;
    public float basedamage = 1f;
    public int HP = 5;
    public float speed;
    public float dodgespeed;
    public bool Movable = true;
    public float DodgeTime = 0.3f;
    public float KnockbackTime = 0.3f;
    public float HurtCD1, HurtCD2;
    public float AtkCD, AtkPreCD;
    public LayerMask EnemyLayer;
    //public Camera camera;
    public State state;
    Vector2 Dodgeposition;

    private Rigidbody2D RB;
    private SpriteRenderer SR;
    private Animator AM;

    private float nowAtkCD;
    private bool twinkleing;
    private bool invincible;
    
    public enum State
    { 
        Move,
        Dodge,
        Attack,
        Hurt,
        Die,
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;
        Instance = this;
        state = State.Move;
        RB = GetComponent<Rigidbody2D>();
        SR = GetComponent<SpriteRenderer>();
        AM = GetComponent<Animator>();
    }

    private void Attack()
    {
        if(nowAtkCD > 0)
            return;
        AM.Play("Attack1");
        SwordLight.Play();
        BAudioManager.Instance.PlayAttack();
        StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine() {
        nowAtkCD = AtkCD;

        Collider2D[] Contact = new Collider2D[10];
        ContactFilter2D ContactF2D = new ContactFilter2D();
        ContactF2D.useLayerMask = true;
        ContactF2D.useTriggers = true;
        ContactF2D.layerMask = EnemyLayer;

        float t = Time.time;
        //WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        bool hitEnemy = false;
        bool hitBullet = false;
        bool hitActress = false;

        while(Time.time - t < AtkPreCD) {
            yield return null;
            int num = AttackRange.OverlapCollider(ContactF2D, Contact);
            for (int i = 0; i < num; i++)
            {
                if(Contact[i].GetComponent<BEnemy>()) {
                    bool tmp = Contact[i].GetComponent<BEnemy>().Hurt();
                    hitEnemy |= tmp;
                    if(Contact[i].GetComponent<BActress>()) {
                        hitActress = tmp;
                    }
                }
                else if(Contact[i].GetComponent<BBullet>()) {
                    hitBullet |= Contact[i].GetComponent<BBullet>().Hurt();
                }
            }
            if((!hitEnemy) && hitBullet) {
                BCameraShake.Instance.Shake(0.03f, 0.03f, Vector3.zero);
            }
            else if(hitActress) {
                BCameraShake.Instance.Shake(0.10f, 0.05f, Vector3.zero);
                BAudioManager.Instance.PlayHit2();
                BFlashImage.Instance.FlashOnce();
                StartCoroutine(SlowCoroutine());
            }
            else if(hitEnemy) {
                BCameraShake.Instance.Shake(0.10f, 0.05f, Vector3.zero);
                BAudioManager.Instance.PlayHit1();
            }
        }
    }
    IEnumerator SlowCoroutine() {
        Time.timeScale = 0.2f;
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
    }

    public void Hurt(int damage, Vector3 knockback, bool must = false)
    {
        if((invincible || state == State.Dodge || state == State.Die) && !must)
            return;
        HP -= damage;
        BAudioManager.Instance.PlayHurt();
        BHPDisplay.Instance.ReduceHP(damage);
        if(HP <= 0) {
            state = State.Die;
            AM.Play("Die");
            StopAllCoroutines();
            Time.timeScale = 1f;
            StartCoroutine(HideCoroutine());
            BCameraShake.Instance.Shake(0.25f, 0.07f, Vector3.zero);
            BCurtain.Instance.Fail();
            return;
        }
        state = State.Hurt;
        AM.Play("Hurt");
        invincible = true;
        StartCoroutine(HurtCoroutine());
        RB.DOMove(new Vector2(transform.position.x + knockback.x, transform.position.y + knockback.y), KnockbackTime).SetEase(Ease.OutCubic);
        if(Movable)
            transform.rotation = Quaternion.Euler(0, 0, BBullet.GetAngleFormDir(knockback) / Mathf.PI * 180f + 90f);
    }
    IEnumerator HurtCoroutine() {
        yield return new WaitForSeconds(HurtCD1);
        state = State.Move;
        twinkleing = true;
        yield return new WaitForSeconds(HurtCD2);
        invincible = false;
        twinkleing = false;
    }
    IEnumerator HideCoroutine() {
        yield return new WaitForSeconds(1f);
        float dt = 0;
        Color col = SR.color;
        while(dt < 0.3f) {
            dt += Time.deltaTime;
            col.a = 1 - dt / 0.3f;
            SR.color = col;
            yield return null;
        }
    }

    private Vector2 GetMousePosition()
    {
        Vector2 screenposition = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(new Vector3(screenposition.x, screenposition.y, 2.6f));
    }

    private void ChangeFaceTo(Vector2 dir) {
        float angle = Mathf.Atan(dir.y / dir.x) / Mathf.PI * 180 + 90;
        if(dir.x < 0)
            angle += 180;
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    private void Move()
    {
        Vector2 moveposition = -((Vector2)transform.position - GetMousePosition());
        if (moveposition.magnitude > 0.1f)
        {
            ChangeFaceTo(-moveposition);
            moveposition.Normalize();
            if(!Movable)
                return;
            AM.SetBool("Moving", true);
            transform.Translate(moveposition * speed * Time.deltaTime, Space.World);
            //camera.transform.Translate(moveposition * speed * Time.deltaTime);
        }
        else {
            AM.SetBool("Moving", false);
        }
    }
    
    private void DodgeMove()
    { 
        //transform.Translate(Dodgeposition * speed * Time.deltaTime * dodgespeed, Space.World);
        RB.MovePosition((Vector2)transform.position + Dodgeposition * dodgespeed / 16f);
        //Vector2 vector = Vector2.Lerp(camera.transform.position, transform.position, speed * Time.deltaTime * 5);
        //Vector3 cameraposition = new Vector3(vector.x, vector.y, camera.transform.position.z);
        //camera.transform.position = cameraposition;
 
    }

    private void Dodge()
    {
        if(!Movable)
            return;
        Dodgeposition = -((Vector2)transform.position - GetMousePosition());
        Dodgeposition.Normalize();
        ChangeFaceTo(-Dodgeposition);
        AM.Play("Dodge");
        BAudioManager.Instance.PlayDodge();
        //RB.DOMove((Vector2)transform.position + Dodgeposition * dodgespeed, DodgeTime);
        state = State.Dodge;
        StartCoroutine(DodgeBackCourtine());
    }

    IEnumerator DodgeBackCourtine()
    {
        yield return new WaitForSeconds(DodgeTime);
        state = State.Move;
    }

    // Update is called once per frame
    private float twinkleTime = 0f;
    void Update()
    {
        RB.velocity = Vector2.zero;
        if(nowAtkCD > 0) {
            nowAtkCD -= Time.deltaTime;
        }
        switch (state)
        {
            case State.Move:
                if(Input.GetMouseButtonDown(0)) {
                    Attack();
                }
                if(Input.GetMouseButtonDown(1)) {
                    Dodge();
                }
                Move();
                break;
            case State.Attack:
                Move();
                break;
            case State.Dodge:
                DodgeMove();
                break;
            case State.Hurt:
                break;
        }
        if(twinkleing) {
            twinkleTime += Time.deltaTime;
            if(twinkleTime > 0.25f)
                twinkleTime -= 0.25f;
            SR.enabled = twinkleTime > 0.10f;
        }
        else {
            SR.enabled = true;
        }
    }
    
}
