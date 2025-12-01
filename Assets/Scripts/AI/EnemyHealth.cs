using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 30;
    int currentHealth;

    [Header("UI")]
    public WorldHealthBar healthBar;   // drag child's WorldHealthBar here

    void Awake()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetHealth(1f);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth < 0)
            currentHealth = 0;

        if (healthBar != null)
        {
            float normalized = (float)currentHealth / maxHealth;
            healthBar.SetHealth(normalized);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(name + " died.");

        // if you use LevelManager, keep this; otherwise you can remove
        // if (LevelManager.Instance != null)
        // {
        //     LevelManager.Instance.OnEnemyDied(gameObject);
        // }

        Destroy(gameObject);
    }
}
