
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	[System.Serializable]
	sealed class SpriteColorClip : PlayableAsset, ITimelineClipAsset
	{
		public ClipCaps clipCaps
		{
			get { return ClipCaps.Extrapolation | ClipCaps.Blending; }
		}
		public override Playable CreatePlayable( PlayableGraph graph, GameObject owner)
		{
			return ScriptPlayable<SpriteColorBehaviour>.Create( graph, m_Source);
		}
		[SerializeField]
		SpriteColorBehaviour m_Source = new();
	}
}
