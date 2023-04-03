using System;


public class Wait
{
	bool done;
	event Action ready;

	public void Ready()
	{
		done = true;
		ready?.Invoke();
		ready = null;
	}

	public void WhenReady(Action action)
	{
		if (done) action();
		else ready += action;
	}
}