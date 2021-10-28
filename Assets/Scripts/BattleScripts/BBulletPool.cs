using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBulletPool : MonoBehaviour
{
    public static BBulletPool Instance = null;

    public const int n = 700;
    public GameObject BulletPrefab;
    public SpriteRenderer[] Images;
    public Animator[] AMs;
    private Queue<GameObject> Queue = new Queue<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        if(Instance != null) {
            Destroy(this.gameObject);
        }
        else {
            Instance = this;
        }

        for (int i = 1; i <= n; i++)
        {
            GameObject tmp = Instantiate(BulletPrefab, this.transform);
            tmp.SetActive(false);
            Queue.Enqueue(tmp);
        }
        //Queue.Dequeue().SetActive(true);
    }

    public GameObject NewBullet(int imageNum)
    {
        if(Queue.Count <= 0) {
            Debug.Log("Bullet Pool has run out!");
            return null;
        }
        GameObject tmp = Queue.Dequeue();
        tmp.SetActive(true);
        if(imageNum != -1) {
            tmp.GetComponent<SpriteRenderer>().sprite = Images[imageNum].sprite;
            tmp.GetComponent<Animator>().runtimeAnimatorController = AMs[imageNum].runtimeAnimatorController;
        }
        return tmp;
    }

    public void Insert(GameObject oldBullet)
    {
        if(oldBullet.activeSelf) {
            oldBullet.SetActive(false);
        }
        Queue.Enqueue(oldBullet);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
