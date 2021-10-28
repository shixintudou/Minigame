using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewBlock : MonoBehaviour
{
    public static PreviewBlock Instance;

    public SpriteRenderer PreviewImage;
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

    public void ChangeDisplay(GameObject Road) {
        PreviewImage.sprite = Road.GetComponent<SpriteRenderer>().sprite;
        Rotate(0);
    }

    public void Rotate(int rotateTimes) {
        PreviewImage.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -90f * rotateTimes);
    }
}
