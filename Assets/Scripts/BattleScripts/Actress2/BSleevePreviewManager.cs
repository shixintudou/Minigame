using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSleevePreviewManager : MonoBehaviour
{
    public static BSleevePreviewManager Instance;
    public GameObject Prefab;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void NewSleevePreview(Vector3 pos, float angle, float life) {
        BSleevePreview preview = Instantiate(Prefab, transform).GetComponent<BSleevePreview>();
        preview.Set(pos, angle, life);
    }
}
