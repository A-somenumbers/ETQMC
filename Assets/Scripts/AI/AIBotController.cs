using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AIBotController : MonoBehaviour
{
    [Header("References")]
    public Transform player;      // drag Player here
    public AStar2D pathfinder;    // drag PathfindingGrid (with AStar2D) here
    public LayerMask wallMask;    // set to Walls

    [Header("Movement / Difficulty")]
    public float easyMoveSpeed = 1.5f;
    public float hardMoveSpeed = 4.5f;
    public float repathIntervalEasy = 1.0f;
    public float repathIntervalHard = 0.2f;
    public float timeToMaxDifficulty = 60f;   // seconds from easy -> hard

    [Header("Vision")]
    public float sightRange = 12f;           // how far the bot can see the player

    [Header("Stuck Detection (optional but useful)")]
    public float stuckDistanceThreshold = 0.01f;
    public float stuckTimeThreshold = 0.4f;

    // runtime
    private Rigidbody2D rb;

    private List<Vector2> currentPath = new List<Vector2>();
    private int pathIndex = 0;

    private float diffTimer = 0f;
    private float moveSpeed;
    private float repathInterval;
    private float repathTimer = 0f;

    private Vector2 lastPos;
    private float stuckTimer = 0f;

    private Vector2 lastSeenPlayerPos;
    private bool hasLastSeenPlayer = false;   // true only if we've seen the player at least once

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true; // keep sprite upright

        moveSpeed      = easyMoveSpeed;
        repathInterval = repathIntervalEasy;
        lastPos        = rb.position;
    }

    void Update()
    {
        if (player == null || pathfinder == null)
        {
            Debug.LogWarning("AIBotController: missing Player or Pathfinder reference");
            return;
        }

        UpdateDifficulty();

        repathTimer -= Time.deltaTime;

        bool canSee = CanSeePlayer();

        if (canSee)
        {
            // We see the player now – update last seen position
            lastSeenPlayerPos = player.position;
            hasLastSeenPlayer = true;

            // Optionally re-path more often while in sight
            if (repathTimer <= 0f)
            {
                RequestPathToLastSeen();
            }
        }
        else
        {
            // Can't see the player – do NOT chase their current position.
            // Only move towards lastSeenPlayerPos if we have one.
            if (hasLastSeenPlayer && repathTimer <= 0f && (currentPath == null || currentPath.Count == 0))
            {
                RequestPathToLastSeen();
            }
        }
    }

    void FixedUpdate()
    {
        FollowPath();
        CheckStuck();
        RotateTowardPlayer();
    }

    // ---------------- DIFFICULTY ----------------
    void UpdateDifficulty()
    {
        diffTimer += Time.deltaTime;
        float t = Mathf.Clamp01(diffTimer / timeToMaxDifficulty);

        moveSpeed      = Mathf.Lerp(easyMoveSpeed,      hardMoveSpeed,      t);
        repathInterval = Mathf.Lerp(repathIntervalEasy, repathIntervalHard, t);
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
            Debug.LogWarning("AIBotController: A* returned NO PATH to lastSeenPlayerPos. Check grid bounds / walls.");
            return;
        }

        // Debug path in Scene view (Gizmos on)
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Debug.DrawLine(currentPath[i], currentPath[i + 1], Color.green, repathInterval);
        }
    }

    void FollowPath(){
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

        // 👇 IMPORTANT: check if this step would hit a wall
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
            RequestPathToLastSeen();   // or RequestPath(player.position) if you're still doing live chase
            return;
        }

        rb.MovePosition(pos + dir * stepDist);
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
                // Force new A* path to the same last seen position
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

    // --------------- LOOK AT PLAYER (just visual) ----------------
    void RotateTowardPlayer()
    {
        if (player == null) return;
        Vector2 dir = (Vector2)player.position - rb.position;
        if (dir.sqrMagnitude < 0.0001f) return;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
