using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BInkLineAnim : MonoBehaviour
{
    public static GameObject Prefab;
    public static Transform FatherTransform;

    public SpriteRenderer SR;
    public float RemainTime;
    public float DisappearTime;

    private float Duration;
    private float playTime;
    private bool playing;
    private Vector3 maxScale;
    void Awake() {
        if(Prefab == null) {
            Prefab = this.gameObject;
            FatherTransform = transform.parent;
            gameObject.SetActive(false);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        playing = true;
        playTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        RemainTime -= Time.deltaTime;
        playTime += Time.deltaTime;
        if(RemainTime < 0) {
            Color col = Color.white;
            col.a = 1 + (RemainTime / DisappearTime);
            SR.color = col;
        }
        if(RemainTime < -DisappearTime) {
            Destroy(this.gameObject);
        }
        if(playing) {
            if(playTime <= Duration) {
                Vector3 nowScale = maxScale;
                nowScale.x = maxScale.x * playTime / Duration;
                transform.localScale = nowScale;
            }
        }
    }

    public void Stop() {
        playing = false;
    }

    public static BInkLineAnim NewInkLine(Vector3 pos, float angle, float dis, float width, float duration, float remainTime, float alpha = 1f) {
        angle += Mathf.PI;
        BInkLineAnim newInkLine = Instantiate(Prefab, FatherTransform).GetComponent<BInkLineAnim>();
        newInkLine.gameObject.SetActive(true);
        newInkLine.transform.position = pos;
        newInkLine.transform.localRotation = Quaternion.Euler(0, 0, angle / Mathf.PI * 180);
        newInkLine.transform.localScale = new Vector3(0, width * 4.5f, 0);
        newInkLine.maxScale = new Vector3(dis * 0.061f, width * 4.5f, 0);
        newInkLine.Duration = duration;
        newInkLine.RemainTime = remainTime;
        newInkLine.SR.color = new Color(1, 1, 1, alpha);
        return newInkLine;
    }
}
