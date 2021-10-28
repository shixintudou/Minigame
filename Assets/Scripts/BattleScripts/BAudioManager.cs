using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BAudioManager : MonoBehaviour
{
    public static BAudioManager Instance;

    public AudioSource Music;
    public AudioSource[] Effects;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        Music.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private int number = 0;
    public void PlayAttack() {
        Effects[number + 1].Play();
        //++number;
        if(number >= 3)
            number -= 3;
    }

    public void PlayDodge() {
        Effects[0].Play();
    }

    public void PlayHurt() {
        Effects[3].Play();
    }

    public void PlayHit1() {
        Effects[5].Play();
    }

    public void PlayHit2() {
        Effects[6].Play();
    }

    public void PlayFive() {
        Effects[4].Play();
    }

    public void StopMusic() {
        StartCoroutine(StopMusicCoroutine());
    }
    IEnumerator StopMusicCoroutine() {
        float dt = 0;
        while(dt < 2f) {
            dt += Time.deltaTime;
            Music.volume = 1f - dt / 2f;
            yield return null;
        }
        Music.Stop();
    }
}
