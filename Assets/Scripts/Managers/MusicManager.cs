using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    [Tooltip("The source of the music.")]
    [SerializeField] private AudioSource audioSource;

    [Tooltip("The songs that the manager can pick from.")]
    [SerializeField] private AudioClip[] songPool;

    [Tooltip("By what factor should the music's volume be muffled.")]
    [SerializeField] private float mufflePower = 5f;

    [Tooltip("How long it takes for the music to fade out.")]
    [SerializeField] private float fadeOutTime = 1f;

    private bool allowMusic = true;
    private bool isMuffled = false;

    public static MusicManager instance;
    
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (songPool.Length > 0)
        {
            foreach (AudioClip song in songPool)
            {
                song.LoadAudioData();
            }

            PickSong();
            audioSource.Play();
        }
        else
        {
            Debug.LogError("The song pool is empty!  No song will play!");
        }
    }

    private void Update()
    {
        if (!audioSource.isPlaying && allowMusic)
        {
            PickSong();
            audioSource.Play();
        }
    }

    public void MuffleMusic()
    {
        if (!isMuffled) 
        {
            audioSource.volume /= mufflePower;
            isMuffled = true;
        }
    }

    public void UnmuffleMusic()
    {
        if (isMuffled)
        {
            audioSource.volume *= mufflePower;
            isMuffled = false;
        }
    }

    public void FadeOutMusic()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        allowMusic = false;

        float timeRatio = 0f;
        float originalVolume = audioSource.volume;
        for (float time = 0f; time <= fadeOutTime; time += Time.fixedDeltaTime)
        {
            timeRatio = time / fadeOutTime;
            audioSource.volume = Mathf.Lerp(originalVolume, 0f, timeRatio);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        audioSource.Stop();
        audioSource.volume = originalVolume;
    }

    private void PickSong()
    {
        audioSource.clip = songPool[Random.Range(0, songPool.Length)];
    }

    public void StopMusic()
    {
        allowMusic = false;
        audioSource.Stop();
    }
}
