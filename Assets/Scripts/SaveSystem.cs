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

	private static readonly GameSettings GAME_SETTINGS =
		new GameSettings(Screen.currentResolution.height, Screen.currentResolution.width, Screen.fullScreenMode, 60);

	public static List<UILoadData> uiGameSaves;
	public static GameSave currentGameSave;

	public struct UILoadData
	{
		public string filePath;
		public DateTime lastChanged;
	}

	[Serializable]
	public struct JsonDateTime
	{
		public long value;

		public static implicit operator DateTime(JsonDateTime jdt)
		{
			return DateTime.FromFileTimeUtc(jdt.value);
		}

		public static implicit operator JsonDateTime(DateTime dt)
		{
			return new JsonDateTime {value = dt.ToFileTimeUtc()};
		}
	}

	[Serializable]
	public abstract class SaveData
	{
		public string saveVersion;
		public JsonDateTime lastChanged;

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
				if (string.IsNullOrEmpty(saveData.saveVersion))
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
		public int width;
		public int height;
		public FullScreenMode fullScreenMode;
		public int targetFrameRate;

		public GameSettings(int width, int height, FullScreenMode fullScreenMode, int targetFrameRate)
		{
			this.width = width;
			this.height = height;
			this.fullScreenMode = fullScreenMode;
			this.targetFrameRate = targetFrameRate;
		}

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
		[NonSerialized]
		public string filePath;

		public Player.PlayerData playerData;
		public string currentLevelName;
		public List<LevelState> levelStates = new List<LevelState>();
		public GlobalVariables globalVariables;
		public string databaseData;

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
			var levelState = GameSessionManager.Instance.GetLevelState(includeEverything);
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
		ApplySettings();

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
		const string startingLevel = "MainCharacterRoom";
		MakeDefaultStartingGameSave(startingLevel, 2);
	}

	public static void MakeDefaultStartingGameSave(string startingLevel, int maxDanceMoveSymbols)
	{
		currentGameSave = new GameSave
		{
			currentLevelName = startingLevel,
			playerData = new Player.PlayerData("Player", Vector2Int.zero,
				new Inventory.InventoryData(null, null)),
			globalVariables =
				new GlobalVariables(false, maxDanceMoveSymbols,
					new GlobalVariables.ScavengerAssociationVariables(false, false, false, false)
				)
		};
	}

	public static UILoadData Save()
	{
		if (!Directory.Exists(GAME_SAVE_FOLDER_PATH))
		{
			Directory.CreateDirectory(GAME_SAVE_FOLDER_PATH);
		}

		var filePath = $@"{GAME_SAVE_FOLDER_PATH}/{DateTime.Now.Ticks}.dat";
		currentGameSave.filePath = filePath;
		currentGameSave.currentLevelName = GameSessionManager.Instance.CurrentLevelName();
		currentGameSave.playerData = (Player.PlayerData) Player.Instance.GetUnitData();
		currentGameSave.databaseData = PixelCrushers.DialogueSystem.PersistentDataManager.GetSaveData();
		currentGameSave.Save();
		return AddUISave(currentGameSave);
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

	private static UILoadData AddUISave(GameSave gameSave)
	{
		var uiLoadData = gameSave.ToUILoadData();
		uiGameSaves.Add(uiLoadData);
		return uiLoadData;
	}

	public static void EraseSave(string filePath)
	{
		var uiLoadData = uiGameSaves.First(u => u.filePath == filePath);
		uiGameSaves.Remove(uiLoadData);
		File.Delete(uiLoadData.filePath);
	}

	public static void SetSettings(Resolution resolution, FullScreenMode fullScreenMode, int targetFrameRate)
	{
		GAME_SETTINGS.width = resolution.width;
		GAME_SETTINGS.height = resolution.height;
		GAME_SETTINGS.fullScreenMode = fullScreenMode;
		GAME_SETTINGS.targetFrameRate = targetFrameRate;
		GAME_SETTINGS.Save();
		ApplySettings();
	}

	private static void ApplySettings()
	{
		Screen.SetResolution(GAME_SETTINGS.width, GAME_SETTINGS.height, GAME_SETTINGS.fullScreenMode);
		Application.targetFrameRate = GAME_SETTINGS.targetFrameRate;
	}
}