using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PaletteFromSprite : EditorWindow
{
	private static Texture2D paletteSprite;
	private static Texture2D palette;
	private static int precision = 1;
	private bool replaced;
	private List<Color> colors;
	private int height;
	private int width;

	[MenuItem("Window/bubucha/Palette From Sprite")]
	private static void Init()
	{
		var window = GetWindowWithRect<PaletteFromSprite>(new Rect(0, 0, 1000, 750));
		window.Show();
	}

	void OnGUI()
	{
		paletteSprite = (Texture2D) EditorGUI.ObjectField(new Rect(3, 3, 250, 250),
			"Color Pallete: ",
			paletteSprite,
			typeof(Texture2D), false);
		precision = EditorGUI.IntField(new Rect(300, 3, 200, 25), "Precision: ", precision);
		if (GUI.Button(new Rect(5, 700, position.width - 10, 20), "Extract Palette"))
		{
			if (paletteSprite)
			{
				ExtractPalette();
				replaced = true;
			}
		}

		if (replaced)
		{
			EditorGUI.PrefixLabel(new Rect(25, 300, 100, 15), 0, new GUIContent("Result:"));
			EditorGUI.DrawPreviewTexture(new Rect(25, 325, width * 5, height * 5), palette);
			if (GUI.Button(new Rect(5, 650, position.width - 10, 20), "Save"))
			{
				var path = EditorUtility.SaveFilePanel("Save Palette",
					$"{Application.dataPath}/Assets/Palettes", "palette.png", "png");
				if (!string.IsNullOrEmpty(path))
				{
					var pngData = palette.EncodeToPNG();
					if (pngData != null)
					{
						File.WriteAllBytes(path, pngData);
						AssetDatabase.Refresh();
					}
				}
			}
		}
	}

	private void ExtractPalette()
	{
		colors = new List<Color>();
		for (var y = 0; y < paletteSprite.height; y += precision)
		{
			for (var x = 0; x < paletteSprite.width; x += precision)
			{
				var c = paletteSprite.GetPixel(x, y);
				if (!colors.Contains(c))
				{
					colors.Add(c);
				}
			}
		}

		var colorsCount = colors.Count;
		height = Mathf.CeilToInt(Mathf.Sqrt(colorsCount));
		width = Mathf.CeilToInt((float) colorsCount / height);
		palette = new Texture2D(width,
			height,
			paletteSprite.format,
			false, true) {filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp};
		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				var index = y * width + x;
				var color = Color.clear;
				if (index < colorsCount)
				{
					color = colors[index];
				}

				palette.SetPixel(x, y, color);
			}
		}

		palette.Apply();
	}
}