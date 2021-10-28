using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class BlockButton : MonoBehaviour, IPointerClickHandler
{
    public Image ParentSlot;
    public GameObject RoadPrefab;
    private static Color initialColor = new Color(1, 1, 1, 0.75f);
    private static Image lastSlot = null;
    // Start is called before the first frame update
    void Start()
    {
        ;
    }

    // Update is called once per frame
    void Update()
    {
        ;
    }

    void OnEnable(){
        ParentSlot.color = initialColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!PlayerMove.Instance.SetRoadPrefab(RoadPrefab))
            return;
        if(lastSlot != null)
            lastSlot.color = initialColor;
        ParentSlot.color = Color.white;
        lastSlot = ParentSlot;
    }
}
