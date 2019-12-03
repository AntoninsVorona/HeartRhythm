using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class PaletteCreator : EditorWindow
{
	[Serializable]
	public class PaletteColor
	{
		public Color colorFrom;
		public Color colorTo;
		public int shadeCount;

		public PaletteColor()
		{
			colorFrom = Color.black;
			colorTo = Color.white;
			shadeCount = 16;
		}
	}

	private Texture2D result;

	[SerializeField]
	public List<PaletteColor> paletteColors = new List<PaletteColor>
	{
		new PaletteColor()
	};

	private bool constructed;
	private int width;
	private int height;

	[MenuItem("Window/bubucha/Palette Creator")]
	private static void Init()
	{
		var window = GetWindowWithRect<PaletteCreator>(new Rect(0, 0, 1000, 750));
		window.Show();
	}

	void OnGUI()
	{
		InitList();

		if (paletteColors.Count > 0)
		{
			if (GUI.Button(new Rect(5, 700, position.width - 10, 20), "Construct Palette"))
			{
				ConstructPalette();
				constructed = true;
			}
		}

		if (constructed)
		{
			EditorGUI.PrefixLabel(new Rect(25, 250, 100, 15), 0, new GUIContent("Result:"));
			EditorGUI.DrawPreviewTexture(new Rect(25, 300, width * 30, height * 30), result);
			if (GUI.Button(new Rect(5, 650, position.width - 10, 20), "Save"))
			{
				var path = EditorUtility.SaveFilePanel("Save Palette", $"{Application.dataPath}/Assets/Palettes", "Palette.png", "png");
				if (!string.IsNullOrEmpty(path))
				{
					var pngData = result.EncodeToPNG();
					if (pngData != null)
					{
						File.WriteAllBytes(path, pngData);
						AssetDatabase.Refresh();
					}
				}
			}
		}
	}

	private void InitList()
	{
		ScriptableObject target = this;
		SerializedObject so = new SerializedObject(target);
		SerializedProperty palette = so.FindProperty("paletteColors");

		EditorGUILayout.PropertyField(palette, true); // True means show children
		so.ApplyModifiedProperties();
	}

	private void ConstructPalette()
	{
		width = paletteColors.Max(p => p.shadeCount);
		height = paletteColors.Count;
		result = new Texture2D(width,
			height,
			TextureFormat.RGBA32,
			false, true) {filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp};

		Debug.Log(height);
		Debug.Log(width);
		for (var y = 0; y < height; y++)
		{
			var color = ShadeColor(paletteColors[y]);
			for (var x = 0; x < width; x++)
			{
				result.SetPixel(x, y, color[x]);
			}
		}

		result.Apply();
	}

	private Color[] ShadeColor(PaletteColor color)
	{
		var shadedColors = new Color[width];
		for (var i = 0; i < color.shadeCount; i++)
		{
			shadedColors[i] = Color.Lerp(color.colorFrom, color.colorTo, (float) i / (color.shadeCount - 1));
		}

		for (var i = color.shadeCount; i < width; i++)
		{
			shadedColors[i] = Color.clear;
		}

		return shadedColors;
	}
}