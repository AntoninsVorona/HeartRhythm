using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneObjects : MonoBehaviour
{
	[Serializable]
	public class LevelState
	{
		public string levelName;
		public List<Unit.UnitData> unitData;

		public LevelState(string levelName)
		{
			this.levelName = levelName;
			unitData = new List<Unit.UnitData>();
		}

		public Unit.UnitData GetDataByName(string identifierName)
		{
			return unitData.FirstOrDefault(u => u.identifierName == identifierName);
		}

		public List<ItemOnGround.ItemOnGroundData> GetItemData()
		{
			return unitData.OfType<ItemOnGround.ItemOnGroundData>().ToList();
		}
	}
	
	public World currentWorld;
	public MobManager currentMobManager;
	public ObstacleManager currentObstacleManager;

	private string levelName;

	public void Initialize(string levelName)
	{
		this.levelName = levelName;
	}
	
	public void Activate()
	{
		gameObject.SetActive(true);
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);
	}

	public LevelState GetLevelState(bool includeEverything)
	{
		var levelState = new LevelState(levelName);
		var allUnitData = new List<Unit.UnitData>();
		if (includeEverything)
		{
			allUnitData.AddRange(currentObstacleManager.GetDataOfUnits());
			allUnitData.AddRange(currentMobManager.GetDataOfUnits());
		}
		else
		{
			allUnitData.AddRange(currentObstacleManager.GetDataOfUnits<ItemOnGround>());
		}

		levelState.unitData = allUnitData;
		return levelState;
	}
}