using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float mvSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    //Dashing
    private float activeMvSpeed;
    [SerializeField] private float dashSpeed;
    public float dashLength = .5f, dashCool = 1f;
    private float dashCounter, DashCoolCounter;
    private float currentX;
    private float currentY;

    //PlayerLooking&Following
    [SerializeField] private Camera mainCamera;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        activeMvSpeed = mvSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = transform.position.z;
        Vector3 direction = mouseWorldPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        
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
