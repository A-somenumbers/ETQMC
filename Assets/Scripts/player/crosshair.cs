using UnityEngine;

public class crosshair : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private Camera mainCam;
   
    private void Update()
    {
        if (Time.timeScale > 0f) 
        {
            Vector3 mouseWorldpos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldpos.z = 0;
            transform.position = mouseWorldpos; 
        }
        

        
    }
}
