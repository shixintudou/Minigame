using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Npc1Behavior : MonoBehaviour
{
    //private bool ismeet = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerMove.Instance.ismeet)
        {
            Behavior();
        }
    }
    public void Behavior()
    {      
        print("¶Ô»°");
        //Time.timeScale = 0;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerMove.Instance.ismeet = false;
            //Time.timeScale = 1;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            PlayerMove.Instance.ismeet = true;
    }
}
