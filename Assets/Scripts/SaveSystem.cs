using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static SceneObjects;

public static class SaveSystem
{
	private const string SAVE_VERSION = "0.1";
	private static readonly string GAME_SAVE_FOLDER_PATH = Application.persistentDataPath + "/saves";
	private static readonly GameSettings GAME_SETTINGS = new GameSettings();
	private static List<UILoadData> uiGameSaves;
	public static GameSave currentGameSave;

	public struct UILoadData
	{
		public string filePath;
		public DateTime lastChanged;
	}

	[Serializable]
	public abstract class SaveData
	{
		public string saveVersion;
		public DateTime lastChanged;

		public void Save(bool saveDate = true)
		{
			var saveData = GetSelfData();
			saveData.saveVersion = SAVE_VERSION;
			if (saveDate)
			{
				saveData.lastChanged = DateTime.Now;
			}

			var jsonData = JsonUtility.ToJson(saveData, true);
			File.WriteAllText(GetSavePath(), jsonData);
		}

		public virtual void Load()
		{
			var savePath = GetSavePath();
			if (File.Exists(savePath))
			{
				var jsonData = File.ReadAllText(GetSavePath());
				JsonUtility.FromJsonOverwrite(jsonData, this);
			}

			ProcessSaveVersion();
		}

		private void ProcessSaveVersion()
		{
			var saveData = GetSelfData();
			if (saveData.saveVersion != SAVE_VERSION)
			{
				if (saveData.saveVersion == null)
				{
					saveData.saveVersion = "0";
				}

				var versionNumbers = saveData.saveVersion.Split('.').Select(v =>
				{
					var success = int.TryParse(v, out var result);
					if (!success)
					{
						throw new ArgumentException("Invalid Save Version, cannot parse");
					}

					return result;
				}).ToList();

//				if (VersionSmallerThan("1.0", versionNumbers))
//				{
//				}

				Save(false);
			}
		}

		private bool VersionSmallerThan(string versionToCompare, List<int> realVersionNumbers)
		{
			var versionNumbers = versionToCompare.Split('.').Select(v =>
			{
				var success = int.TryParse(v, out var result);
				if (!success)
				{
					throw new ArgumentException("Invalid Game Version, cannot parse");
				}

				return result;
			}).ToList();
			var i = 0;
			for (; i < versionNumbers.Count; i++)
			{
				int realNumber;
				if (realVersionNumbers.Count > i)
				{
					realNumber = realVersionNumbers[i];
				}
				else
				{
					return true;
				}

				var compareNumber = versionNumbers[i];
				if (realNumber != compareNumber)
				{
					return realNumber < compareNumber;
				}
			}

			return false;
		}

		protected abstract SaveData GetSelfData();

		protected abstract string GetSavePath();
	}

	[Serializable]
	public class GameSettings : SaveData
	{
		private static readonly string GAME_SETTINGS_SAVE_PATH = Application.persistentDataPath + "/gameSettings.dat";

		public override void Load()
		{
			base.Load();
			Debug.Log(GAME_SETTINGS_SAVE_PATH);
		}

		protected override SaveData GetSelfData()
		{
			return GAME_SETTINGS;
		}

		protected override string GetSavePath()
		{
			return GAME_SETTINGS_SAVE_PATH;
		}
	}

	[Serializable]
	public class GameSave : SaveData
	{
		public string filePath;
		public Player.PlayerData playerData;
		public string currentLevelName;
		public List<LevelState> levelStates = new List<LevelState>();
		public GlobalVariables globalVariables;

		protected override SaveData GetSelfData()
		{
			return this;
		}

		protected override string GetSavePath()
		{
			return filePath;
		}

		public LevelState GetLevelState(string levelName)
		{
			var levelState = levelStates.FirstOrDefault(l => l.levelName == levelName);
			if (levelState == null)
			{
				levelState = new LevelState(levelName);
				levelStates.Add(levelState);
			}

			return levelState;
		}

		public void UpdateLevelState(bool includeEverything)
		{
			var levelState = GameLogic.Instance.GetLevelState(includeEverything);
			var existingLevelState = levelStates.FirstOrDefault(l => l.levelName == levelState.levelName);
			if (existingLevelState != null)
			{
				levelStates.Remove(existingLevelState);
			}

			levelStates.Add(levelState);
		}

		public UILoadData ToUILoadData()
		{
			return new UILoadData
			{
				filePath = filePath,
				lastChanged = lastChanged
			};
		}
	}

	public static void LoadData()
	{
		GAME_SETTINGS.Load();

		uiGameSaves = new List<UILoadData>();
		if (Directory.Exists(GAME_SAVE_FOLDER_PATH))
		{
			var files = Directory.GetFiles(GAME_SAVE_FOLDER_PATH);
			foreach (var filePath in files)
			{
				var gameSave = new GameSave {filePath = filePath};
				gameSave.Load();
				AddUISave(gameSave);
			}
		}
		else
		{
			Directory.CreateDirectory(GAME_SAVE_FOLDER_PATH);
		}
	}

	public static void LoadSave(string filePath)
	{
		currentGameSave = new GameSave {filePath = filePath};
		currentGameSave.Load();
	}

	public static void NewGame()
	{
		//TODO
		const string startingLevel = "";
		currentGameSave = new GameSave
		{
			currentLevelName = startingLevel,
			playerData = new Player.PlayerData("Player", Vector2Int.zero),
			globalVariables = new GlobalVariables(new GlobalVariables.ScavengerAssociationVariables())
		};
	}

	public static void Save()
	{
		if (!Directory.Exists(GAME_SAVE_FOLDER_PATH))
		{
			Directory.CreateDirectory(GAME_SAVE_FOLDER_PATH);
		}

		var filePath = $@"{DateTime.Now.Ticks}.dat";
		currentGameSave.filePath = filePath;
		currentGameSave.currentLevelName = GameLogic.Instance.CurrentLevelName();
		currentGameSave.playerData = (Player.PlayerData) Player.Instance.GetUnitData();
		currentGameSave.Save();
		AddUISave(currentGameSave);
	}

	public static string GetLatestSave()
	{
		var latestSave = uiGameSaves.OrderByDescending(g => g.lastChanged).FirstOrDefault();
		return latestSave.filePath;
	}

	public static bool HasAnySaves()
	{
		return uiGameSaves.Count > 0;
	}

	private static void AddUISave(GameSave gameSave)
	{
		uiGameSaves.Add(gameSave.ToUILoadData());
		//TODO Init on UI
	}
}