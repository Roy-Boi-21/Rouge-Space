using UnityEngine;

public class MinionSpawner : MonoBehaviour
{
    [Tooltip("The minion to spawn.")]
    [SerializeField] private GameObject minion;

    [Tooltip("Where the minions spawn.")]
    [SerializeField] private Transform[] minionSpawnPoints;

    [Tooltip("The audio source that controls when the audio plays and how it's categorized.")]
    [SerializeField] private AudioSource audioSource;

    [Tooltip("The sound that plays when a minion spawns.")]
    [SerializeField] private AudioClip spawnSound;

    [Tooltip("How many minions can be active at once.")]
    [SerializeField] private int minionLimit = 3;

    [Tooltip("The time between minion spawns.")]
    [SerializeField] private float minionSpawnDelay = 5f;

    private Transform spawnPoint;
    private int minionCount = 0;
    private float timeElapsed = 0f;

    private void Start()
    {
        if (minion == null)
        {
            Debug.LogError("Minion not provided.");
        }
        else
        {
            minion.SetActive(false);
        }

        if (minionSpawnPoints.Length == 0)
        {
            Debug.LogError("Spawn points not provided.");
        }
    }

    private void FixedUpdate()
    {
        if (minionCount >= minionLimit)
        {
            return;
        }

        timeElapsed += Time.fixedDeltaTime;

        if (timeElapsed >= minionSpawnDelay)
        {
            SpawnMinion();
            timeElapsed = 0f;
        }
    }

    private void SpawnMinion()
    {
        spawnPoint = minionSpawnPoints[Random.Range(0, minionSpawnPoints.Length - 1)];

        minionCount++;

        audioSource.PlayOneShot(spawnSound);

        GameObject newMinion = Instantiate(minion, spawnPoint.position, transform.rotation);
        newMinion.SetActive(true);
        newMinion.GetComponent<MinionHealth>().SetBoss(gameObject);
        EnemyManager.instance.AddEnemy();
    }

    public void RemoveMinion()
    {
        minionCount--;
    }
}
