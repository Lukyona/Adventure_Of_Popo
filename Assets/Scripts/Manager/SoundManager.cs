using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] AudioSource myBgmAudio;
    [SerializeField] AudioSource mySEAudio;
    [SerializeField] AudioSource birdSound;

    #region 배경음악
    [SerializeField] AudioClip bgm1;
    [SerializeField] AudioClip bgm2;
    [SerializeField] AudioClip bgm3;
    [SerializeField] AudioClip bossBgm;
    [SerializeField] AudioClip endingBgm;
    #endregion

    #region 효과음
    [SerializeField] AudioClip click;
    [SerializeField] AudioClip levelUp;
    [SerializeField] AudioClip wood;
    [SerializeField] AudioClip dragonRoar;
    [SerializeField] AudioClip dragonDie;
    [SerializeField] AudioClip attack1;
    [SerializeField] AudioClip attack2;
    [SerializeField] AudioClip attack3;
    #endregion
    
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void PlayFirstBgm()
    {
        myBgmAudio.clip = bgm1;
        myBgmAudio.volume = 0.3f;
        myBgmAudio.Play();
    }

    public void PlaySecondBgm()
    {
        if(myBgmAudio.isPlaying) myBgmAudio.Stop();

        myBgmAudio.clip = bgm2;
        myBgmAudio.volume = 0.2f;
        myBgmAudio.Play();
    }

    public void PlayThirdBgm()
    {
        if(myBgmAudio.isPlaying) myBgmAudio.Stop();

        myBgmAudio.clip = bgm3;
        myBgmAudio.volume = 0.4f;
        myBgmAudio.Play();
    }

    public void PlayBossBgm()
    {
        birdSound.Stop();
        if(myBgmAudio.isPlaying) myBgmAudio.Stop();

        myBgmAudio.clip = bossBgm;
        myBgmAudio.volume = 0.2f;
        myBgmAudio.Play();
    }

    public void PlayEndingBgm()
    {
        if(myBgmAudio.isPlaying) myBgmAudio.Stop();

        myBgmAudio.clip = endingBgm;
        myBgmAudio.volume = 0.7f;
        myBgmAudio.Play();
    }

    public void PlayClickSound()
    {
        mySEAudio.volume = 0.3f;
        mySEAudio.PlayOneShot(click);
    }


    public void PlayLevelUpSound()
    {
        mySEAudio.volume = 0.7f;
        mySEAudio.PlayOneShot(levelUp);
    }

    public void PlayWoodSound()
    {
        mySEAudio.volume = 0.3f;
        mySEAudio.PlayOneShot(wood);
    }

    public void PlayDragonRoarSound()
    {
        mySEAudio.volume = 1f;
        mySEAudio.PlayOneShot(dragonRoar);
    }

    public void PlayDragonDieSound()
    {
        myBgmAudio.Stop();
        mySEAudio.volume = 0.6f;
        mySEAudio.PlayOneShot(dragonDie);
    }

    public void PlayAttackSound(int attackNum)
    {
        mySEAudio.volume = 0.2f;
        switch(attackNum)
        {
            case 1:
                mySEAudio.PlayOneShot(attack1);
                break;
            case 2:
                mySEAudio.PlayOneShot(attack2);
                break;
            case 3:
                mySEAudio.PlayOneShot(attack3);
                break;
        }
    }
}
