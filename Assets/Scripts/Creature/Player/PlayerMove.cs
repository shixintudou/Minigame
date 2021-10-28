using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Fungus;

public class PlayerMove : MonoBehaviour
{
    public Blocks nowBlock, nextBlock;
    public LayerMask Ground;
    public float Speed;
    public static PlayerMove Instance;
    public bool ismeet = false;
    public GameObject BackpackUI, BeginMoveUI;
    public ButtonBeginMove BeginMoveButton;
    public Transform GridMap;
    public CameraMove MainCamera;
    public Bubble NowBubble;

    private static int[,] go = new int[4, 2] {{0, 1}, {1, 0}, {0, -1}, {-1, 0}};
    public int dir = -1;//FIXME：dir初始值应该置多少
    private CharacterController controller;
    private int lastCheckPointDir;

    public enum State {
        inVillage,
        atCheckPoint,
        inWild,
        roadSetting,
        waiting,
    };
    public State nowState;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        //nowState = State.inVillage;
        nowState = State.inVillage;
        controller = GetComponent<CharacterController>();
        nowBlock = Blocks.GetBlockFromPos(transform.position);
        nextBlock = nowBlock;
    }

    // Update is called once per frame
    private Blocks targetBlock = null;
//    private bool leavingVillage = false;
    private Dictionary<Blocks, bool> forwardBlocks = null;
    private Blocks selectingBlock;
    private GameObject RoadPrefab;
    private int rotateTimes;
    void Update()
    {
        switch (nowState) {
            case State.inWild:
                if(!ismeet) {
                    if (dir < 0) {
                        if(!nextBlock.Coverable) {
                            //return to checkPoint...
                        }
                        nowState = State.roadSetting;
                        BeginSettingRoad(false,dir);
                    }
                    else {
                        controller.Move(new Vector3(go[dir, 0], go[dir, 1], 0) * Speed * Time.deltaTime);
                        if ((nextBlock.transform.position - transform.position).magnitude < 0.04f) {
                            nowBlock = nextBlock;
                            transform.position = nowBlock.transform.position;
                            dir = nowBlock.GetNextBlock(dir, ref nextBlock);
                            if(nowBlock.type == Blocks.Type.village) {
                                nowState = State.inVillage;
                            }
                        }
                    }
                }
                break;
            case State.inVillage:
                if(targetBlock != null) {
                    if(nowBlock == targetBlock) {//TODO:此时行走结束，应该触发气泡事件
                        targetBlock = null;
                        NowBubble.BubbleEvent();
                        Debug.Log("Bubble Event Begin");
                        /*targetBlock = null;
                        if(leavingVillage) {//FIXME:这是临时的写法。leavingVillage字段应该废除
                            leavingVillage = false;
                            nowState = State.atCheckPoint;
                            BeginSettingRoad(true,dir);
                        }
                        */
                    }
                    else {
                        if (dir < 0) {
                            dir = Blocks.BfsEnemyToPlayer(nowBlock, targetBlock, ref nextBlock);
                        }
                        controller.Move(new Vector3(go[dir, 0], go[dir, 1], 0) * Speed * Time.deltaTime);
                        //Debug.Log(targetBlock.gameObject.name);
                        //Debug.Log(nextBlock.gameObject.name);
                        if ((nextBlock.transform.position - transform.position).magnitude < 0.04f)
                        {
                            nowBlock = nextBlock;
                            transform.position = nowBlock.transform.position;
                            if (nowBlock != targetBlock)
                                dir = Blocks.BfsEnemyToPlayer(nowBlock, targetBlock, ref nextBlock);
                        }
                    }
                }
                break;
            case State.roadSetting:
                if(RoadPrefab != null) {
                    if(!PreviewBlock.Instance.gameObject.activeInHierarchy) {
                        HighlightSet.Instance.Hide();
                        PreviewBlock.Instance.gameObject.SetActive(true);
                        PreviewBlock.Instance.transform.position = selectingBlock.transform.position;
                        selectingBlock.GetComponent<SpriteRenderer>().enabled = false;
                    }
                }
                break;
            case State.atCheckPoint:
                if(RoadPrefab != null) {
                    if(selectingBlock == null) {
                        if(Input.GetKeyDown(KeyCode.Mouse0)) {
                            selectingBlock = Blocks.GetClickingBlock();
                            if(selectingBlock != null) {
                                if(!forwardBlocks.ContainsKey(selectingBlock)) {
                                    selectingBlock = null;
                                }
                            }
                        }
                    }
                    if(selectingBlock != null) {
                        if(!PreviewBlock.Instance.gameObject.activeInHierarchy) {
                            BeginMoveButton.SetDisable();
                            HighlightSet.Instance.Hide();
                            PreviewBlock.Instance.gameObject.SetActive(true);
                            PreviewBlock.Instance.transform.position = selectingBlock.transform.position;
                            selectingBlock.GetComponent<SpriteRenderer>().enabled = false;
                        }
                    }
                }
                else if(!BeginMoveButton.Enableing) {
                    BeginMoveButton.SetEnable();
                }
                break;
            case State.waiting:
                ;
                break;
            default:
                break;
        }
    }

    public void WalkInVillage(Vector3 targetPos, bool leave = false) { //TODO:应该获取对气泡的引用
        if(nowState != State.inVillage) {
            Debug.Log("Player is not in village!");
            return;
        }
        if(targetBlock != null) {
            Debug.Log("Player is Moving!");
            return;
        }
        targetBlock = Blocks.GetBlockFromPos(targetPos);
/*        if(leave)
            leavingVillage = true;
*/
    }

    public void BeginSettingRoad(bool isCheckPoint, int newdir) {
        if (isCheckPoint) dir = newdir;
        forwardBlocks = Blocks.GetForwardBlocks(nowBlock, dir);
        HighlightSet.Instance.Display(forwardBlocks);
        BackpackUI.SetActive(true);
        BeginMoveUI.SetActive(isCheckPoint);
        if(isCheckPoint) {
            selectingBlock = null;
            MainCamera.SetMove(forwardBlocks, nowBlock.transform.position);
        }
        else
            selectingBlock = nextBlock;
        RoadPrefab = null;
    }

    public void BeginWalking() {
        BackpackUI.SetActive(false);
        nowState = State.inWild;
        dir = nowBlock.GetNextBlock(dir, ref nextBlock);
        HighlightSet.Instance.Hide();
        MainCamera.StopMove();
    }

    public bool SetRoadPrefab(GameObject prefab) {
        if(selectingBlock != null && RoadPrefab != null)
            return false;
        RoadPrefab = prefab;
        rotateTimes = 0;
        PreviewBlock.Instance.ChangeDisplay(RoadPrefab);
        return true;
    }

    public void Rotate() {
        rotateTimes++;
        rotateTimes %= 4;
        PreviewBlock.Instance.Rotate(rotateTimes);
    }

    public void Accept() {
        Blocks theNewBlock = Instantiate(RoadPrefab, GridMap).GetComponent<Blocks>();
        if(theNewBlock.NewBlockSet(selectingBlock, rotateTimes) == false) {
            Debug.Log("There would be a loop!");
            Destroy(theNewBlock.gameObject);
            return;
        }
        InventoryManager.Instance.CostOnce();
        if (nowState == State.roadSetting) {
            BackpackUI.SetActive(false);
            PreviewBlock.Instance.gameObject.SetActive(false);
            nowState = State.inWild;
            dir = nowBlock.GetNextBlock((dir + 4) % 4, ref nextBlock);
        }
        else if(nowState == State.atCheckPoint) {
            BackpackUI.SetActive(false);
            BackpackUI.SetActive(true);
            PreviewBlock.Instance.gameObject.SetActive(false);
            selectingBlock = null;
            RoadPrefab = null;
            forwardBlocks = Blocks.GetForwardBlocks(nowBlock, dir);
            HighlightSet.Instance.Display(forwardBlocks);
            MainCamera.SetMove(forwardBlocks, nowBlock.transform.position);
        }
    }

    public void Close() {
        if(nowState == State.roadSetting) {
            BackpackUI.SetActive(false);
            BackpackUI.SetActive(true);
            PreviewBlock.Instance.gameObject.SetActive(false);
            RoadPrefab = null;
            HighlightSet.Instance.Display(forwardBlocks);
        }
        else if(nowState == State.atCheckPoint) {
            BackpackUI.SetActive(false);
            BackpackUI.SetActive(true);
            PreviewBlock.Instance.gameObject.SetActive(false);
            selectingBlock.GetComponent<SpriteRenderer>().enabled = true;
            selectingBlock = null;
            RoadPrefab = null;
            HighlightSet.Instance.Display(forwardBlocks);
        }
    }

    public void Meet() {
        ismeet = true;
    }

    public void NoMeet() {
        ismeet = false;
    }
}
