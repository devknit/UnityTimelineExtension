
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	[System.Serializable]
	sealed class TransformClip : PlayableAsset, ITimelineClipAsset
	{
		public ClipCaps clipCaps
		{
			get { return ClipCaps.Extrapolation | ClipCaps.Blending; }
		}
		public override Playable CreatePlayable( PlayableGraph graph, GameObject owner)
		{
			var playable = ScriptPlayable<TransformBehaviour>.Create( graph, m_Source);
			playable.GetBehaviour().Initialize( graph.GetResolver());
			return playable;
		}
		[SerializeField]
		TransformBehaviour m_Source = new();
	}
}
