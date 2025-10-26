
using System;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	[Serializable]
	sealed class AnimatorClip : PlayableAsset, ITimelineClipAsset, IBindableClip
	{
		public ClipCaps clipCaps
		{
			get
			{
				ClipCaps caps = ClipCaps.Extrapolation | ClipCaps.Blending | ClipCaps.SpeedMultiplier;
				
				if( m_AnimationClip != null)
				{
					AnimationPlayableAsset.LoopMode loopMode = m_Source.LoopMode;
					
					if( loopMode != AnimationPlayableAsset.LoopMode.Off
					&&	(loopMode != AnimationPlayableAsset.LoopMode.UseSourceAsset || m_AnimationClip.isLooping != false))
					{
						caps |= ClipCaps.Looping;
					}
					if( m_AnimationClip.empty == false)
					{
						caps |= ClipCaps.ClipIn;
					}
				}
				return caps;
			}
		}
		public override double duration
		{
			get
			{
				if (m_AnimationClip == null || m_AnimationClip.empty)
				{
					return 0;
				}
				double length = m_AnimationClip.length;
				
				if( m_AnimationClip.frameRate > 0)
				{
					double frames = Mathf.Round( m_AnimationClip.length * m_AnimationClip.frameRate);
					length = frames / m_AnimationClip.frameRate;
				}
				if( length < float.Epsilon)
				{
					return base.duration;
				}
				return length;
			}
		}
		public override Playable CreatePlayable( PlayableGraph graph, GameObject owner)
		{
			var playable = ScriptPlayable<AnimatorBehaviour>.Create( graph, m_Source);
			playable.GetBehaviour().Initialize( m_AnimationClip, m_TimelineClip, m_ClipIndex, m_TrackIndex);
		#if UNITY_EDITOR
			if( m_TimelineClip != null && m_AnimationClip != null)
			{
				string[] split = m_TimelineClip.displayName.Split('$');
				
				if( split.Length > 1)
				{
					var builder = new System.Text.StringBuilder();
					
					if( split[ 0].Equals( m_AnimationClip.name) == false)
					{
						builder.Append( m_AnimationClip.name);
						
						for( int i0 = 1; i0 < split.Length; ++i0)
						{
							builder.Append( $"${split[i0]}");
						}
					}
					else
					{
						builder.Append( m_TimelineClip.displayName);
					}
					m_TimelineClip.displayName = builder.ToString();
				}
				else
				{
					m_TimelineClip.displayName = m_AnimationClip.name;
				}
			}
		#endif
			return playable;
		}
		public int GetAssignFieldNameCount()
		{
			return 1;
		}
		public string GetAssignFieldName( int index)
		{
			return typeof(AnimationClip).Name;
		}
		public Type GetAssignFieldType( string fieldName)
		{
			return typeof(AnimationClip);
		}
		public void SetBindObject( UnityEngine.Object obj, string fieldName)
		{
			m_AnimationClip = obj as AnimationClip;
		}
		internal bool Initialize( TimelineClip timelineClip, int clipIndex, int trackIndex)
		{
			m_TimelineClip = timelineClip;
			m_ClipIndex = clipIndex;
			m_TrackIndex = trackIndex;
			
			if( m_AnimationClip != null)
			{
				return m_AnimationClip.humanMotion;
			}
			return false;
		}
		internal AnimationClip AnimationClip
		{
			get{ return m_AnimationClip; }
			set{ m_AnimationClip = value; }
		}
		[SerializeField]
		AnimationClip m_AnimationClip;
		[SerializeField]
		AnimatorBehaviour m_Source = new();
		[NonSerialized]
		TimelineClip m_TimelineClip;
		[NonSerialized]
		int m_ClipIndex;
		[NonSerialized]
		int m_TrackIndex;
	}
}
