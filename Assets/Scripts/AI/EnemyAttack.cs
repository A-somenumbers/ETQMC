using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyAttack : MonoBehaviour
{
    [Header("References")]
    public Transform player;          // drag Player here
    public PlayerHealth playerHealth; // drag Player (with PlayerHealth) here
    public LayerMask wallMask;        // set to Walls layer (same as AIBotController.wallMask)

    [Header("Attack Settings")]
    public float attackRange = 5f;
    public float attackCooldown = 1.0f;
    public int damagePerHit = 10;

    float attackTimer = 0f;

    void Update()
    {
        if (player == null || playerHealth == null) return;

        attackTimer -= Time.deltaTime;

        Vector2 from = transform.position;
        Vector2 to   = player.position;
        Vector2 dir  = to - from;
        float dist   = dir.magnitude;

        if (dist > attackRange) return;

        // line-of-sight: if a wall is between enemy and player, don't attack
        RaycastHit2D hit = Physics2D.Raycast(from, dir.normalized, dist, wallMask);
        if (hit.collider != null)
        {
            // hit a wall before we reach the player
            return;
        }

        if (attackTimer <= 0f)
        {
            attackTimer = attackCooldown;
            DoAttack();
        }
    }

    void DoAttack()
    {
        Debug.Log(name + " attacks the player!");
        playerHealth.TakeDamage(damagePerHit);
        // Later: spawn bullets instead of instant damage.
    }
}
