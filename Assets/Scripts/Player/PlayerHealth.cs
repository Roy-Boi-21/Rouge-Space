using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class PlayerHealth : BaseHealth
{
    [Header("Player Health")]
    [Tooltip("Drag in the player's health bar here.  Make sure it is classified as a filled image.")]
    [SerializeField] private Image healthBar;

    [Tooltip("The text that displays how much health the player currently has.")]
    [SerializeField] private TMP_Text healthText;

    [Tooltip("The animator for the hurt vignette.")]
    [SerializeField] private Animator vignetteAnimator;

    [Tooltip("The camera offset to shake when the player gets hurt.")]
    [SerializeField] private GameObject cameraOffset;

    [Tooltip("How far the camera should be flung from its origin.")]
    [SerializeField] private float cameraShakeIntensity = 1f;

    [Tooltip("For how long the camera shakes after the player takes damage.")]
    [SerializeField] private float cameraShakeTime = 0.25f;

    protected override void Start()
    {
        if (healthBar == null)
        {
            Debug.LogWarning("Health bar not given.");
        }
        else if (healthBar.type != Image.Type.Filled)
        {
            Debug.LogError("Health bar is not a filled image.");
        }

        if (healthText == null)
        {
            Debug.LogError("Health text not given.");
        }

        if (vignetteAnimator == null)
        {
            Debug.LogError("Vignette Animator not given.");
        }

        if (cameraOffset == null)
        {
            Debug.LogError("Camera offset not provided.");
        }

        UpdateHealthBar();

        base.Start();
    }

    public override void Heal(int heal)
    {
        base.Heal(heal);
        UpdateHealthBar();
    }

    public override void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            vignetteAnimator.SetTrigger("Hit");
            StartCoroutine(CameraShakeCoroutine());
        }

        base.TakeDamage(damage);
        UpdateHealthBar();
    }

    public override void Die()
    {
        MusicManager.instance.StopMusic();
        DefeatManager.instance.OpenDefeatMenu();
        base.Die();
    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = (float)currentHealth / (float)maxHealth;
        healthText.text = currentHealth + " / " + maxHealth;
    }

    private IEnumerator CameraShakeCoroutine()
    {
        float timeElapsed = 0f;

        while (timeElapsed <= cameraShakeTime)
        {
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);

            cameraOffset.transform.position = new Vector3(x, y) * cameraShakeIntensity;

            timeElapsed += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        cameraOffset.transform.position = Vector3.zero;
    }
}
