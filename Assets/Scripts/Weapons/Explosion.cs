using UnityEngine;

public class Explosion : ProjectileComponent
{
    [Header("Explosion Data")]
    [Tooltip("The start size of the explosion.")]
    [SerializeField] private float startSize = 0.25f;

    [Tooltip("The end size of the explosion.")]
    [SerializeField] private float endSize = 1.0f;

    [Tooltip("The audio source that manages the explosion sound.")]
    [SerializeField] private AudioSource audioSource;

    [Tooltip("The sound that plays when the explosion appears.")]
    [SerializeField] private AudioClip explosionSound;

    Vector2 startVector;
    Vector2 endVector;
    float elapsedTime = 0f;

    protected override void Start()
    {
        startVector = new Vector2(startSize, startSize);
        endVector = new Vector2(endSize, endSize);

        if (explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }
        else
        {
            Debug.LogWarning("Explosion sound not provided.");
        }

        base.Start();
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        float growthRatio = elapsedTime / lifeTime;

        transform.localScale = Vector2.Lerp(startVector, endVector, growthRatio);
    }

    public float GetSize()
    {
        return endSize;
    }

    public void IncreaseSize(float extraSize)
    {
        endSize += extraSize;
    }
}
