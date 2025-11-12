using UnityEngine;

public class shooting : MonoBehaviour
{

    public Transform firePoint;
    public GameObject bulletP;
    public float bf = 20f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        
        GameObject bullet = Instantiate(bulletP, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * bf, ForceMode2D.Impulse);
        if (PlayerMovement.tripleShot)
        {
            GameObject bullet1 = Instantiate(bulletP, firePoint.position, firePoint.rotation);
            Rigidbody2D rb1 = bullet1.GetComponent<Rigidbody2D>();
            rb1.AddForce((firePoint.up + 0.2f*Vector3.left) * bf, ForceMode2D.Impulse);
            GameObject bullet2 = Instantiate(bulletP, firePoint.position, firePoint.rotation);
            Rigidbody2D rb2 = bullet2.GetComponent<Rigidbody2D>();
            rb2.AddForce((firePoint.up + 0.2f*Vector3.right) * bf, ForceMode2D.Impulse);
        }

    }
}
