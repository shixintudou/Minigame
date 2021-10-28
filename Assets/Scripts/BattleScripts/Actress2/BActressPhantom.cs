using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BActressPhantom : MonoBehaviour
{
    public SpriteRenderer SR;

    private BWaterSleeve sleeve;
    private float lifelength;
    private bool disabled;
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        lifelength -= Time.deltaTime;
        if(lifelength <= 0) {
            Disable();
        }
    }

    public void Enable(Vector3 pos, float angle, float t) {
        gameObject.SetActive(true);
        disabled = false;
        lifelength = 1000;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, 0, angle / Mathf.PI * 180f);
        StartCoroutine(AttackSleeveCoroutine(angle, t));
        
    }
    IEnumerator AttackSleeveCoroutine(float angle, float t) {
        BSleevePreviewManager.Instance.NewSleevePreview(transform.position, angle, 0.7f);
        yield return StartCoroutine(EmergeCoroutine());
        yield return new WaitForSeconds(0.36f);
        sleeve = Instantiate(BWaterSleeve.Prefab, transform).GetComponent<BWaterSleeve>();
        sleeve.Set(transform.position, angle, 0.4f, t);
        lifelength = t;
    }
    IEnumerator EmergeCoroutine() {
        float dt = 0;
        Color col = Color.black;
        while(dt < 0.3f) {
            dt += Time.deltaTime;
            col.a = dt / 0.3f;
            SR.color = col;
            yield return null;
        }
    }

    public void Disable() {
        if(!gameObject.activeSelf || disabled) {
            return;
        }
        disabled = true;
        sleeve.DestroyNow();
        StartCoroutine(HideCoroutine());
    }
    IEnumerator HideCoroutine() {
        float dt = 0;
        Color col = SR.color;
        while(dt < 0.3f) {
            dt += Time.deltaTime;
            col.a = 1 - dt / 0.3f;
            SR.color = col;
            yield return null;
        }
        gameObject.SetActive(false);
    }

}
