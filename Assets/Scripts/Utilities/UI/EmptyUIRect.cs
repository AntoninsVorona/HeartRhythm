﻿namespace UnityEngine.UI
{
	public class EmptyUIRect : Graphic
	{
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
		}
	}
}