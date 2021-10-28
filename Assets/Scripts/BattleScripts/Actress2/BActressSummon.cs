using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BActressSummon : BEnemy
{
    public SpriteRenderer SR;
    public Animator AM;

    private BWaterSleeve sleeve;
    private bool inHurtCD = true;//和hurting不一样。hurting有中断弹幕的效果，这个没有。
    private int thisStage;
    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
        SR = GetComponent<SpriteRenderer>();
        AM = GetComponent<Animator>();
    }

    // Update is called once per frame
    float ddt = 1000;
    void Update()
    {
        ddt -= Time.deltaTime;
        if(ddt < 0) {
            if(thisStage == 1) {
                Hurt();
            }
            else {
                Boom();
            }
            ddt += 1000;
        }
    }

    //这个函数同时拥有唤起的功能
    public void AttackSleeve(float angle, float t) {
        gameObject.SetActive(true);
        ddt = 1000;
        transform.rotation = Quaternion.Euler(0, 0, angle / Mathf.PI * 180f);
        thisStage = BActressManager.Instance.Stage;
        StartCoroutine(AttackSleeveCoroutine(angle, t));
    }
    IEnumerator AttackSleeveCoroutine(float angle, float t) {
        inHurtCD = false;
        BSleevePreviewManager.Instance.NewSleevePreview(transform.position, angle, 0.7f);
        yield return StartCoroutine(EmergeCoroutine());
        yield return new WaitForSeconds(0.36f);
        sleeve = Instantiate(BWaterSleeve.Prefab, transform).GetComponent<BWaterSleeve>();
        sleeve.Set(transform.position, angle, 0.4f, t);
        ddt = t;
    }

    public void AttackKnife() {
        StartCoroutine(AttackKnifeCoroutine());
    }
    IEnumerator AttackKnifeCoroutine() {
        yield return StartCoroutine(BecomeRedCoroutine());
        float angle = Random.Range(0, Mathf.PI / 4);
        for(int i = 0; i < 8; i++) {
            Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 7;
            BulletSpawnStraight(transform.position, dir, 1.5f, 1, 1, Vector3.zero, 1, true, angle - Mathf.PI / 2, true, false, false, 0);
            angle += Mathf.PI / 4;
        }
        StartCoroutine(HideCoroutine());
    }

    private void Emerge() {
        StartCoroutine(EmergeCoroutine());
    }
    IEnumerator EmergeCoroutine() {
        float dt = 0;
        Color col = Color.black;
        while(dt < 0.3f) {
            dt += Time.deltaTime;
            col.a = dt / 0.3f;
            SR.color = col;
            yield return null;
        }
    }

    private void Hide() {
        StartCoroutine(HideCoroutine());
    }
    IEnumerator HideCoroutine() {
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

    private void BecomeRed() {
        StartCoroutine(BecomeRedCoroutine());
    }
    IEnumerator BecomeRedCoroutine() {
        float dt = 0;
        Color col = Color.black;
        while(dt < 0.3f) {
            dt += Time.deltaTime;
            col.r = dt / 0.3f;
            SR.color = col;
            yield return null;
        }
    }

    public override bool Hurt() {
        if(inHurtCD)
            return false;
        inHurtCD = true;
        StopAllCoroutines();
        //Debug.Log(sleeve);
        Hide();
        sleeve?.DestroyNow();
        sleeve = null;
        return true;
    }

    public void Boom() {
        if(inHurtCD)
            return;
        inHurtCD = true;
        AttackKnife();
        sleeve?.DestroyNow();
        sleeve = null;
    }
}
