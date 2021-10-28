using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BHPDisplay : MonoBehaviour
{
    public static BHPDisplay Instance;

    public GameObject Prefab;
    public int MaxHP;
    
    private BHPDot[] Dots;
    private int NowHP;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        NowHP = MaxHP;
        Dots = new BHPDot[MaxHP];
        for(int i = 0; i < MaxHP; i++) {
            Dots[i] = Instantiate(Prefab, transform).GetComponent<BHPDot>();
            Dots[i].gameObject.SetActive(true);
        }
    }

    public void ReduceHP(int count) {
        while(count > 0) {
            if(NowHP <= 0) {
                break;
            }
            Dots[NowHP - 1].DestroyNow();
            count--;
            NowHP--;
        }
    }
}
