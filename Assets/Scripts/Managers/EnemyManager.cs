using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [Tooltip("A reference to the upgrade manager.")]
    [SerializeField] private UpgradeManager upgradeManager;

    [Tooltip("How many credits the manager has to spend on enemies.")]
    [SerializeField] private int credits = 5;

    [Tooltip("How many credits the manager gains each wave.")]
    [SerializeField] private int creditGain = 1;

    [Tooltip("How quickly the rate of credit gain increases each wave.")]
    [SerializeField] private int creditAcceleration = 1;

    [Tooltip("The list of every enemy the manager could spawn.")]
    [SerializeField] private GameObject[] masterEnemyList;

    [Tooltip("The current wave.")]
    [SerializeField] private int wave = 1;

    [Tooltip("How much time must pass before a new wave starts.")]
    [SerializeField] private float waveDelay = 5f;

    [Tooltip("The difference in time between enemy spawns.")]
    [SerializeField] private float spawnDelay = 1f;

    [Tooltip("The factor at which the spawn time between waves in decreased.")]
    [SerializeField] private float spawnAcceleration = 1.05f;

    [Tooltip("The absolute smallest distance the enemy can spawn from the player.")]
    [SerializeField] private float lowerDistanceBound = 48f;

    [Tooltip("The absolute greatest distance the enemy can spawn from the player.")]
    [SerializeField] private float UpperDistanceBound = 64f;

    [Tooltip("The text to display information about the wave and how many enemies are left.")]
    [SerializeField] private TMP_Text infoText;

    private int initialCredits;
    private List<GameObject> enemyList = new List<GameObject>();
    private GameObject player;
    private int enemyCount = 0;
    private int totalWeight = 0;
    private float timePassed = 0f;

    public static EnemyManager instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        if (infoText == null)
        {
            Debug.LogError("Information TMP text asset not provided.");
        }
        if (upgradeManager == null)
        {
            Debug.LogError("Upgrade Manager not found.  How will the player get upgrades?");
        }

        initialCredits = credits;
        player = GameObject.FindGameObjectWithTag("Player");
        enemyList = GetValidEnemies();
        UpdateText();
    }

    private void FixedUpdate()
    {
        if (enemyList.Count == 0)
        {
            return;
        }

        timePassed += Time.fixedDeltaTime;
        if (timePassed >= spawnDelay)
        {
            SpawnEnemy();
            timePassed = 0f;
        }
    }

    public void UpdateText()
    {
        infoText.text = "Wave: " + wave + " | Enemies Remaining: " + enemyCount;
    }

    private IEnumerator StartNewWave()
    {
        yield return new WaitForSeconds(waveDelay);

        creditGain += creditAcceleration;
        initialCredits += creditGain;
        credits = initialCredits;

        wave++;
        spawnDelay /= spawnAcceleration;
        enemyList = GetValidEnemies();
        GetTotalWeight();
        UpdateText();
    }

    private int GetTotalWeight()
    {
        totalWeight = 0;

        foreach (GameObject enemy in enemyList)
        {
            totalWeight += enemy.GetComponent<EnemyHealth>().GetWeight();
        }

        return totalWeight;
    }

    private List<GameObject> GetValidEnemies()
    {
        List<GameObject> list = new List<GameObject>();

        foreach (GameObject enemy in masterEnemyList)
        {
            EnemyHealth enemyThreat = enemy.GetComponent<EnemyHealth>();
            if ((enemyThreat.GetMinThreat() <= wave) && (enemyThreat.GetMaxThreat() >= wave) && (enemyThreat.GetCost() <= credits))
            {
                list.Add(enemy);
            }
        }

        return list;
    }

    private void SpawnEnemy()
    {
        // Stop spawning enemies if the player is dead.
        if (player == null)
        {
            return;
        }

        GameObject enemyToSpawn = enemyList[Random.Range(0, enemyList.Count)];

        // Determine which enemy to pick based on its weight.
        int weight = Random.Range(0, totalWeight);
        //Debug.Log("Starting Weight: " + weight);
        foreach (GameObject enemy in enemyList)
        {
            weight -= enemy.GetComponent<EnemyHealth>().GetWeight();
            if (weight < 0)
            {
                enemyToSpawn = enemy;
                break;
            }
        }

        // Verify that the credits to spawn the enemy before it does so.
        int cost = enemyToSpawn.GetComponent<EnemyHealth>().GetCost();
        if (credits >= cost)
        {
            credits -= cost;
        }
        else
        {
            enemyList = GetValidEnemies();
            return;
        }

        float distance = Random.Range(lowerDistanceBound, UpperDistanceBound);
        float orientation = Random.Range(0, 2f * Mathf.PI);

        float x = player.transform.position.x + (distance * Mathf.Cos(orientation));
        float y = player.transform.position.y + (distance * Mathf.Sin(orientation));
        Vector3 spawnPosition = new Vector3(x, y, 0);

        GameObject newEnemy = Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        newEnemy.transform.LookAt(player.transform.position);
        enemyCount++;

        UpdateText();
    }

    public void DestroyAllEnemyProjectiles()
    {
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject projectile in projectiles)
        {
            if (projectile.GetComponent<ProjectileComponent>() != null)
            {
                Destroy(projectile);
            }
        }
    }

    public void RemoveEnemy()
    {
        enemyCount--;

        if ((enemyCount == 0) && (enemyList.Count == 0))
        {
            upgradeManager.OpenUpgradeMenu();
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            playerHealth.Heal(playerHealth.GetMaxHealth() / 2);
            DestroyAllEnemyProjectiles();
            StartCoroutine(StartNewWave());
        }

        UpdateText();
    }

    public void AddEnemy()
    {
        enemyCount++;

        UpdateText();
    }

    public int GetWave()
    {
        return wave;
    }
}
