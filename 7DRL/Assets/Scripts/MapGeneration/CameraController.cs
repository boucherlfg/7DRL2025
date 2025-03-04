using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 15f;
    public MapGenerator mapGenerator;
    
    private Camera mainCamera;
    
    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        if (mapGenerator == null)
        {
            mapGenerator = FindObjectOfType<MapGenerator>();
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector3 movement = Vector3.zero;
        
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            movement.y += 1;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            movement.y -= 1;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            movement.x += 1;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            movement.x -= 1;

        if (movement.magnitude > 0)
        {
            movement.Normalize();
            Vector3 newPosition = transform.position + movement * (moveSpeed * Time.deltaTime);
            
            ClampPosition(newPosition);
        }
    }

    private void HandleZoom()
    {
        float scrollDelta = Input.mouseScrollDelta.y;
        if (scrollDelta == 0) return;

        // Ajuster la taille orthographique avec le scroll de la souris
        float newSize = mainCamera.orthographicSize - scrollDelta * zoomSpeed;
        mainCamera.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);

        // Recalculer les limites de position après le zoom
        ClampPosition(transform.position);
    }

    private void ClampPosition(Vector3 newPosition)
    {
        float currentWidth = mapGenerator.mapWidth + 
            ((mapGenerator.GetLevels().Count - 1) * MapGenerator.SIZE_INCREASE_PER_LEVEL);
        float currentHeight = mapGenerator.mapHeight + 
            ((mapGenerator.GetLevels().Count - 1) * MapGenerator.SIZE_INCREASE_PER_LEVEL);

        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        float halfWidth = (currentWidth / 2f) - (cameraWidth / 2f);
        float halfHeight = (currentHeight / 2f) - (cameraHeight / 2f);

        newPosition.x = Mathf.Clamp(newPosition.x, -halfWidth, halfWidth);
        newPosition.y = Mathf.Clamp(newPosition.y, -halfHeight, halfHeight);
        newPosition.z = transform.position.z;

        transform.position = newPosition;
    }
}