using UnityEngine;

public class MinionHealth : EnemyHealth
{
    [Header("Minion Data")]
    [Tooltip("The enemy that spawned the minion.")]
    [SerializeField] private GameObject boss;

    public override void Die()
    {
        if (boss != null)
        {
            boss.GetComponent<MinionSpawner>().RemoveMinion();
        }
        base.Die();
    }

    public void SetBoss(GameObject newBoss)
    {
        boss = newBoss;
    }
}
