using System;

[Serializable]
public class Observer
{
	public Action action;

	public Observer(Action action)
	{
		this.action = action;
	}

	public void Notify()
	{
		action();
	}
}