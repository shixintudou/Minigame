using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BHPDot : MonoBehaviour
{
    public Image image;
    public float DisappearTime;
    
    public void DestroyNow() {
        StartCoroutine(DestroyCoroutine());
    }
    IEnumerator DestroyCoroutine() {
        float dt = 0;
        Color col = Color.white;
        while(dt < DisappearTime) {
            dt += Time.deltaTime;
            col.a = 1 - dt / DisappearTime;
            image.color = col;
            yield return null;
        }
        Destroy(gameObject);
    }
}
