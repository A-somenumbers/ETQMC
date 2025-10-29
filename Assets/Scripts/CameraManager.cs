using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private Transform _cameraTransform;
    private Vector3 _cameraOffset;
    private const float CAM_SPEED = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cameraOffset = new Vector3(0, 0, -10f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetPosition = _playerMovement.transform.position + _cameraOffset; // where the camera wants to be

        _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, targetPosition, CAM_SPEED * Time.fixedDeltaTime);


    }
}
