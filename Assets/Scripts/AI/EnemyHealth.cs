using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 200;        // Nemesis: 200, Helpers: 50
    int currentHealth;

    [Header("UI")]
    public WorldHealthBar healthBar;   // drag HealthBar child here

    [Header("Nemesis / Helper Spawns")]
    public bool isNemesis = false;          // tick ON only for main Nemesis
    public GameObject helperPrefab;         // assign helper enemy prefab here
    public int damageChunk = 50;            // spawn helpers every 50 damage taken
    public int[] helperSpawnPattern = { 1, 2, 4 };  // 1st, 2nd, 3rd thresholds
    int chunksAlreadySpawned = 0;

    [Header("AI Enrage Hooks (optional)")]
    public AIBotController bot;     // drag Nemesis' AIBotController here
    public EnemyAttack melee;       // drag EnemyAttack here
    public EnemyShooting shooter;   // drag EnemyShooting here
    [HideInInspector] public float currentRage01;   // 0 = calm, 1 = fully enraged


    // Store base values so we can scale from them
    float baseTimeToMaxDifficulty;
    float baseMeleeCooldown;
    float baseFireCooldown;
    float baseAttackRange;

    void Awake()
    {
        // --- Health init ---
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetHealth(1f);
        }

        Debug.Log($"{name} spawned with HP = {currentHealth}");

        // --- Cache AI baseline values for enrage ---
        if (bot != null)
        {
            baseTimeToMaxDifficulty = bot.timeToMaxDifficulty;
        }

        if (melee != null)
        {
            baseMeleeCooldown = melee.attackCooldown;
        }

        if (shooter != null)
        {
            baseFireCooldown = shooter.fireCooldown;
            baseAttackRange  = shooter.attackRange;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        float normalized = (float)currentHealth / maxHealth;

        if (healthBar != null)
        {
            healthBar.SetHealth(normalized);
        }

        Debug.Log($"{name} took {amount} damage. HP now = {currentHealth}");

        // Nemesis-specific behaviour
        if (isNemesis)
        {
            HandleHelperSpawns();
            UpdateEnrage(normalized);
        }

        if (currentHealth == 0)
        {
            Die();
        }
    }

    void HandleHelperSpawns()
    {
        if (helperPrefab == null) return;
        if (damageChunk <= 0) return;

        int damageTaken = maxHealth - currentHealth;  // e.g. 0..200
        int chunksNow   = damageTaken / damageChunk;  // e.g. 0,1,2,3,...

        // For each new "chunk" crossed, spawn helpers according to pattern
        while (chunksAlreadySpawned < chunksNow && chunksAlreadySpawned < helperSpawnPattern.Length)
        {
            int count = helperSpawnPattern[chunksAlreadySpawned];
            Debug.Log($"{name} spawning {count} helper(s) at damage chunk {chunksAlreadySpawned + 1}");
            SpawnHelpers(count);
            chunksAlreadySpawned++;
        }
    }

    void SpawnHelpers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // small random offset around Nemesis
            Vector2 offset = Random.insideUnitCircle * 1.5f;
            Vector3 spawnPos = transform.position + (Vector3)offset;

            GameObject helper = Instantiate(helperPrefab, spawnPos, Quaternion.identity);
            Debug.Log("Spawned helper: " + helper.name);
        }
    }

    void UpdateEnrage(float hp01)
    {
        // hp01 = 1 → full HP (calm)
        // hp01 = 0 → dead
        float rage = 1f - hp01;  // 0..1 (0 calm, 1 super angry)
        currentRage01 = rage;
        // Make the difficulty ramp faster as HP drops
        if (bot != null && baseTimeToMaxDifficulty > 0f)
        {
            // at full HP → baseTimeToMaxDifficulty
            // at 1 HP   → 25% of that time (ramps fast)
            bot.timeToMaxDifficulty = Mathf.Lerp(baseTimeToMaxDifficulty,
                                                 baseTimeToMaxDifficulty * 0.25f,
                                                 rage);
        }

        // Make melee attack faster
        if (melee != null && baseMeleeCooldown > 0f)
        {
            melee.attackCooldown = Mathf.Lerp(baseMeleeCooldown,
                                              baseMeleeCooldown * 0.35f,
                                              rage);
        }

        // Make shooting faster + a bit longer range
        if (shooter != null)
        {
            if (baseFireCooldown > 0f)
            {
                shooter.fireCooldown = Mathf.Lerp(baseFireCooldown,
                                                  baseFireCooldown * 0.3f,
                                                  rage);
            }

            if (baseAttackRange > 0f)
            {
                shooter.attackRange = Mathf.Lerp(baseAttackRange,
                                                 baseAttackRange + 3f,
                                                 rage);
            }
        }
    }

    void Die()
    {
        Debug.Log($"{name} died.");

        // If you're using LevelManager to track waves, you can notify it here:
        // if (LevelManager.Instance != null)
        // {
        //     LevelManager.Instance.OnEnemyDied(gameObject);
        // }

        Destroy(gameObject);
    }
}
