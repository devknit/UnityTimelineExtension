
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.TimelineExtension
{
	[System.Serializable]
	sealed class TextClip : PlayableAsset, ITimelineClipAsset
	{
		public ClipCaps clipCaps
		{
			get { return ClipCaps.Extrapolation; }
		}
		public override Playable CreatePlayable( PlayableGraph graph, GameObject owner)
		{
			return ScriptPlayable<TextBehaviour>.Create( graph, m_Source);
		}
		internal void Initialize( TimelineClip timelineClip)
		{
			m_TimelineClip = timelineClip;
		#if UNITY_EDITOR
			m_TimelineClip.displayName = m_Source.m_Text?.Split( '\n').FirstOrDefault() ?? string.Empty;
		#endif
		}
		[SerializeField]
		TextBehaviour m_Source = new();
		[System.NonSerialized]
		TimelineClip m_TimelineClip;
	}
}
