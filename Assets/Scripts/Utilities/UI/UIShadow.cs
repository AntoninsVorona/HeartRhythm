using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Shadow", 14)]
[RequireComponent(typeof(RectTransform))]
public class UIShadow : BaseMeshEffect
{
	[SerializeField]
	private Vector2 mEffectDistance = new Vector2(1f, -1f);

	[SerializeField]
	private bool mUseGlobalOffset = true;

	[SerializeField]
	private bool mUseGraphicAlpha = true;

	protected UIShadow()
	{
	}

#if UNITY_EDITOR
	protected override void OnValidate()
	{
		EffectDistance = mEffectDistance;
		base.OnValidate();
	}
#endif

	private Color EffectColor { get; } = new Color(0f, 0f, 0f, 0.5f);

	private Vector2 EffectDistance
	{
		get => mEffectDistance;
		set
		{
			if (value.x > 600)
				value.x = 600;
			if (value.x < -600)
				value.x = -600;

			if (value.y > 600)
				value.y = 600;
			if (value.y < -600)
				value.y = -600;

			if (mEffectDistance == value)
				return;

			mEffectDistance = value;

			if (graphic != null)
				graphic.SetVerticesDirty();
		}
	}

	protected void ApplyShadowZeroAlloc(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
	{
		UIVertex vt;

		var neededCpacity = verts.Count * 2;
		if (verts.Capacity < neededCpacity)
			verts.Capacity = neededCpacity;

		for (int i = start; i < end; ++i)
		{
			vt = verts[i];
			verts.Add(vt);

			Vector3 v = vt.position;
			v.x += x;
			v.y += y;
			vt.position = v;
			var newColor = color;
			if (mUseGraphicAlpha)
				newColor.a = (byte) ((newColor.a * verts[i].color.a) / 255);
			vt.color = newColor;
			verts[i] = vt;
		}
	}

	private void OnRenderObject()
	{
		if (mUseGlobalOffset)
			graphic.SetVerticesDirty();
	}

	protected void ApplyShadow(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
	{
		var neededCpacity = verts.Count * 2;
		if (verts.Capacity < neededCpacity)
			verts.Capacity = neededCpacity;

		ApplyShadowZeroAlloc(verts, color, start, end, x, y);
	}

	public override void ModifyMesh(VertexHelper vh)
	{
		if (!IsActive())
		{
			return;
		}

		List<UIVertex> verts = new List<UIVertex>();
		vh.GetUIVertexStream(verts);

		Text foundtext = GetComponent<Text>();

		float best_fit_adjustment = 1f;

		if (foundtext && foundtext.resizeTextForBestFit)
		{
			best_fit_adjustment = (float) foundtext.cachedTextGenerator.fontSizeUsedForBestFit /
			                      (foundtext.resizeTextMaxSize - 1); //max size seems to be exclusive 
		}

		float distanceX = EffectDistance.x * best_fit_adjustment;
		float distanceY = EffectDistance.y * best_fit_adjustment;
		Vector2 pos = new Vector2(distanceX, distanceY);

		if (mUseGlobalOffset)
			pos = Rotate(pos, -((RectTransform) transform).eulerAngles.z);

		int start = 0;
		ApplyShadow(verts, EffectColor, start, verts.Count, pos.x, pos.y);

		vh.Clear();
		vh.AddUIVertexTriangleStream(verts);
	}

	private static Vector2 Rotate(Vector2 v, float degrees)
	{
		var radians = degrees * Mathf.Deg2Rad;
		var sin = Mathf.Sin(radians);
		var cos = Mathf.Cos(radians);

		var tx = v.x;
		var ty = v.y;

		return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
	}
}