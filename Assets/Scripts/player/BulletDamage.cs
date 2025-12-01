using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    public int damage = 10;
    public float lifeTime = 3f;

    void Start()
    {
        // auto-destroy after a few seconds so bullets don't live forever
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Only damage enemies
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth eh = other.GetComponent<EnemyHealth>();
            if (eh != null)
            {
                eh.TakeDamage(damage);
            }

            Destroy(gameObject); // bullet disappears on hit
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            // Optional: bullet dies when it hits a wall
            Destroy(gameObject);
        }
    }
}
