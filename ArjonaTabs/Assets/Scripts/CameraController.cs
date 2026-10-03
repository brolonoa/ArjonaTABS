using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float zoomSpeed = 5f;

    [SerializeField] private Camera camera;

    private void Update()
    {
        MoveCamera();
        ZoomCamera();
    }

    private void MoveCamera()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movement =
            transform.right * horizontal +
            transform.forward * vertical;

        movement.y = 0f;

        transform.position +=
            movement.normalized * moveSpeed * Time.deltaTime;
    }

    private void ZoomCamera()
    {
        float scroll = Input.mouseScrollDelta.y;

        camera.orthographicSize -= scroll * zoomSpeed * Time.deltaTime;

        camera.orthographicSize =
            Mathf.Clamp(camera.orthographicSize, 5f, 20f);
    }
}