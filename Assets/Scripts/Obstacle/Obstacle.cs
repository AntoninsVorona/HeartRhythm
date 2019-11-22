public class Obstacle : Unit
{
	protected override void InteractWithObject(Unit unit)
	{
	}

	public override void Die()
	{
		GameLogic.Instance.currentSceneObjects.currentWorld.UnoccupyTargetTile(currentPosition);
		Destroy(gameObject);
	}
}