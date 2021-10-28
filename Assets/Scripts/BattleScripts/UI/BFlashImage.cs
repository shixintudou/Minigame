using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BFlashImage : MonoBehaviour
{
    public static BFlashImage Instance;
    
    public Image[] images;
    public float MaxA;
    
    private int num = 0;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void FlashOnce() {
        if(num >= 4)
            MaxA = 0.4f;
        if(num >= 8)
            MaxA = 0.7f;
        if(num >= 9) {
            return;
        }
        StartCoroutine(FlashCoroutine());
    }
    IEnumerator FlashCoroutine() {
        Debug.Log(num);
        images[num].gameObject.SetActive(true);
        float dt = 0;
        Color col = Color.white;
        col.a = 0;
        while(dt < 0.1f) {
            dt += Time.deltaTime * 5f;
            col.a = MaxA * (dt / 0.1f);
            images[num].color = col;
            yield return null;
        }
        dt = 0;
        while(dt < 0.9f) {
            dt += Time.deltaTime * 5f;
            col.a = MaxA * (1 - dt / 0.9f);
            images[num].color = col;
            yield return null;
        }
        ++num;
    }
}
