using UnityEngine;

public class AIBotController : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Vision")]
    public float sightRange = 10f;
    public LayerMask obstacleMask;   // walls

    private Vector3 _lastSeenPlayerPos;
    private bool _hasSeenPlayer;

    void Update()
    {
        if (player == null) return;

        bool canSeePlayer = CanSeePlayer();

        if (canSeePlayer)
        {
            _hasSeenPlayer = true;
            _lastSeenPlayerPos = player.position;
            MoveTowards(player.position);
        }
        else if (_hasSeenPlayer)
        {
            // go to last known position
            float dist = Vector2.Distance(transform.position, _lastSeenPlayerPos);
            if (dist > 0.1f)
                MoveTowards(_lastSeenPlayerPos);
        }
        else
        {
            // idle / patrol later
        }
    }

    bool CanSeePlayer()
    {
        Vector2 toPlayer = player.position - transform.position;
        if (toPlayer.magnitude > sightRange) return false;

        // raycast to check walls
        RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer.normalized, toPlayer.magnitude, obstacleMask);
        // if we hit something, we can't see
        return hit.collider == null;
    }

    void MoveTowards(Vector3 target)
    {
        Vector2 dir = (target - transform.position).normalized;
        transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
    }
}
