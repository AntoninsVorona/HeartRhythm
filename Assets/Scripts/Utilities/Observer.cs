using System;

[Serializable]
public class Observer
{
	public Action actionBegin;
	public Action actionEnd;
	
	public Observer(Action action)
	{
		actionBegin = action;
	}
	
	public Observer(Action actionBegin, Action actionEnd)
	{
		this.actionBegin = actionBegin;
		this.actionEnd = actionEnd;
	}

	public void NotifyBegin()
	{
		actionBegin();
	}
	
	public void NotifyEnd()
	{
		actionEnd();
	}
}