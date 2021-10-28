using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Blocks : MonoBehaviour
{
    [SerializeField]
    private Blocks[] Next;
    public bool[] Connected;
    public bool Coverable;
    public static LayerMask Ground = 1 << 7;
    private bool initialized = false;
    public Blocks oldblock1;

    public enum Type {
        village,
        checkPoint,
        wild,
    };
    public Type type = Type.wild;
    // Start is called before the first frame update
    void Start()
    {
        if (initialized)
            return;
        Next = new Blocks[4]; //Up, Right, Down, Left
        Vector3 posBack = transform.position + Vector3.back;
        RaycastHit hitInfo;
        Physics.Raycast(posBack + Vector3.up, Vector3.forward, out hitInfo, 5f, Ground);
        Next[0] = hitInfo.collider?.GetComponent<Blocks>();
        Physics.Raycast(posBack + Vector3.right, Vector3.forward, out hitInfo, 5f, Ground);
        Next[1] = hitInfo.collider?.GetComponent<Blocks>();
        Physics.Raycast(posBack + Vector3.down, Vector3.forward, out hitInfo, 5f, Ground);
        Next[2] = hitInfo.collider?.GetComponent<Blocks>();
        Physics.Raycast(posBack + Vector3.left, Vector3.forward, out hitInfo, 5f, Ground);
        Next[3] = hitInfo.collider?.GetComponent<Blocks>();
    }

    // Update is called once per frame
    void Update()
    {
        ;
    }

    public static Blocks GetBlockFromPos(Vector3 pos) {
        Physics.Raycast(pos + Vector3.back, Vector3.forward, out RaycastHit hitInfo, 5f, Ground);
        return hitInfo.collider?.GetComponent<Blocks>();
    }

    public static Blocks GetClickingBlock() {
        if(EventSystem.current.IsPointerOverGameObject())
            return null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity)) {
            return hitInfo.collider.GetComponent<Blocks>();
        }
        return null;
    }

    public int GetNextBlock(int dir, ref Blocks nextBlock)
    {
        if(dir < 0)
            dir += 4;
        if (Connected[dir]) {
            nextBlock = Next[dir];
            if (Next[dir].CouldPass(dir)) {
                return dir;
            }
            else {
                return dir - 4;
            }
        }
        else {
            int newDir = (dir + 1) % 4;
            if (!Connected[newDir]) {
                newDir = (dir + 3) % 4;
            }
            if (Connected[newDir]) {
                nextBlock = Next[newDir];
                if (Next[newDir].CouldPass(newDir)) { 
                    return newDir;
                }
                else {
                    return newDir - 4;
                }
            }
            return -5;
        }
    }

    private void Rotate(int times)
    {//顺时针旋转
        bool[] newConnected = new bool[4];
        for (int i = 0; i < 4; i++)
        {
            newConnected[(i + times) % 4] = Connected[i];
        }
        for (int i = 0; i < 4; i++)
        {
            Connected[i] = newConnected[i];
        }
    }

    private static void NewBlockLink(Blocks newBlock, Blocks oldBlock) {
        if(newBlock.Next == null || newBlock.Next.Length != 4) {
            newBlock.Next = new Blocks[4];
        }
        for (int i = 0; i < 4; i++)
        {
            newBlock.Next[i] = oldBlock.Next[i];
            if(oldBlock.Next[i])
                oldBlock.Next[i].Next[(i + 2) % 4] = newBlock;
        }
    }

    public bool NewBlockSet(Blocks oldBlock, int rotateTimes)
    {
        if(ExistLoop(this, rotateTimes, oldBlock)) {
            return false;
        }
        NewBlockLink(this, oldBlock);
        transform.position = oldBlock.transform.position;
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -90f * rotateTimes);

        //destroy trees above the oldblock
        BoxCollider TreeTest = oldBlock.gameObject.GetComponent<BoxCollider>();
        Collider[] Trees= Physics.OverlapBox(TreeTest.gameObject.transform.position, TreeTest.size*0.5f, new Quaternion(0,0,0,0), LayerMask.GetMask("Item") ,QueryTriggerInteraction.Collide);
        foreach ( Collider i in Trees)
        {
            if (i.tag=="Tree")
            {

                Destroy(i.gameObject);
                Debug.Log(i.name+" destroyed");
            }
        }

        Destroy(oldBlock.gameObject);
        Rotate(rotateTimes);
        initialized = true;
        return true;
    }

    

    private bool CouldPass(int dir)
    {
        return Connected[(dir + 2) % 4];
    }

    private struct BfsNode
    {
        public Blocks block;
        public int enterDir, dep;
        public BfsNode(Blocks B, int eD, int Dep) {
            block = B; enterDir = eD; dep = Dep;
        }
        public BfsNode(Blocks B, BfsNode lB) {
            block = B; enterDir = lB.enterDir; dep = lB.dep + 1;
        }
    }
    public static int BfsEnemyToPlayer(Blocks startBlock, Blocks endBlock, ref Blocks nextBlock)
    {
        if(startBlock == endBlock)
            return -1;
        int maxDis = 15;
        Queue<BfsNode> Q = new Queue<BfsNode>();
        Dictionary<Blocks, bool> hash = new Dictionary<Blocks, bool>();
        for(int dir = 0; dir < 4; dir++) {
            if(startBlock.Connected[dir] && startBlock.Next[dir].CouldPass(dir)) {
                Q.Enqueue(new BfsNode(startBlock.Next[dir], dir, 1));
                hash.Add(startBlock.Next[dir], true);
            }
        }
        BfsNode ansNode = new BfsNode(startBlock, 0, 0);
        while (Q.Count > 0) {
            BfsNode nowNode = Q.Dequeue();
            Blocks now = nowNode.block;
            if (now == endBlock) {
                ansNode = nowNode;
                break;
            }
            if (nowNode.dep > maxDis) break;
            for (int dir = 0; dir < 4; dir++) {
                if (now.Connected[dir] && now.Next[dir].CouldPass(dir)) {
                    if (!hash.ContainsKey(now.Next[dir])) {
                        Q.Enqueue(new BfsNode(now.Next[dir], nowNode));
                        hash.Add(now.Next[dir], true);
                    }
                }
            }
        }
        if (ansNode.dep == 0) return -1;
        nextBlock = startBlock.Next[ansNode.enterDir];
        return ansNode.enterDir;
    }

    //传入老地块和新地块，判断新地块会不会造成循环，并且不改变原有结构
    //做法：先把新地块连接进来，判断完成后再改回去
    private struct Block_Dir
    {
        public Blocks block;
        public int dir;
        public Block_Dir(Blocks B,int Dir) {
            block = B; dir = Dir;
        }
    }
    public static bool ExistLoop(Blocks newBlock, int rotateTimes, Blocks oldBlock) {
        newBlock.Rotate(rotateTimes);
        NewBlockLink(newBlock, oldBlock);
        Queue<Block_Dir> Q = new Queue<Block_Dir>();
        Dictionary<Block_Dir, bool> hash = new Dictionary<Block_Dir, bool>();
        bool flag = false;
        for(int i = 0; i < 4; i++) {
            if(newBlock.Connected[i]) {
                Block_Dir node = new Block_Dir(newBlock, i);
                Q.Enqueue(node);
                hash.Add(node, true);
            }
            while (Q.Count > 0) {
                Block_Dir now = Q.Dequeue();
                Blocks nextBlock = null;
                int dir = now.block.GetNextBlock(now.dir, ref nextBlock);
                if(nextBlock.type == Type.village)
                    continue;
                if(dir >= 0) {
                    Block_Dir node = new Block_Dir(nextBlock, dir);
                    if(hash.ContainsKey(node)) {
                        flag = true;
                        break;
                    }
                    else {
                        Q.Enqueue(node);
                        hash.Add(node, true);
                    }
                }
            }
        }
        NewBlockLink(oldBlock, newBlock);
        newBlock.Rotate(4 - rotateTimes);
        return flag;
    }

    public static Dictionary<Blocks, bool> GetForwardBlocks(Blocks nowBlock, int dir) {
        Dictionary<Blocks, bool> forwardBlocks = new Dictionary<Blocks, bool>();
        Blocks nextBlock = null;
        do {
            dir = nowBlock.GetNextBlock(dir, ref nextBlock);
            nowBlock = nextBlock;
            if(!nextBlock.Coverable)
                break;
            if(!forwardBlocks.ContainsKey(nextBlock))
                forwardBlocks.Add(nextBlock, true);
        }while(dir >= 0);
        return forwardBlocks;
    }
}
