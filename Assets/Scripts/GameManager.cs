using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    public int nowLevel = 1;
    // Start is called before the first frame update
    private Scene NowScene;
    void Start()
    {
        if(Instance == null) {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*public void Load(string name) {
        LoadSceneParameters para = new LoadSceneParameters();
        para.loadSceneMode = LoadSceneMode.Additive;
        NowScene = SceneManager.LoadScene(name, para);
    }*/
    public void Load() {
        if(nowLevel == 1) {
            SceneManager.LoadScene("Battle_FivePeople");
            nowLevel = 2;
        }
        else if(nowLevel == 3) {
            SceneManager.LoadScene("Battle_PrimaryActress");
            nowLevel = 4;
        }
        else if(nowLevel == 5) {
            SceneManager.LoadScene("Battle_Actress2");
            nowLevel = 6;
        }
        else if(nowLevel == 7) {//Impossible
            SceneManager.LoadScene("Battle_FivePeople");
            nowLevel = 8;
        }
    }

    public void Abort(bool succeed) {
        if(succeed) {
            if(nowLevel == 2) {
                SceneManager.LoadScene("Main2");
                nowLevel = 3;
            }
            else if(nowLevel == 4) {
                SceneManager.LoadScene("Main3");
                nowLevel = 5;
            }
            else if(nowLevel == 6) {
                SceneManager.LoadScene("Main4");
                nowLevel = 7;
            }
        }
        else {
            if(nowLevel == 2) {
                SceneManager.LoadScene("Main1");
                nowLevel = 1;
            }
            else if(nowLevel == 4) {
                SceneManager.LoadScene("Main3");
                nowLevel = 5;
            }
            else if(nowLevel == 6) {
                SceneManager.LoadScene("Main3");
                nowLevel = 5;
            }
        }
    }
}
