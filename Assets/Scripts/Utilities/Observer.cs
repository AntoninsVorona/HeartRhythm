using System;

[Serializable]
public class Observer
{
	public UnityEngine.Object owner;
	public Action actionBegin;
	public Action actionEnd;
	
	public Observer(UnityEngine.Object owner, Action action)
	{
		this.owner = owner;
		actionBegin = action;
	}
	
	public Observer(UnityEngine.Object owner, Action actionBegin, Action actionEnd)
	{
		this.owner = owner;
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