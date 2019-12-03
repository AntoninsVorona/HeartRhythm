using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SpriteColorChanger : EditorWindow
{
	private static Texture2D changeColorsSprite;
	private static Texture2D colorPalette;
	private static Texture2D replacedTexture;
	private static List<Color> colors;
	private static bool replaced;
	private static Dictionary<Color, Color> cachedColors;
	private static int height;
	private static int width;

	[MenuItem("Window/bubucha/Sprite Color Changer")]
	private static void Init()
	{
		var window = GetWindowWithRect<SpriteColorChanger>(new Rect(0, 0, 1000, 750));
		window.Show();
	}

	void OnGUI()
	{
		changeColorsSprite = (Texture2D) EditorGUI.ObjectField(new Rect(3, 3, 250, 250),
			"Sprite or Sheet: ",
			changeColorsSprite,
			typeof(Texture2D), false);
		colorPalette = (Texture2D) EditorGUI.ObjectField(new Rect(3, 153, 250, 250),
			"Color Pallete: ",
			colorPalette,
			typeof(Texture2D), false);
		if (GUI.Button(new Rect(5, 700, position.width - 10, 20), "Replace Pixels"))
		{
			if (changeColorsSprite && colorPalette)
			{
				ReplaceTexture();
				replaced = true;
			}
		}

		if (replaced)
		{
			EditorGUI.PrefixLabel(new Rect(25, 300, 100, 15), 0, new GUIContent("Result:"));
			EditorGUI.DrawPreviewTexture(new Rect(25, 325, width * 5, height * 5), replacedTexture);
			if (GUI.Button(new Rect(5, 650, position.width - 10, 20), "Save"))
			{
				var path = EditorUtility.SaveFilePanel("Save Changed Sprite",
					$"{Application.dataPath}/Assets/ChangedColorPNGs", "changed.png", "png");
				if (!string.IsNullOrEmpty(path))
				{
					var pngData = replacedTexture.EncodeToPNG();
					if (pngData != null)
					{
						File.WriteAllBytes(path, pngData);
						AssetDatabase.Refresh();
					}
				}
			}
		}
	}

	private void ReplaceTexture()
	{
		cachedColors = new Dictionary<Color, Color>();
		height = changeColorsSprite.height;
		width = changeColorsSprite.width;
		CalculateColorPalette();
		replacedTexture = new Texture2D(width,
			height,
			changeColorsSprite.format,
			false, true) {filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp};
		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				var pixel = changeColorsSprite.GetPixel(x, y);
				var newColor = Mathf.Approximately(pixel.a, 0) ? Color.clear : FindClosestColor(pixel);
				replacedTexture.SetPixel(x, y, newColor);
			}
		}

		replacedTexture.Apply();
	}

	private void CalculateColorPalette()
	{
		colors = new List<Color>();
		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				var c = colorPalette.GetPixel(x, y);
				if (!colors.Contains(c))
				{
					colors.Add(c);
				}
			}
		}
	}

	private Color FindClosestColor(Color newColor)
	{
		if (!cachedColors.ContainsKey(newColor))
		{
			cachedColors.Add(newColor, ClosestColor(newColor));
		}

		return cachedColors[newColor];
	}

	private Color ClosestColor(Color target)
	{
		return colors.OrderBy(n => ColorDiff(n, target)).First();
	}

	private float ColorDiff(Color c1, Color c2)
	{
		var red = c1.r - c2.r;
		var green = c1.g - c2.g;
		var blue = c1.b - c2.b;
		return Mathf.Sqrt(red * red
		                  + green * green
		                  + blue * blue);
	}
}