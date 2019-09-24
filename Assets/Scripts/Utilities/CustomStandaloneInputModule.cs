using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomStandaloneInputModule : StandaloneInputModule
{
	public bool IsPointerOverUI()
	{
		var pointerEventData = GetLastPointerEventData(-1);
		if (pointerEventData != null)
		{
			var obj = pointerEventData.pointerCurrentRaycast.gameObject;
			return obj != null && obj.layer == LayerMask.NameToLayer("UI");
		}

		return false;
	}

	public GameObject CurrentHit()
	{
		var pointerEventData = GetLastPointerEventData(-1);
		var obj = pointerEventData?.pointerCurrentRaycast.gameObject;
		return obj;
	}

	public GameObject CurrentHitWithLayer(LayerMask layer)
	{
		var pointerEventData = GetLastPointerEventData(-1);
		if (pointerEventData != null)
		{
			var obj = pointerEventData.pointerCurrentRaycast.gameObject;
			if (obj != null && LayerMask.GetMask(LayerMask.LayerToName(obj.layer)) == layer)
			{
				return obj;
			}
		}

		return null;
	}
}