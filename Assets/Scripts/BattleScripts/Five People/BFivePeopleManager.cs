using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFivePeopleManager : MonoBehaviour
{
    public static BFivePeopleManager Instance;

    public List<BFivePeople> People;
    public float AtkDis;
    public float AtkDuraton1;
    public float AtkDuraton2;
    public float AtkRemainTime1;
    public float AtkRemainTime2;
    public float KnockbackPower;
    public float SInterval1;
    public float SInterval2;
    Vector3 LBScreen, RTScreen;

    private float width, height;
    private Stack<BFivePeople> RemoveStack;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        LBScreen = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        RTScreen = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        LBScreen.z = RTScreen.z = 0;
        width = RTScreen.x - LBScreen.x;
        height = RTScreen.y - LBScreen.y;

        RemoveStack = new Stack<BFivePeople>();

        StartCoroutine(UpdateCoroutine());
    }

    void Update()
    {
        if(People.Count == 0) {
            BCurtain.Instance.Success();
        }
    }

    IEnumerator UpdateCoroutine()
    {
        yield return new WaitForSeconds(1f);
        while(true) {
            yield return StartCoroutine(Skill1Coroutine(GetVerticles1p1()));
            yield return new WaitForSeconds(1.5f);

            yield return StartCoroutine(Skill2Coroutine(GetVerticles2()));
            yield return new WaitForSeconds(3.5f);

            yield return StartCoroutine(Skill1Coroutine(GetVerticles1p2()));
            yield return new WaitForSeconds(1.5f);

            yield return StartCoroutine(Skill2Coroutine(GetVerticles2()));
            yield return new WaitForSeconds(3.5f);
        }
    }

    private Vector3[] GetVerticles1p1() {
        Vector3[] v = new Vector3[5];
        v[0].y = RTScreen.y - height * Random.Range(0f, 0.1f);
        v[0].x = LBScreen.x + width * (0.4f + Random.Range(0f, 0.2f));

        v[1].y = LBScreen.y + height * (0.375f + Random.Range(0f, 0.25f));
        v[1].x = LBScreen.x + width * Random.Range(0f, 0.06f);

        v[2].y = LBScreen.y + height * Random.Range(0f, 0.1f);
        v[2].x = LBScreen.x + width * (0.23f + Random.Range(0f, 0.2f));

        v[3].y = LBScreen.y + height * Random.Range(0f, 0.1f);
        v[3].x = LBScreen.x + width * (0.57f + Random.Range(0f, 0.2f));

        v[4].y = LBScreen.y + height * (0.375f + Random.Range(0f, 0.25f));
        v[4].x = RTScreen.x - width * Random.Range(0f, 0.06f);

        int flip = Random.Range(0, 2);
        if(flip == 1) {
            for(int i = 0; i < 5; i++) {
                v[i] *= -1;
            }
        }

        return v;
    }

    private Vector3[] GetVerticles1p2() {
        Vector3[] v = new Vector3[5];
        v[0].y = RTScreen.y - height * (0.1f + Random.Range(0f, 0.1f));
        v[0].x = LBScreen.x + width * (0.4f + Random.Range(0f, 0.2f));

        v[1].y = LBScreen.y + height * (0.375f + Random.Range(0f, 0.25f));
        v[1].x = LBScreen.x + width * (0.1f + Random.Range(0f, 0.06f));

        v[2].y = LBScreen.y + height * (0.1f + Random.Range(0f, 0.1f));
        v[2].x = LBScreen.x + width * (0.23f + Random.Range(0f, 0.2f));

        v[3].y = LBScreen.y + height * (0.1f + Random.Range(0f, 0.1f));
        v[3].x = LBScreen.x + width * (0.57f + Random.Range(0f, 0.2f));

        v[4].y = LBScreen.y + height * (0.375f + Random.Range(0f, 0.25f));
        v[4].x = RTScreen.x - width * (0.1f + Random.Range(0f, 0.06f));

        int flip = Random.Range(0, 2);
        if(flip == 1) {
            for(int i = 0; i < 5; i++) {
                v[i] *= -1;
            }
        }
        return v;
    }

    private Vector3[] GetVerticles2() {
        Vector3[] v = new Vector3[5];
        Quaternion rotateQ = Quaternion.AngleAxis(90f, new Vector3(0, 0, 1f));
        float angle = Random.Range(0, 6.28f);
        v[2] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * RTScreen.magnitude;
        Vector3 dx = rotateQ * v[2];
        dx.Normalize();
        dx *= 3;
        v[0] = v[2] + dx * 2;
        v[1] = v[2] + dx;
        v[3] = v[2] - dx;
        v[4] = v[2] - dx * 2;
        return v;
    }

    IEnumerator Skill1Coroutine(Vector3[] v) {
        while(RemoveStack.Count > 0) {
            People.Remove(RemoveStack.Pop());
        }
        int count = People.Count;
        for(int i = 0; i < count; i++) {
            yield return new WaitForSeconds(SInterval1);
            GoAttack(People[i], v[i], v[(i + 1) % 5], AtkDuraton1, AtkRemainTime1);
            People[i].AM.Play("Five1");
        }
    }

    IEnumerator Skill2Coroutine(Vector3[] v) {
        while(RemoveStack.Count > 0) {
            People.Remove(RemoveStack.Pop());
        }
        Vector3 pos = BPlayer.Instance.transform.position;
        Vector3 dx = v[0] - pos;
        dx.Normalize();
        dx *= 1.2f;
        int sig = Random.Range(0, 2);
        if(sig == 0)
            sig = -1;
        dx *= sig;
        int count = People.Count;
        for(int i = 0; i < count; i++) {
            yield return new WaitForSeconds(SInterval2);
            if(i == 0) {
                SummonPreview(v[0], pos);
                yield return new WaitForSeconds(0.1f);
            }
            GoAttack(People[i], v[i], pos + i * dx, AtkDuraton2 - i * 0.15f, AtkRemainTime2);
            People[i].AM.Play("Five2");
        }
    }

    private void GoAttack(BFivePeople person, Vector3 v1, Vector3 v2, float duration, float remainTime) {
        Vector3 dir = v2 - v1, mid = (v2 + v1) / 2;
        dir.Normalize();
        person.Teleport(mid - dir * (AtkDis / 2));
        float angle = Mathf.Atan(dir.y / dir.x);
        if(dir.x < 0)angle += Mathf.PI;
        person.Attack(angle, AtkDis, person.Width, duration, remainTime, KnockbackPower);
    }

    private void SummonPreview(Vector3 v1, Vector3 v2) {
        Vector3 dir = v2 - v1, mid = (v2 + v1) / 2;
        dir.Normalize();
        BInkLineAnim.NewInkLine(mid - dir * (AtkDis / 2), BBullet.GetAngleFormDir(dir), AtkDis * 2, 0.08f, 0.3f, 1f, 0.5f);
    }

    public void ListRemove(BFivePeople person) {
        RemoveStack.Push(person);
    }
}
