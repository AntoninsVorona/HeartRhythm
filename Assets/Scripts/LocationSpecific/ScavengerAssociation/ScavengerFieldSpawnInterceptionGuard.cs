using UnityEngine;

public class ScavengerFieldSpawnInterceptionGuard : MonoBehaviour
{
	private SceneObjects sceneObjects;

	private void Start()
	{
		sceneObjects = GetComponent<SceneObjects>();
		const string interceptionGuardName = "InterceptionGuard";
		var interceptionGuardData = GameSessionManager.Instance.currentLevelState.GetDataByName(interceptionGuardName);
		if (interceptionGuardData != null)
		{
			var interceptionGuard = (InterceptionGuard) sceneObjects.currentMobManager.SpawnMob(interceptionGuardName,
				interceptionGuardData.currentPosition,
				new Mob.MovementSettings {moveDuringPeaceMode = false});
			interceptionGuard.Rotate();
		}
	}
}