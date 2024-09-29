using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] private AudioSource myBgmAudio;
    [SerializeField] private AudioSource mySEAudio;
    [SerializeField] private AudioSource birdSound;

    #region 배경음악
    [SerializeField] private AudioClip bgm1;
    [SerializeField] private AudioClip bgm2;
    [SerializeField] private AudioClip bgm3;
    [SerializeField] private AudioClip bossBgm;
    [SerializeField] private AudioClip endingBgm;
    #endregion

    #region 효과음
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip levelUp;
    [SerializeField] private AudioClip wood;
    [SerializeField] private AudioClip dragonRoar;
    [SerializeField] private AudioClip dragonDie;
    [SerializeField] private AudioClip attack1;
    [SerializeField] private AudioClip attack2;
    [SerializeField] private AudioClip attack3;
    #endregion

    private void Awake()
    {
        if (instance == null)
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
        if (myBgmAudio.isPlaying) myBgmAudio.Stop();

        myBgmAudio.clip = bgm2;
        myBgmAudio.volume = 0.2f;
        myBgmAudio.Play();
    }

    public void PlayThirdBgm()
    {
        if (myBgmAudio.isPlaying) myBgmAudio.Stop();

        myBgmAudio.clip = bgm3;
        myBgmAudio.volume = 0.4f;
        myBgmAudio.Play();
    }

    public void PlayBossBgm()
    {
        birdSound.Stop();
        if (myBgmAudio.isPlaying) myBgmAudio.Stop();

        myBgmAudio.clip = bossBgm;
        myBgmAudio.volume = 0.2f;
        myBgmAudio.Play();
    }

    public void PlayEndingBgm()
    {
        if (myBgmAudio.isPlaying) myBgmAudio.Stop();

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
        switch (attackNum)
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
