using UnityEngine;
using UnityEngine.EventSystems;

public class CustomStandaloneInputModule : StandaloneInputModule
{
    public bool IsPointerOverUI()
    {
        PointerEventData pointerEventData = GetLastPointerEventData(-1);
        if (pointerEventData != null)
        {
            var obj = pointerEventData.pointerEnter;
            return obj != null && obj.layer == LayerMask.NameToLayer("UI");
        }

        return false;
    }

    public GameObject CurrentHit()
    {
        PointerEventData pointerEventData = GetLastPointerEventData(-1);
        if (pointerEventData != null)
        {
            return pointerEventData.pointerEnter;
        }

        return null;
    }
    
    public GameObject CurrentHitWithLayer(LayerMask layer)
    {
        PointerEventData pointerEventData = GetLastPointerEventData(-1);
        if (pointerEventData != null)
        {
            var obj = pointerEventData.pointerEnter;
            if (obj != null && LayerMask.GetMask(LayerMask.LayerToName(obj.layer)) == layer)
            {
                return obj;
            }
        }

        return null;
    }

}