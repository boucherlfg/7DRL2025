using UnityEngine;

public class MapVisualizer : MonoBehaviour
{
    public Vector3 baseMapOffset = Vector3.zero;
    public Vector3 savedMapOffset = new Vector3(20, 0, 0); // Décalage pour afficher la carte sauvegardée à côté de la carte de base

    private void OnDrawGizmos()
    {
        if (GameManager.Instance != null)
        {
            // Dessiner la carte de base
            GameManager.Instance.DrawMap(baseMapOffset);

            // Dessiner la carte sauvegardée
            GameManager.Instance.DrawMap(savedMapOffset);
        }
    }
}