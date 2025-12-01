using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyShooting : MonoBehaviour
{
    [Header("References")]
    public Transform player;          // drag Player here
    public Transform firePoint;       // front of the enemy
    public GameObject enemyBullet;    // drag EnemyBullet prefab here
    public LayerMask wallMask;        // set to Walls

    [Header("Attack Settings")]
    public float attackRange = 6f;
    public float fireCooldown = 1.2f;
    public float bulletForce = 7f;    // slower than player bf=20
    public int damagePerHit = 10;     // passed to EnemyBulletDamage

    float fireTimer = 0f;

    void Update()
    {
        if (player == null || firePoint == null || enemyBullet == null) return;

        fireTimer -= Time.deltaTime;

        // Distance check
        Vector2 from = firePoint.position;
        Vector2 to   = player.position;
        Vector2 dir  = to - from;
        float dist   = dir.magnitude;

        if (dist > attackRange) return;

        // Line of sight: raycast against walls
        RaycastHit2D hit = Physics2D.Raycast(from, dir.normalized, dist, wallMask);
        if (hit.collider != null)
        {
            // Wall between enemy and player, don't shoot
            return;
        }

        if (fireTimer <= 0f)
        {
            fireTimer = fireCooldown;
            Shoot(dir.normalized);
        }
    }

    void Shoot(Vector2 direction)
    {
        GameObject b = Instantiate(enemyBullet, firePoint.position, Quaternion.identity);

        // Set damage (optional but nice)
        EnemyBulletDamage dmg = b.GetComponent<EnemyBulletDamage>();
        if (dmg != null)
        {
            dmg.damage = damagePerHit;
        }

        Rigidbody2D rb = b.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(direction * bulletForce, ForceMode2D.Impulse);
        }
    }
}
