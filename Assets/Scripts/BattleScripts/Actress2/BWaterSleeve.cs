using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BWaterSleeve : MonoBehaviour
{
    public static GameObject Prefab = null;

    public float MaxLength;
    public float KnockbackPower;
    public int Damage;

    [SerializeField]
    private float width;
    private Vector3 knockback;
    private float lifelength = 1;
    // Start is called before the first frame update
    void Start()
    {
        if(Prefab == null) {
            Prefab = this.gameObject;
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //lifelength -= Time.deltaTime;
        if(lifelength <= 0) {
            DestroyNow();
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.GetComponent<BPlayer>()) {
            HurtPlayer(BPlayer.Instance);
        }
    }

    private void HurtPlayer(BPlayer player) {
        Vector3 distance = player.transform.position - transform.position;
        if(Vector3.Dot(knockback, distance) > 0) {
            player.Hurt(Damage, knockback * KnockbackPower);
        }
        else {
            player.Hurt(Damage, -knockback * KnockbackPower);
        }
    }

    public void Set(Vector3 pos, float angle, float w, float t) {
        gameObject.SetActive(true);
        transform.position = pos;
        transform.localScale = new Vector3(MaxLength, w, 1);
        transform.rotation = Quaternion.Euler(0, 0, angle * 180f / Mathf.PI);
        lifelength = t;
        angle += Mathf.PI / 2f;
        knockback = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
    }

    public void DestroyNow() {
        Destroy(gameObject);
    }
}
