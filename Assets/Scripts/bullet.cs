using UnityEngine;

public class bullet : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);

        }
        Destroy(gameObject);
        
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject,2f);
    }


}
