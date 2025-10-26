
using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	[Serializable]
	sealed class TimeScaleBehaviour : PlayableBehaviour
	{
		[SerializeField, Min( 0.0f)]
		internal float m_TimeScale = 1.0f;
	}
}
