using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Fungus;

public class Bubble : MonoBehaviour
{
    public GameObject bindingNPC;
    public int Mydir;
    public int SceneNum;

    public enum Kind
    {
        LeavingVillage,
        TalktoNpc,
    };

    public Kind myKind;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDown() {
        PlayerMove.Instance.WalkInVillage(transform.position, true);
        PlayerMove.Instance.NowBubble = this;
        Debug.Log("mouse down");
    }
    //TODO:请添加更多事件。气泡的类型可以用一个enum表示。

    public void BubbleEvent()
    {
        if(myKind==Kind.LeavingVillage)
        {
            PlayerMove.Instance.nowState = PlayerMove.State.atCheckPoint;
            PlayerMove.Instance.BeginSettingRoad(true, Mydir);
        }
        else if(myKind==Kind.TalktoNpc)
        {
            Flowchart.BroadcastFungusMessage("Meet" + PlayerMove.Instance.NowBubble.tag);
            
        }
    }

}
