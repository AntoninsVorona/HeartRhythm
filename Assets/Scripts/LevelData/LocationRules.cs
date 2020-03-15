using UnityEngine;

public abstract class LocationRules : MonoBehaviour
{
	protected SceneObjects sceneObjects;

	protected virtual void Awake()
	{
		Instance = this;
		sceneObjects = GetComponent<SceneObjects>();
	}

	protected virtual void Start()
	{
		CreateObservers();
	}

	protected virtual void CreateObservers()
	{
		var observer = new Observer(this, OnBeatDone);
		sceneObjects.beatListeners.Add(observer);
	}

	protected abstract void OnBeatDone();

	public static LocationRules Instance { get; private set; }
}