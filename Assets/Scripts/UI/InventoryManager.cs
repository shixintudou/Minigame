using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance = null;
    public const int initialActionPoints = 30;
    public int ActionPoint = initialActionPoints;
    public int Cost = 2;
    public Text ActionPointText;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        ShowActionPoint();
    }

    public void CostOnce()
    {
        ActionPoint -= Cost;
        ShowActionPoint();
    }

    public void Plus(int num)
    {
        ActionPoint += num;
        ShowActionPoint();
    }

    public void ShowActionPoint()
    {
        ActionPointText.text = "x " + ActionPoint.ToString();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if(ActionPoint<=0)
        {
            Save.Instance.BackToLastSaveScene();
        }
    }
}
