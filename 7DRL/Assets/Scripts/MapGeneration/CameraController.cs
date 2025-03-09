using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 15f;
    public MapGenerator mapGenerator;
    private bool _canControl = true;
    private Camera mainCamera;
    
    private void Start()
    {
        mainCamera = Camera.main;
        if (mapGenerator == null)
        {
            mapGenerator = FindFirstObjectByType<MapGenerator>();
        }
        
        // Ajouter un délai pour laisser le temps à la map de se générer
        Invoke(nameof(SetInitialZoom), 0.1f);
        MapNode.Entered.AddListener(OnMapNodeEntered);
        MapNode.Exited.AddListener(OnMapNodeExited);
    }

    private void OnMapNodeExited()
    {
        Invoke(nameof(SetInitialZoom), 0.1f);
        _canControl = true;
    }

    private void OnMapNodeEntered(NodeType arg0)
    {
        _canControl = false;
    }

    private void SetInitialZoom()
    {
        if (mapGenerator == null || mainCamera == null) return;

        // Trouver les points les plus éloignés
        Vector2 minPoint = Vector2.zero;
        Vector2 maxPoint = Vector2.zero;
        bool firstNode = true;

        foreach (var level in mapGenerator.GetLevels())
        {
            foreach (var node in level.points)
            {
                if (node == null) continue;

                Vector3 nodePos = node.transform.position;
                if (firstNode)
                {
                    minPoint = maxPoint = new Vector2(nodePos.x, nodePos.y);
                    firstNode = false;
                }
                else
                {
                    minPoint.x = Mathf.Min(minPoint.x, nodePos.x);
                    minPoint.y = Mathf.Min(minPoint.y, nodePos.y);
                    maxPoint.x = Mathf.Max(maxPoint.x, nodePos.x);
                    maxPoint.y = Mathf.Max(maxPoint.y, nodePos.y);
                }
            }
        }

        // Ajouter une marge
        float margin = 2f;
        float width = (maxPoint.x - minPoint.x) + (margin * 2);
        float height = (maxPoint.y - minPoint.y) + (margin * 2);

        // Calculer le zoom nécessaire
        float targetSize = Mathf.Max(
            height / 2f,
            width / (2f * mainCamera.aspect)
        );

        // Appliquer le zoom avec une marge supplémentaire de 10%
        mainCamera.orthographicSize = Mathf.Min(maxZoom, targetSize * 1.1f);

        // Centrer la caméra
        Vector2 center = (maxPoint + minPoint) * 0.5f;
        transform.position = new Vector3(center.x, center.y, transform.position.z);
    }

    private void Update()
    {
        //if(!_canControl) return;
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
            Vector3 newPosition = mainCamera.transform.position + movement * (moveSpeed * Time.deltaTime);
            
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
        if (mapGenerator == null || mainCamera == null) return;

        // Calculer la taille visible de la caméra
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Trouver les points les plus éloignés sur la carte
        Vector2 minPoint = Vector2.zero;
        Vector2 maxPoint = Vector2.zero;
        bool firstNode = true;

        foreach (var level in mapGenerator.GetLevels())
        {
            foreach (var node in level.points)
            {
                if (node == null) continue;

                Vector3 nodePos = node.transform.position;
                if (firstNode)
                {
                    minPoint = maxPoint = new Vector2(nodePos.x, nodePos.y);
                    firstNode = false;
                }
                else
                {
                    minPoint.x = Mathf.Min(minPoint.x, nodePos.x);
                    minPoint.y = Mathf.Min(minPoint.y, nodePos.y);
                    maxPoint.x = Mathf.Max(maxPoint.x, nodePos.x);
                    maxPoint.y = Mathf.Max(maxPoint.y, nodePos.y);
                }
            }
        }

        // Ajouter une marge
        float margin = 2f;
        minPoint -= Vector2.one * margin;
        maxPoint += Vector2.one * margin;

        // Calculer la taille réelle de la carte
        float currentWidth = maxPoint.x - minPoint.x;
        float currentHeight = maxPoint.y - minPoint.y;
        Vector2 mapCenter = (maxPoint + minPoint) * 0.5f;

        // Calculer les limites effectives en tenant compte de la vue caméra
        float maxX = (currentWidth - cameraWidth) * 0.5f;
        float maxY = (currentHeight - cameraHeight) * 0.5f;

        // Empêcher le déplacement si la caméra est plus grande que la carte
        if (cameraWidth >= currentWidth)
        {
            newPosition.x = mapCenter.x;
        }
        else
        {
            newPosition.x = Mathf.Clamp(newPosition.x, mapCenter.x - maxX, mapCenter.x + maxX);
        }

        if (cameraHeight >= currentHeight)
        {
            newPosition.y = mapCenter.y;
        }
        else
        {
            newPosition.y = Mathf.Clamp(newPosition.y, mapCenter.y - maxY, mapCenter.y + maxY);
        }

        newPosition.z = transform.position.z;
        mainCamera.transform.position = newPosition;
    }
}