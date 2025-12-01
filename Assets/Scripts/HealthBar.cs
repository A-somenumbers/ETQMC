using UnityEngine;

public class WorldHealthBar : MonoBehaviour
{
    [Header("Bar Settings")]
    public Transform fill;   // the green part that shrinks

    // Optional: offset above the character
    public Vector3 worldOffset = new Vector3(0f, 1.2f, 0f);

    // We'll set this from the health script
    float currentNormalized = 1f;

    Transform target; // the character we sit above

    void Awake()
    {
        target = transform.parent; // assume this bar is a child of the character
    }

    public void SetHealth(float normalized)
    {
        normalized = Mathf.Clamp01(normalized);

        if (fill != null)
        {
            // scale in X
            fill.localScale = new Vector3(normalized, 1f, 1f);

            // move the center so the LEFT edge stays fixed
            // when normalized = 1  → offset = 0
            // when normalized = 0.5 → offset = -0.25
            float offset = (normalized - 1f) * 0.5f;
            Vector3 p = fill.localPosition;
            p.x = offset;
            fill.localPosition = p;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Keep bar above the character
        transform.position = target.position + worldOffset;

        // Make sure bar doesn't rotate with character (stay upright)
        transform.rotation = Quaternion.identity;
    }
}
