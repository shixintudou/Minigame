using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BActressPrimaryManager : MonoBehaviour
{
    public static BActressPrimaryManager Instance;

    // There are 4 actresses
    public List<BActressPrimary> Actresses;
    public float interval;

    // Start is called before the first frame update
    private float dt = 1;
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        dt -= Time.deltaTime;
        if(dt < 0) {
            StartCoroutine(AttackCoroutine());
            dt += 10000;
        }
    }

    IEnumerator AttackCoroutine() {
        WaitForSeconds waitSecond = new WaitForSeconds(interval);
        Actresses[0].AttackStraight();
        yield return waitSecond;
        Actresses[1].AttackStraight();
        yield return waitSecond;
        Actresses[2].AttackStraight();
        yield return waitSecond;
        Actresses[3].AttackStraight();
        yield return waitSecond;

        Actresses[0].AttackSpiral();
        yield return waitSecond;
        Actresses[2].AttackSpiral();
        yield return waitSecond;
        Actresses[1].AttackSpiral();
        yield return waitSecond;
        Actresses[3].AttackSpiral();
        yield return waitSecond;

        Actresses[0].AttackStraight();
        yield return waitSecond;
        Actresses[2].AttackStraight();
        yield return waitSecond;
        Actresses[1].AttackSpiral();
        yield return waitSecond;
        Actresses[3].AttackSpiral();
        yield return waitSecond;

        Actresses[0].AttackSpiral();
        yield return waitSecond;
        Actresses[2].AttackSpiral();
        yield return waitSecond;
        Actresses[1].AttackStraight();
        yield return waitSecond;
        Actresses[3].AttackStraight();
        yield return waitSecond;

        Actresses[2].AttackGo();
    }
}
