using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    [Tooltip("The noise the button makes when clicked.")]
    [SerializeField] private AudioClip buttonNoise;

    [Tooltip("The object that plays the sound after the button disappears.")]
    [SerializeField] private GameObject deathSoundPlayer;

    public void PlaySound()
    {
        GameObject deathObject = Instantiate(deathSoundPlayer, transform.position, transform.rotation);

        if (deathObject.TryGetComponent<DeathSoundPlayer>(out DeathSoundPlayer deathPlayer))
        {
            deathPlayer.LoadClip(buttonNoise);
        }
        else
        {
            Debug.LogError("The produced death object is not a death sound player!");
        }
    }
}
