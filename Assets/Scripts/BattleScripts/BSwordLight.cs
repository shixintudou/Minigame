using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BSwordLight : MonoBehaviour
{
    public float AppearTime;
    public float RemainTime;
    public float DisappearTime;
    public float GoDis;
    public Transform ReferenceTransform;

    private SpriteRenderer SR;
    private float nowTime;
    private float t1, t2, t3;
    private Color col = Color.white;
    private Vector3 goDir;
    // Start is called before the first frame update
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        SR.enabled = false;
        t1 = AppearTime;
        t2 = t1 + RemainTime;
        t3 = t2 + DisappearTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(nowTime < t3) {
            nowTime += Time.deltaTime;
            float alpha = 0;
            if(nowTime < t1) {
                alpha = 1;//nowTime / AppearTime;
                //transform.position += goDir * Time.deltaTime * GoDis / t1;
            }
            else if (nowTime < t2) {
                alpha = 1;
            }
            else {
                alpha = 1 - (nowTime - t2) / DisappearTime;
            }
            col.a = alpha;
            SR.color = col;
        }
        else if(SR.enabled == true) {
            SR.enabled = false;
        }
    }

    public void Play() {
        SR.enabled = true;
        transform.position = ReferenceTransform.position;
        transform.localRotation = ReferenceTransform.rotation;
        float angle = (transform.localRotation.eulerAngles.z + 90f) / 180f * Mathf.PI;
        goDir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        transform.position -= goDir * GoDis;
        transform.DOMove(transform.position + goDir * GoDis, t1).SetEase(Ease.OutQuad);
        nowTime = 0;
    }
}
