using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI")]
    public WorldHealthBar healthBar;   // drag the child's WorldHealthBar here

    void Awake()
    {
        currentHealth = maxHealth;

        // initialize bar if assigned
        if (healthBar != null)
        {
            healthBar.SetHealth(1f); // full health
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        if (healthBar != null)
        {
            float normalized = (float)currentHealth / maxHealth;
            healthBar.SetHealth(normalized);
        }

        Debug.Log("Player took damage. HP = " + currentHealth);

        if (currentHealth == 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        gameObject.SetActive(false);
    }
}
