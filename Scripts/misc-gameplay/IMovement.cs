public interface IMovement<TContext>
{
    public void Move(TContext context, float deltaTime);
}