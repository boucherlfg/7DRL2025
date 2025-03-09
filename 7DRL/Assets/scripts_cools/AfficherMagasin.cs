using UnityEngine;

public class AfficherMagasin : MonoBehaviour
{
public Item item;
public TMPro.TextMeshPro prix;
public SpriteRenderer image;

    void Start()
    {
        image.sprite = item.sprite;
    }
    public void changeImage(){
    image.sprite = item.sprite;

    }

}
