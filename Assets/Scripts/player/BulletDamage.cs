using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    public int damage = 10;

    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Only hit enemies
        if (other.CompareTag("MainEnemy"))
        {
            Destroy(gameObject);
            EnemyHealth eh = other.GetComponent<EnemyHealth>();
            if (eh != null)
            {
                eh.TakeDamage(damage);
                  // 👈 only reduce HP, DON'T destroy enemy here
            }

                  // destroy bullet, not enemy
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            Destroy(gameObject);        // bullet dies on walls
        }
    }
}
