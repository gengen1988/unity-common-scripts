public interface IBrain<TContext>
{
    public void Refresh(TContext context);
}

public interface IBrainManager<TContext>
{
    public void AddBrain(IBrain<TContext> brain);
    public void RemoveBrain(IBrain<TContext> brain);
}

public interface IBrainObserve<TContext>
{
    public void Observe(TContext context);
}

public interface IBrainOrient<TContext>
{
    public void Orient(TContext context);
}

public interface IBrainDecide<TContext>
{
    public void Decide(TContext context);
}