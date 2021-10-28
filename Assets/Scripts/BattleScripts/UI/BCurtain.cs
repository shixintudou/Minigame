using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BCurtain : MonoBehaviour
{
    public static BCurtain Instance;
    public Image Curtain;

    private bool ended = false;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fail() {
        if(ended)
            return;
        ended = true;
        gameObject.SetActive(true);
        StartCoroutine(FailCoroutine());
    }
    IEnumerator FailCoroutine() {
        yield return new WaitForSeconds(1f);
        float dt = 0;
        Color col = Curtain.color;
        while(dt <= 1f) {
            dt += Time.deltaTime;
            col.a = dt * 0.5f;
            Curtain.color = col;
            yield return null;
        }
        dt = 0;
        yield return new WaitForSeconds(0.5f);
        BAudioManager.Instance.StopMusic();
        while(dt <= 1f) {
            dt += Time.deltaTime;
            col.g = col.b = 1f - dt * 1f;
            col.r = 1 - dt * 0.7f;
            col.a = 0.5f + dt * 0.3f;
            Curtain.color = col;
            yield return null;
        }
        dt = 0;
        while(dt <= 2f) {
            dt += Time.deltaTime;
            col.r = 0.3f - dt * 0.15f;
            col.a = 0.8f + dt * 0.10f;
            Curtain.color = col;
            yield return null;
        }
        dt = 0;
        GameManager.Instance.Abort(false);
    }

    public void Success() {
        if(ended)
            return;
        ended = true;
        gameObject.SetActive(true);
        StartCoroutine(SuccessCoroutine());
    }
    IEnumerator SuccessCoroutine() {
        yield return new WaitForSeconds(1f);
        float dt = 0;
        Color col = Curtain.color;
        col.r = col.g = 0.9f;
        col.b = 0.9f;
        while(dt <= 2f) {
            dt += Time.deltaTime;
            col.a = dt * 0.5f;
            Curtain.color = col;
            yield return null;
        }
        dt = 0;
        yield return new WaitForSeconds(0.3f);
        BAudioManager.Instance.StopMusic();
        while(dt <= 1f) {
            dt += Time.deltaTime;
            //col.r = col.g = 1f - dt;
            col.r = col.g = col.b = (1f - dt) * 0.9f;
            Curtain.color = col;
            yield return null;
        }
        GameManager.Instance.Abort(true);
    }
}
