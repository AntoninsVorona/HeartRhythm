using UnityEngine;
using UnityEngine.UI;

public class InputSymbol : MonoBehaviour
{
    [SerializeField]
    protected Image symbolImage;

    public virtual void Initialize(Sprite sprite)
    {
        UpdateSymbol(sprite);
        gameObject.SetActive(true);
    }
    
    public virtual void UpdateSymbol(Sprite sprite)
    {
        symbolImage.sprite = sprite;
    }
}