using UnityEngine;

public class EnemyHealth : BaseHealth
{
    [Header("Enemy Health")]
    [Tooltip("Add this amount of score when the enemy dies.")]
    [SerializeField] private int score = 0;

    [Tooltip("How likely this enemy to spawn in the given enemy pool.  A higher weight means it is more likely to spawn.")]
    [SerializeField] private int probabilityWeight = 1;

    [Tooltip("Enemy Manager: How much it costs to deploy this enemy.")]
    [SerializeField] private int cost = 0;

    [Tooltip("Enemy Manager: The first wave this enemy can spawn on.")]
    [SerializeField] private int minThreat = 0;

    [Tooltip("Enemy Manager: The last wave this enemy can spawn on.  Set to '-1' if this enemy can always spawn.")]
    [SerializeField] private int maxThreat = int.MaxValue;

    public override void Die()
    {
        ScoreManager.instance.AddScore(score);
        EnemyManager.instance.RemoveEnemy();
        base.Die();
    }

    public int GetWeight()
    {
        return probabilityWeight;
    }

    public int GetCost()
    {
        return cost;
    }

    public int GetMinThreat()
    {
        return minThreat;
    }

    public int GetMaxThreat()
    {
        return maxThreat;
    }

    private void OnValidate()
    {
        cost = Mathf.Max(1, cost);
    }
}
