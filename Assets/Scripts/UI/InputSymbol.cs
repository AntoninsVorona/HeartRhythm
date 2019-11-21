using UnityEngine;
using UnityEngine.UI;

public class InputSymbol : MonoBehaviour
{
    [SerializeField]
    private Image image;

    public void Initialize(Sprite sprite)
    {
        UpdateSymbol(sprite);
        gameObject.SetActive(true);
    }
    
    public void UpdateSymbol(Sprite sprite)
    {
        image.sprite = sprite;
    }
}