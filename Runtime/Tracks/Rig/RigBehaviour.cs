
using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	[Serializable]
	sealed class RigBehaviour : PlayableBehaviour
	{
		[SerializeField, Range( 0.0f, 1.0f)]
		internal float m_Weight = 1.0f;
	}
}
