using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] AudioClip click;

    private AudioSource myAudio;
    
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        myAudio = GetComponent<AudioSource>();
    }


    public void PlayClickSound()
    {
        myAudio.PlayOneShot(click);
    }
}
