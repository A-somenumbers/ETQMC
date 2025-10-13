using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public float mvSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    //Dashing
    private float activeMvSpeed;
    [SerializeField] private float dashSpeed;
    public float dashLength = .5f, dashCool = 1f;
    private float dashCounter, DashCoolCounter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        activeMvSpeed = mvSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        moveInput.Normalize();
        Dash();


    }
    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * activeMvSpeed;
    }

    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (DashCoolCounter <= 0 && dashCounter <= 0)
            {
                activeMvSpeed = dashSpeed;
                dashCounter = dashLength;
            }

        }

        if (dashCounter > 0)
        {
            dashCounter -= Time.deltaTime;

            if (dashCounter <= 0)
            {
                activeMvSpeed = mvSpeed;
                DashCoolCounter = dashCool;
            }
        }

        if (DashCoolCounter > 0)
        {
            DashCoolCounter -= Time.deltaTime;
        }
    }
}
