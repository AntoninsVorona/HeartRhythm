/// Credit Melang
/// Sourced from - http://forum.unity3d.com/members/melang.593409/

using System.Collections.Generic;
namespace UnityEngine.UI.Extensions
{
    //An outline that looks a bit nicer than the default one. It has less "holes" in the outline by drawing more copies of the effect
    [AddComponentMenu("UI/Effects/Extensions/Nicer Outline")]
	public class NicerOutline : BaseMeshEffect
	{
		[SerializeField]
		private Vector2 mEffectDistance = new Vector2 (1f, -1f);

		[Range(4, 20)]
		public int effectAmount = 8;

		[SerializeField]
		private bool mUseGraphicAlpha = true;

		private readonly List < UIVertex > m_Verts = new List<UIVertex>();

		// Properties
		private Color EffectColor { get; } = new Color (0f, 0f, 0f, 0.5f);

		private Vector2 EffectDistance
		{
			get => mEffectDistance;
			set
			{
				value.x = Mathf.Clamp(value.x, -600, 600);
				value.y = Mathf.Clamp(value.y, -600, 600);

				if (mEffectDistance == value)
					return;

				mEffectDistance = value;
				if (graphic != null)
					graphic.SetVerticesDirty ();
			}
		}

		protected void ApplyShadowZeroAlloc(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
        {
            UIVertex vt;

            var neededCpacity = verts.Count * 2;
            if (verts.Capacity < neededCpacity)
                verts.Capacity = neededCpacity;

            for (int i = start; i < end; ++i)
            {
                vt = verts[i];
                verts.Add(vt);

                Vector3 v = vt.position;
                v.x += x;
                v.y += y;
                vt.position = v;
                var newColor = color;
                if (mUseGraphicAlpha)
                    newColor.a = (byte)((newColor.a * verts[i].color.a) / 255);
                vt.color = newColor;
                verts[i] = vt;
            }
        }

        protected void ApplyShadow(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
        {
            var neededCpacity = verts.Count * 2;
            if (verts.Capacity < neededCpacity)
                verts.Capacity = neededCpacity;

            ApplyShadowZeroAlloc(verts, color, start, end, x, y);
        }


        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive ())
			{
				return;
			}

	        m_Verts.Clear();
            vh.GetUIVertexStream(m_Verts);

            Text foundtext = GetComponent<Text>();

			float best_fit_adjustment = 1f;

			if (foundtext && foundtext.resizeTextForBestFit)
			{
				best_fit_adjustment = (float)foundtext.cachedTextGenerator.fontSizeUsedForBestFit / (foundtext.resizeTextMaxSize-1); //max size seems to be exclusive
			}

			float distanceX = EffectDistance.x * best_fit_adjustment;
			float distanceY = EffectDistance.y * best_fit_adjustment;

			int start = 0;
			int count = m_Verts.Count;

			for (int i = 0; i < effectAmount; i++)
			{
				float perc = 1f / effectAmount;
				float rot = Mathf.Deg2Rad * i * perc * 360;

				ApplyShadow (m_Verts, this.EffectColor, start, m_Verts.Count, Mathf.Cos(rot) * distanceX, Mathf.Sin(rot) * distanceY);
				start = count;
				count = m_Verts.Count;
			}

            vh.Clear();
            vh.AddUIVertexTriangleStream(m_Verts);
        }

#if UNITY_EDITOR
		protected override void OnValidate ()
		{
			EffectDistance = mEffectDistance;
			base.OnValidate ();
		}
#endif
	}
}