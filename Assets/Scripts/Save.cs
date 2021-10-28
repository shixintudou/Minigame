using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save : MonoBehaviour
{
   
    public GameObject MapPrefab;
    public GameObject GridMap;

    public Vector3 LastPosition;
    public int Lastdir;
    public int LastActionPoints;

    public static Save Instance = null;
    // Start is called before the first frame update
    void Start()
    {
        if(Instance!=null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        LastPosition = StringToVector3(PlayerPrefs.GetString("LastPosition", Vector3.zero.ToString()));
        Lastdir = PlayerPrefs.GetInt("Lastdir", -1);
        LastActionPoints = PlayerPrefs.GetInt("LastActionPoints", 0);
       // StartGameLoad();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SavePoint()
    {
        LastActionPoints = InventoryManager.Instance.ActionPoint;
        Lastdir = PlayerMove.Instance.dir;
        LastPosition = PlayerMove.Instance.gameObject.transform.position;
    }

    public void StartGameLoad()
    {
        PlayerMove.Instance.gameObject.transform.position = LastPosition;
        //PlayerMove.Instance.dir = Lastdir;
        InventoryManager.Instance.ActionPoint = LastActionPoints;
    }

    public void BackToLastSavePoint()
    {
        PlayerMove.Instance.gameObject.transform.position = LastPosition;
        //PlayerMove.Instance.dir = Lastdir;
        InventoryManager.Instance.ActionPoint = LastActionPoints;
        InventoryManager.Instance.ShowActionPoint();
        //PlayerMove.Instance.nowBlock = Blocks.GetBlockFromPos(PlayerMove.Instance.transform.position);
        //PlayerMove.Instance.nextBlock = PlayerMove.Instance.nowBlock;
        PlayerMove.Instance.nowState = PlayerMove.State.atCheckPoint;
        PlayerMove.Instance.BeginSettingRoad(true, Lastdir);
        
        

    }

    public void BackToLastSaveScene()
    {
        Destroy(GridMap);
        GridMap = Instantiate(MapPrefab);
        PlayerMove.Instance.gameObject.transform.position = LastPosition;
        PlayerMove.Instance.GridMap = GridMap.transform;

        InventoryManager.Instance.ActionPoint = LastActionPoints;
        InventoryManager.Instance.ShowActionPoint();
        PlayerMove.Instance.nowBlock = Blocks.GetBlockFromPos(PlayerMove.Instance.transform.position);
        PlayerMove.Instance.nextBlock = PlayerMove.Instance.nowBlock;
        PlayerMove.Instance.nowState = PlayerMove.State.atCheckPoint;
        PlayerMove.Instance.BeginSettingRoad(true, Lastdir);
        
        //PlayerMove.Instance.
    }

    public void ExitSave()
    {
        PlayerPrefs.SetInt("Lastdir", Lastdir);
        PlayerPrefs.SetInt("LastActionPoints", LastActionPoints);
        PlayerPrefs.SetString("LastPosition", LastPosition.ToString());
    }

    private Vector3 StringToVector3(string str)
    {
        str = str.Replace("(", "").Replace(")", "");
        string[] tmp = str.Split(',');
        return new Vector3(float.Parse(tmp[0]),float.Parse(tmp[1]), float.Parse(tmp[2]));
    }

}
