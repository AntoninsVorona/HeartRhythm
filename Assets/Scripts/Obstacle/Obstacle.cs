public class Obstacle : Unit
{
	protected override void InteractWithObject(Unit unit)
	{
	}

	public override void Die()
	{
		World.Instance.UnoccupyTargetTile(currentPosition);
		Destroy(gameObject);
	}
}