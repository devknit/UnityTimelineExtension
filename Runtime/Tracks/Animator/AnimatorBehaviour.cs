
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace Knit.Timeline
{
	[Serializable]
	sealed class AnimatorBehaviour : PlayableBehaviour
	{
		internal void Initialize( AnimationClip animationClip, TimelineClip timelineClip, int clipIndex, int trackIndex)
		{
			m_AnimationClip = animationClip;
			m_TimelineClip = timelineClip;
			m_ClipIndex = clipIndex;
			m_TrackIndex = trackIndex;
		}
		internal bool CreatePlayable( PlayableGraph playableGraph, AnimatorTrackLayer trackLayer, int inputIndex)
		{
			if( m_AnimationClip != null && m_AnimationClip.legacy == false)
			{
				m_ClipPlayable = AnimationClipPlayable.Create( 
					playableGraph, m_AnimationClip);
				m_ClipPlayable.SetRemoveStartOffset( m_RemoveStartOffset);
				m_ClipPlayable.SetApplyFootIK( m_FootIK);
				m_ClipPlayable.SetOverrideLoopTime( m_LoopMode != AnimationPlayableAsset.LoopMode.UseSourceAsset);
				m_ClipPlayable.SetLoopTime( m_LoopMode == AnimationPlayableAsset.LoopMode.On);
				m_ClipPlayable.SetSpeed( 0.0f);
				trackLayer.ConnectClipPlayable( m_ClipIndex, ref m_ClipPlayable, 0);
				m_InputIndex = inputIndex;
				return true;
			}
			return false;
		}
		public override void OnPlayableDestroy( Playable playable)
		{
			if( m_ClipPlayable.IsValid() != false)
			{
				m_ClipPlayable.Destroy();
			}
		}
		internal bool Process( bool pauseEditor, 
			Dictionary<AnimationClip, AnimatorBehaviour> prevBehaviours,
			Dictionary<AnimationClip, AnimatorBehaviour> nextBehaviours,
			double trackTime, double deltaTime, float weight)
		{
			bool ret = false;
			
			if( m_ClipPlayable.IsValid() != false)
			{
				m_PreviousTime = m_PlayableTime;
				double s = m_TimelineClip.timeScale;
				double p = m_TimelineClip.start;
				double q = m_TimelineClip.end;
				
			#if UNITY_EDITOR
				if( pauseEditor != false && weight > 0.0f)
				{
					m_PlayableTime = GetStartTime( trackTime, p, q, s);
					m_ClipPlayable.SetTime( m_PlayableTime);
					m_PreviousPaused = true;
					return true;
				}
			#endif
				if( weight <= 0.0f)
				{
					m_PlayableTime = 0.0;
					m_PreviousPaused = true;
				}
				else
				{
					nextBehaviours[ m_AnimationClip] = this;
					m_PreviousPaused = false;
					
					if( m_TakeOverTime != false && prevBehaviours.TryGetValue( m_AnimationClip, out var behaviour) != false)
					{
						if( behaviour != this && behaviour.m_PreviousTime >= 0.0)
						{
							m_PlayableTime = behaviour.m_PreviousTime;
						}
					}
					if( m_PreviousPaused != false
					||	(m_TimelineClip.preExtrapolationMode == TimelineClip.ClipExtrapolation.PingPong && (trackTime < p || trackTime >= q)))
					{
						m_PlayableTime = GetStartTime( trackTime, p, q, s);
					}
					else
					{
						if( (m_TimelineClip.preExtrapolationMode == TimelineClip.ClipExtrapolation.Hold && p > trackTime)
						||	(m_TimelineClip.postExtrapolationMode == TimelineClip.ClipExtrapolation.Hold && q < trackTime))
						{
							deltaTime = 0.0f;
						}
						m_PlayableTime += deltaTime * s;
						
						switch( m_TimelineClip.postExtrapolationMode)
						{
							case TimelineClip.ClipExtrapolation.Loop:
							{
								double ds = m_TimelineClip.duration * s;
								
								while( m_PlayableTime < 0.0)
								{
									m_PlayableTime += ds; 
								}
								while( m_PlayableTime > ds)
								{
									m_PlayableTime -= ds; 
								}
								break;
							}
							case TimelineClip.ClipExtrapolation.Continue:
							{
								double ds = m_AnimationClip.length * s;
								
								while( m_PlayableTime < 0.0)
								{
									m_PlayableTime += ds; 
								}
								while( m_PlayableTime > ds)
								{
									m_PlayableTime -= ds; 
								}
								break;
							}
						}
					}
					ret = true;
				}
				m_ClipPlayable.SetTime( Math.Max( 0.0f, m_PlayableTime));
			}
			return ret;
		}
		double GetStartTime( double trackTime, double p, double q, double s)
		{
			double ps = (trackTime - p) * s;
			
			switch( m_TimelineClip.preExtrapolationMode)
			{
				case TimelineClip.ClipExtrapolation.Hold:
				{
					if( ps < 0.0)
					{
						ps = 0.0;
					}
					break;
				}
				case TimelineClip.ClipExtrapolation.Loop:
				{
					double ds = m_TimelineClip.duration * s;
					
					while( ps < 0.0)
					{
						ps += ds; 
					}
					break;
				}
				case TimelineClip.ClipExtrapolation.PingPong:
				{
					double ds = m_TimelineClip.duration * s;
					int i0 = 0;
					
					while( ps < 0.0)
					{
						ps += ds; 
						i0 = (i0 + 1) % 2;
					}
					if( i0 == 1)
					{
						ps = ds - ps;
					}
					break;
				}
				case TimelineClip.ClipExtrapolation.Continue:
				{
					double ds = m_AnimationClip.length * s;
					
					while( ps < 0.0)
					{
						ps += ds; 
					}
					break;
				}
			}
			switch( m_TimelineClip.postExtrapolationMode)
			{
				case TimelineClip.ClipExtrapolation.Hold:
				{
					double qs = (q - p) * s;
					
					if( qs >= 0)
					{
						if( ps > qs)
						{
							ps = qs;
						}
					}
					break;
				}
				case TimelineClip.ClipExtrapolation.Loop:
				{
					double ds = m_TimelineClip.duration * s;
					
					while( ps > ds)
					{
						ps -= ds; 
					}
					break;
				}
				case TimelineClip.ClipExtrapolation.PingPong:
				{
					double ds = m_TimelineClip.duration * s;
					int i0 = 0;
					
					while( ps > ds)
					{
						ps -= ds;
						i0 = (i0 + 1) % 2;
					}
					if( i0 == 1)
					{
						ps = ds - ps;
					}
					break;
				}
				case TimelineClip.ClipExtrapolation.Continue:
				{
					double ds = m_AnimationClip.length * s;
					
					while( ps > ds)
					{
						ps -= ds; 
					}
					break;
				}
			}
			return ps;
		}
	#if UNITY_EDITOR
		internal bool HasHumanoidMotion()
		{
			return m_AnimationClip?.humanMotion ?? false;
		}
	#endif
		internal AnimationPlayableAsset.LoopMode LoopMode
		{
			get{ return m_LoopMode; }
		}
		[SerializeField]
		Vector3 m_PositionOffset;
		[SerializeField]
		Vector3 m_RotationOffset;
		[SerializeField]
		bool m_RemoveStartOffset = true;
		[SerializeField]
		bool m_FootIK = true;
		[SerializeField]
		AnimationPlayableAsset.LoopMode m_LoopMode;
		[SerializeField]
		bool m_TakeOverTime = true;
		
		[NonSerialized]
		AnimationClip m_AnimationClip;
		[NonSerialized]
		TimelineClip m_TimelineClip;
		[NonSerialized]
		AnimationClipPlayable m_ClipPlayable;
		[NonSerialized]
		double m_PreviousTime;
		[NonSerialized]
		double m_PlayableTime;
		[NonSerialized]
		bool m_PreviousPaused;
		[NonSerialized]
		internal int m_ClipIndex;
		[NonSerialized]
		internal int m_TrackIndex;
		[NonSerialized]
		internal int m_InputIndex;
	}
}
