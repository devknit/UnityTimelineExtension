
using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	[Serializable]
	internal sealed class LightBehaviour : PlayableBehaviour
	{
		[SerializeField]
		internal Color m_Color = Color.white;
		[SerializeField, Min( 0.0f)]
		internal float m_Intensity = 1.0f;
		[SerializeField]
		internal float m_Range = 10.0f;
		[SerializeField]
		internal float m_SpotAngle = 30.0f;
		[SerializeField]
		internal float m_InnerSpotAngle = 21.80208f;
		[SerializeField, Min( 0.0f)]
		internal float m_BounceIntensity = 1.0f;
		[SerializeField]
		internal float m_ColorTemperature = 6570.0f;
	}
}
