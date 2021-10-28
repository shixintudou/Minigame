using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BActressManager : MonoBehaviour
{
    public static BActressManager Instance;

    public BActress Actress;
    public BActressSummon SummonPrefab;
    public BActressSummon[] Summons;
    public int SummonNum;
    public int Stage; // 1 or 2

    private Coroutine StopStage;
    private Coroutine StopS1P1 = null;
    private Vector3 quaterH, quaterW;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        Stage = 1;
        Summons = new BActressSummon[SummonNum];
        for(int i = 0; i < SummonNum; i++) {
            Summons[i] = Instantiate(SummonPrefab, transform).GetComponent<BActressSummon>();
        }
        StopStage = StartCoroutine(Stage1Coroutine());
        quaterH = new Vector3(0, Actress.MaxH / 2f, 0);
        quaterW = new Vector3(Actress.MaxW / 2f, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        ;
    }

    IEnumerator Stage1Coroutine() {
        yield return new WaitForSeconds(1f);
        while(true) {
            SkillS1P1();
            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator Stage2Coroutine() {
        Actress.Walk(0.5f);
        yield return new WaitForSeconds(0.5f);
        while(true) {
            SkillS2P1();
            Actress.Walk(0.8f);
            yield return new WaitForSeconds(2.3f);
            Actress.AttackKnifeSpiral();
            yield return new WaitForSeconds(1f);
            yield return Actress.AttackDash();
        }
    }

    private void SkillS1P1() {
        Vector3 dir = BPlayer.Instance.transform.position - Actress.transform.position;
        float angle = Mathf.Atan(dir.y / dir.x);
        int sig = Random.Range(0, 2);
        if(sig == 0) sig = -1;
        angle += Mathf.PI / Random.Range(2.6f, 3f) * sig;
        dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        dir = Quaternion.AngleAxis(90f, new Vector3(0, 0, 1f)) * dir;
        dir *= 3f;
        StopS1P1 = StartCoroutine(SkillS1P1Coroutine(angle, dir));
    }
    IEnumerator SkillS1P1Coroutine(float angle, Vector3 dir) {
        Vector3 origPos = Actress.transform.position - Quaternion.AngleAxis(90f, new Vector3(0, 0, 1f)) * dir * 1;
        yield return new WaitForSeconds(0.1f);
        for(int i = 0; i < SummonNum / 2; i++) {
            Summons[i * 2 + 1].transform.position = origPos + dir * (i + 0.5f);
            Summons[i * 2 + 1].AttackSleeve(angle, 4);
            Summons[i * 2].transform.position = origPos - dir * (i + 0.5f);
            Summons[i * 2].AttackSleeve(angle, 4);
            yield return new WaitForSeconds(0.1f - 0.02f * i);
        }
        Actress.AttackKnifeStraight(7, 0.58f);
    }

    private void SkillS2P1() {
        Vector3 dir = BPlayer.Instance.transform.position - Actress.transform.position;
        float angle = Mathf.Atan(dir.y / dir.x);
        int sig = Random.Range(0, 2);
        if(sig == 0) sig = -1;
        angle += Mathf.PI / Random.Range(2.6f, 3f) * sig;
        Actress.AttackSleeve(angle, 2);
        Summons[0].transform.position = posRand(quaterH + quaterW);
        Summons[1].transform.position = posRand(quaterH - quaterW);
        Summons[2].transform.position = posRand(- quaterH + quaterW);
        Summons[3].transform.position = posRand(- quaterH - quaterW);
        for(int i = 0; i < 4; i++) {
            angle += Mathf.PI / 5f + Random.Range(-0.2f, 0.2f);
            Summons[i].AttackSleeve(angle, 2);
        }
    }
    private Vector3 posRand(Vector3 pos) {
        pos = new Vector3(pos.x + Random.Range(-2f, 2f), pos.y + Random.Range(-1.3f, 1.3f), 0);
        return pos;
    }

    public void ChangeStage() {
        if(StopS1P1 != null) {
            StopCoroutine(StopS1P1);
            StartCoroutine(ChangeStageCoroutine());
        }
        if(StopStage != null)
            StopCoroutine(StopStage);
        Stage = 2;
        StopStage = StartCoroutine(Stage2Coroutine());
    }
    IEnumerator ChangeStageCoroutine() {
        for(int i = 0; i < SummonNum / 2; i++) {
            Summons[i * 2 + 1].Hurt();
            Summons[i * 2].Hurt();
            yield return new WaitForSeconds(0.1f - 0.02f * i);
        }
    }

    public void Stop() {
        StopCoroutine(StopStage);
    }
}
