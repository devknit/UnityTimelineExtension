
using System;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	[Serializable]
	sealed class TimeScaleClip : PlayableAsset, ITimelineClipAsset
	{
		public ClipCaps clipCaps
		{
			get { return ClipCaps.Extrapolation | ClipCaps.Blending; }
		}
		public override Playable CreatePlayable( PlayableGraph graph, GameObject owner)
		{
			return ScriptPlayable<TimeScaleBehaviour>.Create( graph, m_Source);
		}
		[SerializeField]
		TimeScaleBehaviour m_Source = new();
	}
}
