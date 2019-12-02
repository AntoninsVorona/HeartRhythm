using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SpriteColorChanger : EditorWindow
{
	private static Sprite changeColorsSprite;
	private static Texture2D colorPalette;
	private static Texture2D replacedTexture;
	private bool replaced;
	private List<Color> colors;
	private Dictionary<Color, Color> cachedColors;
	private int height;
	private int width;

	[MenuItem("Window/bubucha/Sprite Color Changer")]
	private static void Init()
	{
		var window = GetWindowWithRect<SpriteColorChanger>(new Rect(0, 0, 1000, 750));
		window.Show();
	}

	void OnGUI()
	{
		changeColorsSprite = (Sprite) EditorGUI.ObjectField(new Rect(3, 3, 250, 250),
			"Sprite or Sheet: ",
			changeColorsSprite,
			typeof(Sprite), false);
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
					}
				}
			}
		}
	}

	private void ReplaceTexture()
	{
		var texture2D = changeColorsSprite.texture;
		CalculateColorPalette();
		cachedColors = new Dictionary<Color, Color>
		{
			{Color.clear, Color.clear}
		};
		height = texture2D.height;
		width = texture2D.width;
		replacedTexture = new Texture2D(width,
			height,
			texture2D.format,
			false, true) {filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp};
		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				var newColor = FindClosestColor(texture2D.GetPixel(x, y));
				replacedTexture.SetPixel(x, y, newColor);
			}
		}

		replacedTexture.Apply();
	}

	private void CalculateColorPalette()
	{
		colors = new List<Color>
		{
			Color.clear
		};
		for (var m = 0; m < colorPalette.mipmapCount; m++)
		{
			var c = colorPalette.GetPixels(m);
			foreach (var color in c)
			{
				if (!colors.Contains(color))
				{
					colors.Add(color);
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