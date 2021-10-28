using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BActressPrimary : BEnemy
{
    public float AtkInterval;
    public Animator AM;

    private bool haveDir = false;
    private Vector3 dir;
    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
        AM = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!haveDir) {
            dir = BPlayer.Instance.transform.position - transform.position;
            haveDir = true;
        }
    }

    public void AttackStraight() {
        BulletSpawnStraight(transform.position, dir * 1.2f, 3, 1, 1, Vector3.zero, 0,
                            true, BBullet.GetAngleFormDir(dir) - Mathf.PI / 2f, true, false, false, 0);
    }

    public void AttackSpiral() {
        BulletSpawnSpiral(transform.position, BPlayer.Instance.transform.position, -Mathf.PI / 3, 0.6f, 3, 1, 1, 0, true, true, false, 1);
    }

    public void AttackGo() {
        AM.Play("DashMid");
        transform.rotation = Quaternion.Euler(0, 0, BBullet.GetAngleFormDir(dir) / Mathf.PI * 180f - 90f);
        transform.DOMove(BPlayer.Instance.transform.position - dir * 0.1f, 0.8f).SetEase(Ease.Linear);
        StartCoroutine(AttackGoCoroutine());
    }
    IEnumerator AttackGoCoroutine() {
        yield return new WaitForSeconds(0.9f);
        AM.Play("DashEnd");
        BPlayer.Instance.Hurt(5, Vector3.zero, true);
        BCameraShake.Instance.Shake(0.10f, 0.07f, Vector3.zero);
    }

    public override bool Hurt() {
        Debug.Log("That's impossible!");
        return false;
    }
}
