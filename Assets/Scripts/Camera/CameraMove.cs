using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public int EdgeWidth;
    public float MoveSpeed;

    private bool isMoving = false;
    private Vector3 originPos;
    private float l, r, t, b;
    // Start is called before the first frame update
    void Start()
    {
        ;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void LateUpdate()
    {
        if(isMoving) {
            Vector3 mousePos = Input.mousePosition;
            if(mousePos.x < EdgeWidth || mousePos.y < EdgeWidth ||
            mousePos.x > Screen.width - EdgeWidth || mousePos.y > Screen.height - EdgeWidth) {
                Vector3 dir = new Vector3(mousePos.x - Screen.width / 2, mousePos.y - Screen.height / 2, 0);
                if(Mathf.Abs(dir.x) > Screen.width / 3)
                    dir.x = dir.x > 0 ? 1 : -1;
                else
                    dir.x = 0;
                if(Mathf.Abs(dir.y) > Screen.height / 3)
                    dir.y = dir.y > 0 ? 1 : -1;
                else
                    dir.y = 0;

                transform.position += dir * MoveSpeed * Time.deltaTime;
            }
            if(transform.position.x < originPos.x + l) {
                transform.position = new Vector3(originPos.x + l, transform.position.y, transform.position.z);
            }
            else if(transform.position.x > originPos.x + r) {
                transform.position = new Vector3(originPos.x + r, transform.position.y, transform.position.z);
            }
            if(transform.position.y < originPos.y + b) {
                transform.position = new Vector3(transform.position.x, originPos.y + b, transform.position.z);
            }
            else if(transform.position.y > originPos.y + t) {
                transform.position = new Vector3(transform.position.x, originPos.y + t, transform.position.z);
            }
        }
    }

    public void SetMove(Dictionary<Blocks, bool> forwardBlocks, Vector3 playerPos) {
        if(isMoving == false) {
            isMoving = true;
            originPos = transform.position;
        }
        l = r = t = b = 0;
        foreach (Blocks forwardBlock in forwardBlocks.Keys) {
            l = Mathf.Min(l, forwardBlock.transform.position.x - playerPos.x);
            r = Mathf.Max(r, forwardBlock.transform.position.x - playerPos.x);
            b = Mathf.Min(b, forwardBlock.transform.position.y - playerPos.y);
            t = Mathf.Max(t, forwardBlock.transform.position.y - playerPos.y);
        }
        l--; r++; b--; t++;
    }

    public void StopMove() {
        isMoving = false;
        transform.position = originPos;
    }
}
