using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBeginMove : MonoBehaviour
{
    public Button thisButton;
    public Image ButtonImage;
    public bool Enableing = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDisable() {
        thisButton.enabled = false;
        Enableing = false;
        ButtonImage.color = Color.grey;
    }

    public void SetEnable() {
        thisButton.enabled = true;
        Enableing = true;
        ButtonImage.color = Color.white;
    }
}
