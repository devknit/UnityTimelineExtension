
using System;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	[Serializable]
	sealed class RepeatClip : PlayableAsset, ITimelineClipAsset
	{
		public ClipCaps clipCaps
		{
			get { return ClipCaps.None; }
		}
		public override Playable CreatePlayable( PlayableGraph graph, GameObject owner)
		{
			var playable = ScriptPlayable<RepeatBehaviour>.Create( graph, m_Source);
			playable.GetBehaviour().Initalize( this);
			return playable;
		}
		internal void Initalize( PlayableDirector playableDirector, 
			RepeatReceiver receiver, RepeatTrack track, TimelineClip clip)
		{
			m_PlayableDirector = playableDirector;
			m_Receiver = receiver;
			m_Track = track;
			m_Clip = clip;
		}
		internal PlayableDirector PlayableDirector
		{
			get{ return m_PlayableDirector; }
		}
		internal RepeatReceiver Receiver
		{
			get{ return m_Receiver; }
		}
		internal RepeatTrack Track
		{
			get{ return m_Track; }
		}
		internal TimelineClip Clip
		{
			get{ return m_Clip; }
		}
		[SerializeField]
		RepeatBehaviour m_Source = new();
		[NonSerialized]
		PlayableDirector m_PlayableDirector;
		[NonSerialized]
		RepeatReceiver m_Receiver;
		[NonSerialized]
		RepeatTrack m_Track;
		[NonSerialized]
		TimelineClip m_Clip;
	}
}
