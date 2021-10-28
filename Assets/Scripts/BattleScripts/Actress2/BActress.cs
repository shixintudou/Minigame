using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BActress : BEnemy
{
    public float HurtCD;
    public SpriteRenderer SR;
    public Animator AM;
    public Rigidbody2D RB;
    public BActressPhantom Phantom;
    public Collider2D AttackRange;
    public int MaxHP;
    public int HP;
    public int HPStage1;
    public float MaxH;
    public float MaxW;
    public float Speed;
    public float DashSpeed1;
    public float DashSpeed2;
    public float DetectDis;

    private bool inHurtCD = false;//和hurting不一样。hurting有中断弹幕的效果，这个没有。
    private bool twinkleing = false;
    private bool dashing = false;
    private float diagonal;
    private Vector3 velocity;
    [SerializeField]
    private float circleSize;

    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
        diagonal = Mathf.Sqrt(MaxH * MaxH + MaxW * MaxW);
    }

    // Update is called once per frame
    private float twinkleTime = 0;
    void Update()
    {
        RB.velocity = Vector2.zero;
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(!dashing)
            return;
        Collider2D other = collision.collider;
        if(other.gameObject.GetComponent<BPlayer>()) {
            HurtPlayer(BPlayer.Instance);
        }
    }
    private void HurtPlayer(BPlayer player, float power = 1f, int damage = 1) {
        Vector3 distance = player.transform.position - transform.position;
        distance.Normalize();
        player.Hurt(damage, distance * power);
    }

    private Vector3 GetWalkDir() {
        float angle = Random.Range(0f, Mathf.PI * 2);
        Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        dir -= transform.position / (diagonal * 0.4f);
        angle = Mathf.Atan(dir.y / dir.x);
        if(dir.x < 0)angle += Mathf.PI;
        dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        return dir;
    }

    private Vector3 GetDashDir() {
        Vector3 dir = transform.position - BPlayer.Instance.transform.position;
        Vector3 dir2 = dir;
        Vector3 pos1, pos2;
        float angle = BBullet.GetAngleFormDir(dir);
        float angle2 = angle;
        while(true) {
            dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            pos1 = transform.position + dir * DashSpeed1 * 0.6f;
            if(Mathf.Abs(pos1.x) <= MaxW && Mathf.Abs(pos1.y) <= MaxH) {
                break;
            }
            angle += Mathf.PI / 8;
        }
        while(true) {
            dir2 = new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), 0);
            pos2 = transform.position + dir2 * DashSpeed1 * 0.6f;
            if(Mathf.Abs(pos2.x) <= MaxW && Mathf.Abs(pos2.y) <= MaxH) {
                break;
            }
            angle2 -= Mathf.PI / 8;
        }
        if((pos1 - BPlayer.Instance.transform.position).magnitude > (pos2 - BPlayer.Instance.transform.position).magnitude)
            return dir;
        else
            return dir2;
    }

    private void ChangeFaceTo(Vector3 dir) {
        transform.DORotate(new Vector3(0, 0, BBullet.GetAngleFormDir(dir) / Mathf.PI * 180f - 90f), 0.25f);
        //transform.rotation = Quaternion.Euler(0, 0, BBullet.GetAngleFormDir(dir) / Mathf.PI * 180f - 90f);
    }

    public void Walk(float t) {
        AM.Play("Run");
        Vector3 dir = GetWalkDir();
        ChangeFaceTo(dir);
        RB.DOMove(transform.position + dir * Speed * t, t).SetEase(Ease.Linear);
        StartCoroutine(WalkReturnCoroutine(t));
    }
    IEnumerator WalkReturnCoroutine(float t) {
        yield return new WaitForSeconds(t);
        AM.Play("Rotate");
    }

    public void AttackSleeve(float angle, float t) {
        Phantom.Enable(transform.position, angle, t);
    }

    public void AttackHit() {
        Collider2D[] Contact = new Collider2D[10];
        ContactFilter2D ContactF2D = new ContactFilter2D();
        ContactF2D.useLayerMask = true;
        ContactF2D.useTriggers = true;
        ContactF2D.layerMask = 1 << LayerMask.NameToLayer("Player");
        
        int num = AttackRange.OverlapCollider(ContactF2D, Contact);
        for (int i = 0; i < num; i++) {
            if(Contact[i].GetComponent<BPlayer>()) {
                HurtPlayer(BPlayer.Instance, 3f, 2);
                BCameraShake.Instance.Shake(0.10f, 0.07f, Vector3.zero);
            }
        }
    }

    Coroutine StopAttackKnifeStraight = null;
    public void AttackKnifeStraight(float count, float interval) {
        StopAttackKnifeStraight = StartCoroutine(AttackKnifeStraightCoroutine(count, interval));
    }
    IEnumerator AttackKnifeStraightCoroutine(float count, float interval) {
        AM.Play("RotateFast");
        WaitForSeconds waitSeconds = new WaitForSeconds(interval);
        float angle = Random.Range(0, Mathf.PI / 4);
        for(int j = 0; j < count; j++) {
            for(int i = 0; i < 8; i++) {
                Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 7;
                BulletSpawnStraight(transform.position, dir, 5, 1, 1, Vector3.zero, 1, true, angle - Mathf.PI / 2, true, false, false, 0);
                angle += Mathf.PI / 4;
            }
            angle += Mathf.PI / 8;
            yield return waitSeconds;
        }
        AM.Play("Rotate");
    }

    public void AttackKnifeSpiral() {
        StartCoroutine("AttackKnifeSpiralCoroutine");
    }
    IEnumerator AttackKnifeSpiralCoroutine() {
        AM.Play("RotateFast");
        yield return new WaitForSeconds(0.5f);
        float angle = Random.Range(0, Mathf.PI / 8);
        float dis = (transform.position - BPlayer.Instance.transform.position).magnitude;
        int sig = Random.Range(0, 2);
        if(sig == 0)
            sig = -1;
        for(int i = 0; i < 16; i++) {
            Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            BulletSpawnSpiral(transform.position, transform.position + dir * dis, Mathf.PI / 3f * sig, dis / 5f, 3, 1, 1, 1, true, false, false, 1);
            angle += Mathf.PI / 8;
        }
        yield return null;
    }

    public IEnumerator AttackDash() {
        yield return StartCoroutine(AttackDashCoroutine());
    }
    IEnumerator AttackDashCoroutine() {
        AM.Play("Rotate");
        float angle = Random.Range(0, Mathf.PI / 3);
        float dt = 0;
        while((transform.position - BPlayer.Instance.transform.position).magnitude > DetectDis) {
            dt += Time.deltaTime;
            if(dt >= 1f) {
                for(int i = 0; i < 6; i++) {
                    BulletSpawnStraight(transform.position, new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 7, 5, 1, 1,
                                         Vector3.zero, 1, true, angle - Mathf.PI / 2, true, false, false, 0);
                    angle += Mathf.PI / 3;
                }
                angle += Mathf.PI / 9;
                dt -= 1f;
            }
            yield return null;
        }
        AM.Play("DashBegin");
        Vector3 dir = GetDashDir();//transform.position - BPlayer.Instance.transform.position;
        //dir.Normalize();
        ChangeFaceTo(-dir);
        RB.DOMove(transform.position + dir * DashSpeed1 * 0.6f, 0.6f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.6f);

        float ratio = (float)(MaxHP - HP) / (float)MaxHP;
        if(Random.Range(0f, 1f) > ratio * 0.5f) {
            yield return new WaitForSeconds(0.6f);
            AM.Play("DashMid");
            angle = Random.Range(0f, Mathf.PI * 2);
            dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            dir = BPlayer.Instance.transform.position + dir * circleSize * ratio - transform.position;
            ChangeFaceTo(dir);
            RB.DOMove(transform.position + dir, dir.magnitude / DashSpeed2).SetEase(Ease.Linear);
            dashing = true;
            yield return new WaitForSeconds(dir.magnitude / DashSpeed2);
            dashing = false;
            AM.Play("DashEnd");
            //Attack!
            yield return new WaitForSeconds(0.13f);
            AttackHit();
            yield return new WaitForSeconds(0.4f);
            dir = GetDashDir();
            ChangeFaceTo(dir);
            RB.DOMove(transform.position + dir * Speed * 1.5f, 1f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(1f);
        }
        else {
            yield return new WaitForSeconds(0.6f);
            AM.Play("Run");
            dir = GetDashDir();
            ChangeFaceTo(dir);
            RB.DOMove(transform.position + dir * Speed * 1.5f, 1f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }

    public override bool Hurt() {
        if(inHurtCD) {
            return false;
        }
        if(BActressManager.Instance.Stage == 1) {
            HPStage1--;
            if(HPStage1 == 0) {
                if(StopAttackKnifeStraight != null)
                    StopCoroutine(StopAttackKnifeStraight);
                Phantom.Disable();
                BActressManager.Instance.ChangeStage();
            }
        }
        else {
            HP--;
            if(HP == 0) {
                AM.Play("Die");
                StopAllCoroutines();
                DOTween.KillAll();
                StartCoroutine(HideCoroutine());
                BActressManager.Instance.Stop();
                BCurtain.Instance.Success();
            }
        }
        inHurtCD = true;
        StartCoroutine(HurtCuroutine());
        return true;
    }
    IEnumerator HurtCuroutine() {
        yield return new WaitForSeconds(0.2f);
        twinkleing = true;
        yield return new WaitForSeconds(HurtCD - 0.2f);
        inHurtCD = false;
        twinkleing = false;
    }

    IEnumerator HideCoroutine() {
        yield return new WaitForSeconds(0.6f);
        float dt = 0;
        Color col = SR.color;
        while(dt < 0.3f) {
            dt += Time.deltaTime;
            col.a = 1 - dt / 0.3f;
            SR.color = col;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
