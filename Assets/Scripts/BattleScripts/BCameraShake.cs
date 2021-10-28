using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BCameraShake : MonoBehaviour
{
    public static BCameraShake Instance;

    public Transform Medium;
    public Transform Reference1;
    public Transform Reference2;
    public Transform Reference3;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Medium.position + Reference2.position + Reference3.position;
    }

    public void Shake(float duration, float force, Vector3 dir) {
        if(dir.x == 0 && dir.y == 0)
            dir = new Vector3(1, 1, 0);
        Reference2.position -= dir * force * 1.4f;
        Reference2.DOMove(Reference1.position, duration * 0.5f).SetUpdate(true);
        Reference3.DOShakePosition(duration, -dir * force * 1.3f, 15, 0).SetUpdate(true);
    }

    public void FrameFreeze(float duration) {
        StartCoroutine(UnfreezeCoroutine(duration, Time.timeScale));
    }
    IEnumerator UnfreezeCoroutine(float duration, float lastScale)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = lastScale;
    }
}
