using UnityEngine;

public class EnemyBulletDamage : MonoBehaviour
{
    public int damage = 10;
    public float lifeTime = 3f;
    
    void Start()
    {
        
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Hit the player?
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            bool shiel = other.GetComponent<PlayerMovement>().shielded;
            if (ph != null)
            {
                if(shiel == false)
                {
                    ph.TakeDamage(damage);
                }
                Destroy(gameObject);
            }

            //Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            // Bullets die on walls
            Destroy(gameObject);
        }
    }
}
