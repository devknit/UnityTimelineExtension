
using System;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	[Serializable]
	sealed class VideoClip : PlayableAsset, ITimelineClipAsset
	{
		public ClipCaps clipCaps
		{
			get { return ClipCaps.None; }
		}
		public override Playable CreatePlayable( PlayableGraph graph, GameObject owner)
		{
			var playable = ScriptPlayable<VideoBehaviour>.Create( graph, m_Source);
			playable.GetBehaviour().Initialize( m_PlayableDirector, m_VideoPlayer, m_StartTime, m_EndTime);
			return playable;
		}
		internal void Initalize( PlayableDirector playableDirector, VideoPlayer videoPlayer, double startTime, double endTime)
		{
			m_PlayableDirector = playableDirector;
			m_VideoPlayer = videoPlayer;
			m_StartTime = startTime;
			m_EndTime = endTime;
		}
		[SerializeField]
		VideoBehaviour m_Source = new();
		[NonSerialized]
		PlayableDirector m_PlayableDirector;
		[NonSerialized]
		VideoPlayer m_VideoPlayer;
		[NonSerialized]
		double m_StartTime;
		[NonSerialized]
		double m_EndTime;
	}
}
