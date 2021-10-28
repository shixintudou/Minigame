using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tmp_magnet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int xtmp= (int)this.transform.position.x, ytmp= (int)this.transform.position.y;
        float xn = this.transform.position.x, yn = this.transform.position.y;
        if (xn - xtmp < -0.5) xtmp -= 1;
        if (xn - xtmp > 0.5) xtmp += 1;
        if (yn - ytmp < -0.5) ytmp -= 1;
        if (yn - ytmp > 0.5) ytmp += 1;

        this.transform.position = new Vector2(xtmp,ytmp);
    }
}
