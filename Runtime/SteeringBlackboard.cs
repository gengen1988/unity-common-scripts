public class SteeringBlackboard : BlackboardComponent<SteeringField>
{
}

public enum SteeringField
{
    HasTarget,
    TargetPosition,
    TargetVelocity,
    FriendsNearby,
}