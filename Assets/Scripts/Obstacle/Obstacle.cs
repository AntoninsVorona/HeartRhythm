using UnityEngine;

public class Obstacle : Unit
{
    public void GetDestroyed()
    {
        Die();
    }

    protected override void InteractWithObject(Unit unit)
    {
    }

    protected override void Die()
    {
        Destroy(gameObject);
    }
}