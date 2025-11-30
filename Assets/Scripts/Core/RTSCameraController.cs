using UnityEngine;

public class RTSCameraController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 15f;
    public float fastMultiplier = 2f;

    [Header("Rotation")]
    public float rotateSpeed = 60f;

    [Header("Zoom")]
    public float zoomSpeed = 150f;
    public float minY = 3f;
    public float maxY = 15f;

    [Header("Bounds")]
    public bool useBounds = true;
    public float minX = -3.89f;
    public float maxX = 6.11f;
    public float minZ = -7.17f;
    public float maxZ = 2.83f;

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
        ClampPosition();
    }

    void HandleMovement()
    {
        Vector3 moveDir = Vector3.zero;

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();

        if (Input.GetKey(KeyCode.W)) moveDir += forward;
        if (Input.GetKey(KeyCode.S)) moveDir -= forward;
        if (Input.GetKey(KeyCode.D)) moveDir += right;
        if (Input.GetKey(KeyCode.A)) moveDir -= right;

        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) speed *= fastMultiplier;

        if (moveDir.sqrMagnitude > 0.001f)
        {
            moveDir.Normalize();
            transform.position += moveDir * speed * Time.deltaTime;
        }
    }

    void HandleRotation()
    {
        float rot = 0f;
        if (Input.GetKey(KeyCode.Q)) rot -= 1f;
        if (Input.GetKey(KeyCode.E)) rot += 1f;

        if (Mathf.Abs(rot) > 0.01f)
            transform.Rotate(Vector3.up, rot * rotateSpeed * Time.deltaTime, Space.World);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) < 0.001f) return;

        Vector3 pos = transform.position;
        pos.y -= scroll * zoomSpeed * Time.deltaTime;

        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }

    void ClampPosition()
    {
        if (!useBounds) return;

        Vector3 pos = transform.position;

        pos.y = Mathf.Clamp(pos.y, minY, maxY);     
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

        transform.position = pos;
    }
}
