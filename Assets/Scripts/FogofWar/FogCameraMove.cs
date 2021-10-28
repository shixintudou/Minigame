using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogCameraMove : MonoBehaviour
{
    public GameObject MainCamera;
    public Vector3 Offset = new Vector3(0, 0, 0);
    private Transform MainCameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        MainCameraTransform = MainCamera.transform;
        transform.position = MainCameraTransform.position + Offset;
        transform.rotation = MainCameraTransform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void LateUpdate()
    {
        transform.position = MainCameraTransform.position + Offset;
    }
}
