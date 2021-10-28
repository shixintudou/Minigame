using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSleevePreview : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Set(Vector3 pos, float angle, float life) {
        gameObject.SetActive(true);
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, 0, angle / Mathf.PI * 180f);
        StartCoroutine(EmergeCoroutine(life));
    }
    IEnumerator EmergeCoroutine(float life) {
        Vector3 scale = transform.localScale;
        float dt = 0;
        while(dt < 0.3f) {
            dt += Time.deltaTime;
            scale.y = dt;
            transform.localScale = scale;
            yield return null;
        }
        yield return new WaitForSeconds(life - 0.3f);
        Destroy(gameObject);
    }
}
