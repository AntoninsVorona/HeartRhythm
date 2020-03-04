using UnityEngine;

public abstract class BattleRules : MonoBehaviour
{
	protected SceneObjects sceneObjects;

	protected virtual void Awake()
	{
		Instance = this;
		sceneObjects = GetComponent<SceneObjects>();
	}

	protected virtual void Start()
	{
		var observer = new Observer(this, OnBeatDone);
		sceneObjects.beatListeners.Add(observer);
	}

	protected abstract void OnBeatDone();

	public static BattleRules Instance { get; private set; }
}