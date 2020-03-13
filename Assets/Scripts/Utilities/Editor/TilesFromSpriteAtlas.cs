using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TilesFromSpriteAtlas : EditorWindow
{
	public RuleTile prefab;
	public List<Sprite> sprites;

	[MenuItem("Window/bubucha/TilesFromSpriteAtlas")]
	private static void Init()
	{
		GetWindow(typeof(TilesFromSpriteAtlas)).Show();
	}

	private void OnGUI()
	{
		var so = new SerializedObject(this);
		var prefabProperty = so.FindProperty("prefab");
		EditorGUILayout.PropertyField(prefabProperty, false);
		var spritesProperty = so.FindProperty("sprites");
		EditorGUILayout.PropertyField(spritesProperty, true);
		so.ApplyModifiedProperties();
		if (GUILayout.Button("Convert"))
		{
			var counter = -1;
			var assetPath = AssetDatabase.GetAssetPath(prefab);
			var directoryName = Path.GetDirectoryName(assetPath);
			var prefabName = prefab.name;
			sprites.ForEach(s =>
			{
				var tile = Instantiate(prefab);
				tile.m_DefaultSprite = s;
				if (tile.m_TilingRules.Count > 0)
				{
					tile.m_TilingRules.First().m_Sprites[0] = s;
				}

				AssetDatabase.CreateAsset(tile, $"{directoryName}\\{prefabName}{++counter}.asset");
			});

			AssetDatabase.SaveAssets();
		}
	}
}