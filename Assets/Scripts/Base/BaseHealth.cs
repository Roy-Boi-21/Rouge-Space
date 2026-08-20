using System;
using System.Collections;
using UnityEngine;

public class BaseHealth : MonoBehaviour
{
    [Header("Base Health")]
    [Tooltip("The current health of the object.")]
    [SerializeField] protected int currentHealth = 5;

    [Tooltip("The maximum health of the object.")]
    [SerializeField] protected int maxHealth = 5;

    [Tooltip("The damage reduction of this object.")]
    [SerializeField] protected int armor = 0;

    [Tooltip("The amount of invincibility time the object gets after taking damage.")]
    [SerializeField] protected float invincibilityTime = 0f;

    [Tooltip("Optional: Drag the object's sprite renderer in here to make it flash when it takes damage.")]
    [SerializeField] protected SpriteRenderer spriteRenderer;

    [Serializable]
    public class SpriteData
    {
        [Tooltip("The sprite at the corresponding health state.")]
        public Sprite sprite;

        [Tooltip("The health needed to trigger this sprite appearing.")]
        public int healthThreshold;
    }

    [Tooltip("The sprites this object can take on depending on its health.")]
    [SerializeField] private SpriteData[] sprites;

    [Tooltip("Optional: The color that flashes when this object takes damage.")]
    [SerializeField] protected Color damageColor = Color.red;

    [Tooltip("Optional: How long the sprite's color appears damaged after getting hit.")]
    [SerializeField] protected float damageFlashTime = 0.125f;

    [Tooltip("Optional: The color that flashes when this object is invincible.")]
    [SerializeField] protected Color invincibleColor = new Color(1f, 1f, 1f, 0.5f);

    [Tooltip("Optional: How quickly the sprite's color alternates between its default color and its invincible color.")]
    [SerializeField] protected float invincibleFlashTime = 0.125f;

    [Tooltip("Optional: The lower bound of the object's pitch shift.")]
    [SerializeField] protected float lowerPitch = 0.75f;

    [Tooltip("Optional: The upper bound of the object's pitch shift.")]
    [SerializeField] protected float upperPitch = 1.25f;

    [Tooltip("Optional: The audio source responsible for playing and categorizing the audio.")]
    [SerializeField] protected AudioSource audioSource;

    [Tooltip("Optional: The sound that plays when this object gets hit.")]
    [SerializeField] protected AudioClip hitSound;

    [Tooltip("Optional: The sound that plays when this object gets hit, however this objects armor nullifys the damage it would take.")]
    [SerializeField] protected AudioClip deflectSound;

    [Tooltip("Optional: The sound that plays when this object gets dies.")]
    [SerializeField] protected AudioClip deathSound;

    [Tooltip("Optional: The object that spawns when this object dies to play the death sound.")]
    [SerializeField] protected GameObject deathSoundPlayer;

    [Tooltip("Optional: The particle effect that appears when the object get hit.")]
    [SerializeField] protected GameObject hitEffect;

    [Tooltip("Optional: The particle effect that appears when the object dies.")]
    [SerializeField] protected GameObject deathEffect;

    protected Color originalColor;
    protected bool isInvincible = false;
    protected bool wasDamaged = false;
    protected bool isDying = false;
    private int spriteIndex = 0;

    protected virtual void Start()
    {
        if (spriteRenderer != null) 
        { 
            originalColor = spriteRenderer.color;
        }
        else
        {
            originalColor = Color.white;
        }
    }

    public virtual void Heal(int heal)
    {
        currentHealth += heal;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public virtual int GetMaxHealth()
    {
        return maxHealth;
    }

    public virtual void SetMaxHealth(int newMax)
    {
        maxHealth = newMax;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if (isInvincible || isDying)
        {
            return;
        }

        int realDamage = damage - armor;

        if (realDamage <= 0)
        {
            if (deflectSound != null)
            {
                audioSource.pitch = UnityEngine.Random.Range(lowerPitch, upperPitch);
                audioSource.PlayOneShot(deflectSound);
            }
            return;
        }

        currentHealth -= realDamage;

        if (sprites.Length > 0 && spriteIndex < (sprites.Length - 1))
        {
            if (currentHealth <= sprites[spriteIndex + 1].healthThreshold)
            {
                spriteIndex++;
                spriteRenderer.sprite = sprites[spriteIndex].sprite;
            }
        }

        if (!wasDamaged)
        {
            StartCoroutine(DamageFlash());
        }

        if (hitEffect != null)
        {
            GameObject newEffect = Instantiate(hitEffect, transform.position, transform.rotation);
            newEffect.SetActive(true);
        }

        if (hitSound != null)
        {
            audioSource.pitch = UnityEngine.Random.Range(lowerPitch, upperPitch);
            audioSource.PlayOneShot(hitSound);
        }

        if (currentHealth <= 0 && !isDying)
        {
            Die();
        }

        if (invincibilityTime > 0f)
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    public virtual void Die()
    {
        isDying = true;

        if (deathEffect != null)
        {
            GameObject newEffect = Instantiate(deathEffect, transform.position, transform.rotation);
            newEffect.SetActive(true);
        }

        if (deathSound != null && deathSoundPlayer != null)
        {
            GameObject deathObject = Instantiate(deathSoundPlayer, transform.position, transform.rotation);

            if (deathObject.TryGetComponent<DeathSoundPlayer>(out DeathSoundPlayer deathPlayer))
            {
                //audioSource.PlayOneShot(deathSound);
                deathPlayer.LoadClip(deathSound);
            }
            else
            {
                Debug.LogError("The produced death object is not a death sound player!");
            }
        }

        Destroy(gameObject);
    }

    protected IEnumerator DamageFlash()
    {
        wasDamaged = true;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = damageColor;
        }
        yield return new WaitForSeconds(damageFlashTime);
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        wasDamaged = false;
    }

    protected IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        for (float i = 0f; i < invincibilityTime; i += invincibleFlashTime)
        {
            if ((spriteRenderer != null) && (!wasDamaged))
            {
                if (spriteRenderer.color == originalColor)
                {
                    spriteRenderer.color = invincibleColor;
                }
                else
                {
                    spriteRenderer.color = originalColor;
                }
            }
            yield return new WaitForSeconds(invincibleFlashTime);
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        isInvincible = false;
    }

    private void OnValidate()
    {
        invincibleFlashTime = Mathf.Max(1f/512f, invincibleFlashTime);
        lowerPitch = Mathf.Min(lowerPitch, upperPitch);
        upperPitch = Mathf.Max(lowerPitch, upperPitch);
    }
}
