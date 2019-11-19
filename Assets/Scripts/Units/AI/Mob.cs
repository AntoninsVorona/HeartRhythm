using System;

public class Mob : Unit
{
    public enum TypeOfMovement
    {
        None = 0,
        Constant = 1,
        Random = 2,
        FollowPlayer = 3
    }

    public class MovementSettings
    {
        public TypeOfMovement typeOfMovement;
        public MovementDirectionUtilities.MovementDirection movementDirection;
    }

    public MovementSettings movementSettings;
    public bool initializeSelf = true;

    private void Awake()
    {
        if (initializeSelf)
        {
            MobController.Instance.InitializeMob(this, spawnPoint);
        }
    }

    public void MakeAction()
    {
        switch (movementSettings.typeOfMovement)
        {
            case TypeOfMovement.None:
                break;
            case TypeOfMovement.Constant:
                Move(movementSettings.movementDirection);
                break;
            case TypeOfMovement.Random:
            case TypeOfMovement.FollowPlayer:
                AssignRandomDirection();
                Move(movementSettings.movementDirection);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void AssignRandomDirection()
    {
        movementSettings.movementDirection =
            EnumUtilities.RandomEnumValue<MovementDirectionUtilities.MovementDirection>();
    }

    protected override void Die()
    {
        MobController.Instance.RemoveMob(this);
    }

    protected override void InteractWithObject(Obstacle obstacle)
    {
    }

    protected override void InteractWithObject(Unit unit)
    {
    }
}