using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HighlightSet : MonoBehaviour
{
    public static HighlightSet Instance;
    public SpriteRenderer SquareHighlight;
    public int SquareCount;
    public float FlashInterval;
    private List<SpriteRenderer> squares;
    private int displayCount;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        SquareHighlight.gameObject.SetActive(false);
        squares = new List<SpriteRenderer>();
        for(int i = 0; i < SquareCount; i++) {
            squares.Add(Instantiate(SquareHighlight.gameObject, transform).GetComponent<SpriteRenderer>());
        }
    }

    private bool isDisplaying = false;
    private bool increasing = false;
    private float intensity;
    // Update is called once per frame
    void Update()
    {
        if(isDisplaying) {
            if(increasing) {
                intensity += Time.deltaTime / FlashInterval;
                if(intensity >= 1f)
                    increasing = false;
            }
            else {
                intensity -= Time.deltaTime / FlashInterval;
                if(intensity <= 0f)
                    increasing = true;
            }
            //int lightIntensity = (int)(192 * intensity);
            for(int i = 0; i < displayCount; i++) {
                Color tColor = squares[i].color;
                tColor.a = intensity * 0.75f;
                squares[i].color = tColor;
            }
        }
        else {
            intensity = 1f;
        }
    }

    public void Display(Dictionary<Blocks, bool> forwardBlocks) {
        displayCount = forwardBlocks.Count;
        if(displayCount > SquareCount) {
            for(int i = SquareCount; i < displayCount; i++) {
                squares.Add(Instantiate(SquareHighlight.gameObject, transform).GetComponent<SpriteRenderer>());
            }
            SquareCount = displayCount;
        }
        int j = 0;
        foreach (Blocks forwardBlock in forwardBlocks.Keys) {
            squares[j].gameObject.SetActive(true);
            squares[j].transform.position = forwardBlock.transform.position;
            j++;
        }
        for(; j < SquareCount; j++) {
            squares[j].gameObject.SetActive(false);
        }
        isDisplaying = true;
    }

    public void Hide() {
        for(int j = 0; j < displayCount; j++) {
            squares[j].gameObject.SetActive(false);
        }
        isDisplaying = false;
    }
}
