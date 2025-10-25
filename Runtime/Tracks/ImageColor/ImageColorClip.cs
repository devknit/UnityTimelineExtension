
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.TimelineExtension
{
	[System.Serializable]
	sealed class ImageColorClip : PlayableAsset, ITimelineClipAsset
	{
		public ClipCaps clipCaps
		{
			get { return ClipCaps.Extrapolation | ClipCaps.Blending; }
		}
		public override Playable CreatePlayable( PlayableGraph graph, GameObject owner)
		{
			return ScriptPlayable<ImageColorBehaviour>.Create( graph, m_Source);
		}
		[SerializeField]
		ImageColorBehaviour m_Source = new();
	}
}
