using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
	private const string PATH = "Items";
	private readonly Dictionary<string, Item> itemCache = new Dictionary<string, Item>();

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
			return;
		}
	}

	public Item GetItemByName(string itemName)
	{
		if (itemCache.ContainsKey(itemName))
		{
			return itemCache[itemName];
		}

		var item = Resources.Load<Item>($"{PATH}/{itemName}");
		itemCache.Add(itemName, item);
		if (!item)
		{
			Debug.LogError($"Item with name {itemName} doesn't exist!");
			return null;
		}

		return item;
	}

	public static ItemManager Instance { get; private set; }
}