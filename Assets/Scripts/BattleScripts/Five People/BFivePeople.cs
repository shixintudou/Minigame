using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BFivePeople : BEnemy
{
    public float Width;
    public bool Passable;
    public Animator AM;

    private Vector3 velocity;
    private BInkLineAnim nowInkLine;
    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
        AM = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += velocity * Time.deltaTime;
    }

    public override bool Hurt() {
        if(hurting)
            return false;
        hurting = true;
        BFivePeopleManager.Instance.ListRemove(this);
        nowInkLine.Stop();
        StopCoroutine("AttackCoroutine");
        AM.Play("FiveDead");
        velocity = Vector3.zero;
        //改成玩家方向
        float angle = BPlayer.Instance.transform.rotation.eulerAngles.z + 90f;
        Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.PI / 180f), Mathf.Sin(angle * Mathf.PI / 180f), 0);
        transform.localRotation = Quaternion.Euler(0, 0, angle + 90f);
        transform.DOMove(transform.position + dir * 5, 1.0f).SetEase(Ease.OutExpo);
        return true;
    }

    public void Teleport(Vector3 pos) {
        transform.position = pos;
    }

    public void Attack(float angle, float dis, float width, float duration, float remainTime, float KnockbackPower) {
        float absOfVelo = dis / duration;
        int count = (int)(dis / width / 2f);
        velocity = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * absOfVelo;
        transform.localRotation = Quaternion.Euler(0, 0, angle / Mathf.PI * 180 - 90);
        Quaternion rotateQ = Quaternion.AngleAxis(90f, new Vector3(0, 0, 1f));
        nowInkLine = BInkLineAnim.NewInkLine(transform.position, angle, dis, width, duration, remainTime);
        StartCoroutine(AttackCoroutine(duration / count, count, width, remainTime, rotateQ * velocity * KnockbackPower / absOfVelo, transform.position, angle));
        //BAudioManager.Instance.PlayFive();
        transform.position += velocity / absOfVelo;
    }

    IEnumerator AttackCoroutine(float interval, int count, float r, float remainTime, Vector3 knock, Vector3 origPos, float angle) {
        float t = 0;
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
        for(int i = 0; i < count;) {
            yield return waitFrame;
            t += Time.deltaTime;
            while(t > interval) {
                BulletSpawnStraight(origPos + velocity * interval * i, Vector3.zero, remainTime - i * interval, r, 1, knock, 1, Passable, angle, false, false, true);
                t -= interval;
                i ++;
            }
            //BulletSpawn(transform.position, Vector3.zero, remainTime, r, 1, knock, Passable);
        }
    }
}
