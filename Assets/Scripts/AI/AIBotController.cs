using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AIBotController : MonoBehaviour
{
    EnemyHealth myHealth;

    [Header("References")]
    public Transform player;      // drag Player here
    public AStar2D pathfinder;    // drag PathfindingGrid (with AStar2D) here
    public LayerMask wallMask;    // set to Walls

    [Header("Movement / Difficulty (base)")]
    public float easyMoveSpeed = 1.5f;
    public float hardMoveSpeed = 4.5f;
    public float repathIntervalEasy = 1.0f;
    public float repathIntervalHard = 0.2f;
    public float timeToMaxDifficulty = 60f;   // seconds from easy -> hard

    [Header("Enrage Tuning")]
    public float calmMoveSpeed = 2.5f;        // when rage = 0, lerp with base difficulty
    public float enragedMoveSpeed = 6f;       // absolute max speed when fully enraged
    public float calmRepathInterval = 0.6f;   // extra safety, but we mainly use difficulty repath
    public float enragedRepathInterval = 0.15f;

    [Header("Direct Chase Fallback")]
    public float directChaseMinDistance = 0.3f;      // don't jitter when right on top
    public float stuckDirectChaseSpeedMultiplier = 1.3f;

    [Header("Vision")]
    public float sightRange = 12f;           // how far the bot can see the player

    [Header("Stuck Detection")]
    public float stuckDistanceThreshold = 0.01f;
    public float stuckTimeThreshold = 0.4f;

    // runtime
    Rigidbody2D rb;

    List<Vector2> currentPath = new List<Vector2>();
    int pathIndex = 0;

    float diffTimer = 0f;
    float difficulty01 = 0f;    // 0..1 from easy to hard

    float moveSpeed;            // effective speed after difficulty + rage
    float repathInterval;       // effective repath after difficulty + rage
    float repathTimer = 0f;

    Vector2 lastPos;
    float stuckTimer = 0f;

    Vector2 lastSeenPlayerPos;
    bool hasLastSeenPlayer = false;   // true only if we've seen the player at least once

    void Awake()
    {
        myHealth = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true; // keep sprite upright

        moveSpeed      = easyMoveSpeed;
        repathInterval = repathIntervalEasy;
        lastPos        = rb.position;
    }

    void Update()
    {
        // If we don't have references (or the player was destroyed), do nothing.
        if (player == null || pathfinder == null)
        {
            return;
        }

        UpdateDifficulty();   // updates difficulty01

        repathTimer -= Time.deltaTime;

        bool canSee = CanSeePlayer();

        if (canSee)
        {
            lastSeenPlayerPos = player.position;
            hasLastSeenPlayer = true;

            if (repathTimer <= 0f)
            {
                RequestPathToLastSeen();
            }
        }
        else
        {
            if (hasLastSeenPlayer && repathTimer <= 0f &&
                (currentPath == null || currentPath.Count == 0))
            {
                RequestPathToLastSeen();
            }
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // 0..1 rage from health (0 calm, 1 fully enraged)
        float rage = (myHealth != null) ? myHealth.currentRage01 : 0f;

        // --- Combine difficulty ramp + enrage into final speed + repath ---

        // Base from difficulty: easy -> hard
        float baseMoveFromDiff   = Mathf.Lerp(easyMoveSpeed,      hardMoveSpeed,      difficulty01);
        float baseRepathFromDiff = Mathf.Lerp(repathIntervalEasy, repathIntervalHard, difficulty01);

        // Final move speed: blend base difficulty speed toward enraged speed
        moveSpeed = Mathf.Lerp(baseMoveFromDiff, enragedMoveSpeed, rage);

        // Final repath interval: as rage increases, recalc more often
        float baseRepath = Mathf.Lerp(baseRepathFromDiff, calmRepathInterval, rage); // optional extra blend
        repathInterval   = Mathf.Lerp(baseRepath, enragedRepathInterval, rage);

        // --- Movement ---

        // If we don't currently have a usable path, directly chase the player
        if (currentPath == null || currentPath.Count == 0 || pathIndex >= currentPath.Count)
        {
            DirectChase();
            CheckStuck();
            RotateTowardPlayer();
            return;
        }

        FollowPath();
        CheckStuck();
        RotateTowardPlayer();
    }

    // ---------------- DIFFICULTY ----------------
    void UpdateDifficulty()
    {
        diffTimer += Time.deltaTime;
        difficulty01 = Mathf.Clamp01(diffTimer / timeToMaxDifficulty);
    }

    // ---------------- VISION ----------------
    bool CanSeePlayer()
    {
        Vector2 from = rb.position;
        Vector2 to   = player.position;
        Vector2 dir  = to - from;
        float dist   = dir.magnitude;

        if (dist > sightRange)
            return false;

        // Raycast: if we hit a wall before the player, we can't see them
        RaycastHit2D hit = Physics2D.Raycast(from, dir.normalized, dist, wallMask);
        if (hit.collider != null)
        {
            return false;
        }

        return true;
    }

    // --------------- PATHFINDING ----------------
    void RequestPathToLastSeen()
    {
        if (!hasLastSeenPlayer) return;

        currentPath = pathfinder.FindPath(transform.position, lastSeenPlayerPos);
        pathIndex = 0;
        repathTimer = repathInterval;

        if (currentPath == null || currentPath.Count == 0)
        {
            // Path failed; let FixedUpdate fall back to DirectChase
            currentPath = null;
            return;
        }

        // Debug path in Scene view (Gizmos on)
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Debug.DrawLine(currentPath[i], currentPath[i + 1], Color.green, repathInterval);
        }
    }

    void FollowPath()
    {
        if (currentPath == null || currentPath.Count == 0) return;
        if (pathIndex >= currentPath.Count) return;

        Vector2 pos    = rb.position;
        Vector2 target = currentPath[pathIndex];
        Vector2 toNode = target - pos;
        float   dist   = toNode.magnitude;

        // Reached this node? Go to the next one.
        if (dist < 0.05f)
        {
            pathIndex++;
            return;
        }

        Vector2 dir      = toNode.normalized;
        float   stepDist = moveSpeed * Time.fixedDeltaTime;
        if (stepDist > dist) stepDist = dist;

        // Check if this step would hit a wall
        RaycastHit2D hit = Physics2D.Raycast(
            pos,
            dir,
            stepDist + 0.05f,
            wallMask   // make sure this is set to Walls in Inspector
        );

        if (hit.collider != null)
        {
            // We'd collide with a wall if we moved -> force a new A* path
            repathTimer = 0f;
            RequestPathToLastSeen();
            return;
        }

        rb.MovePosition(pos + dir * stepDist);
    }

    // --------------- DIRECT CHASE FALLBACK ----------------
    void DirectChase()
    {
        // Use player’s current position to keep pressure high,
        // but ONLY if there is no wall in the way.
        Vector2 myPos = rb.position;
        Vector2 targetPos = player.position;
        Vector2 toTarget = targetPos - myPos;
        float dist = toTarget.magnitude;

        if (dist <= directChaseMinDistance)
            return;

        Vector2 dir = toTarget.normalized;

        // if a wall is between us and the player, don't direct-chase.
        RaycastHit2D hit = Physics2D.Raycast(myPos, dir, dist, wallMask);
        if (hit.collider != null)
        {
            // A wall is blocking line of sight → let A* pathfinding handle it.
            return;
        }

        float chaseSpeed = moveSpeed * stuckDirectChaseSpeedMultiplier;
        rb.MovePosition(myPos + dir * chaseSpeed * Time.fixedDeltaTime);
    }


    // --------------- STUCK HANDLING ----------------
    void CheckStuck()
    {
        Vector2 pos = rb.position;
        float moved = Vector2.Distance(pos, lastPos);

        if (moved < stuckDistanceThreshold && hasLastSeenPlayer)
        {
            stuckTimer += Time.fixedDeltaTime;
            if (stuckTimer >= stuckTimeThreshold)
            {
                stuckTimer = 0f;
                // Force new A* path to the last seen position
                repathTimer = 0f;
                RequestPathToLastSeen();
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPos = pos;
    }

    // --------------- LOOK AT PLAYER (visual only) ----------------
    void RotateTowardPlayer()
    {
        if (player == null) return;
        Vector2 dir = (Vector2)player.position - rb.position;
        if (dir.sqrMagnitude < 0.0001f) return;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
