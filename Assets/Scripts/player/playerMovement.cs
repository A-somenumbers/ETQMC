using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float mvSpeed = 5f;
    private float dmvSpeed;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    //Dashing
    private float activeMvSpeed;
    [SerializeField] private float dashSpeed;
    private float DdashSpeed;
    public float dashLength = .5f, dashCool = 1f;
    private float dashCounter, DashCoolCounter;
    private float currentX;
    private float currentY;

    //PlayerLooking&Following
    [SerializeField] private Camera mainCamera;

    //Items
    bool SpeedUp = false;
    float speedTime = 10f;
    private float timeIn = 0;
    public static bool tripleShot = false; 
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        activeMvSpeed = mvSpeed;
        dmvSpeed = mvSpeed * 2;
        DdashSpeed = dashSpeed * 2;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = transform.position.z;
        Vector3 direction = mouseWorldPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        Dash();
        powerUps();
        Console.WriteLine("time left" + timeIn);


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
    
    void powerUps()
    {

        Debug.Log("the " + timeIn);

        if (SpeedUp)
        {
            mvSpeed = dmvSpeed;
            dashSpeed = DdashSpeed;
            timeIn -= Time.deltaTime;

        }
        if (timeIn < 0)
        {
            mvSpeed = dmvSpeed/2;
            dashSpeed = DdashSpeed/2;
            SpeedUp = false;
            activeMvSpeed = mvSpeed;
            timeIn = 0;
        }
        
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Dash"))
        {
            SpeedUp = true;
            timeIn += speedTime;
            activeMvSpeed = dmvSpeed;
            Destroy(collision.gameObject);        
            
        }
        if (collision.CompareTag("shot"))
        {
            Destroy(collision.gameObject);
            tripleShot = true;
            Console.WriteLine("triple Obtained");
        }
        
    }
}
