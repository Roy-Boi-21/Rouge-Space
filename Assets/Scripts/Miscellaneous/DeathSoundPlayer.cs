using UnityEngine;

public class DeathSoundPlayer : MonoBehaviour
{
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip audioClip;

    private bool activated = false;

    public void LoadClip(AudioClip clip)
    {
        audioClip = clip;
        audioSource.clip = audioClip;
        audioSource.Play();
        activated = true;
    }

    private void Update()
    {
        if (!audioSource.isPlaying && activated)
        {
            Destroy(gameObject);
        }
    }
}
