
using System;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	[Serializable]
	sealed class VideoBehaviour : PlayableBehaviour
	{
		internal void Initialize( PlayableDirector playableDirector, VideoPlayer videoPlayer, double startTime, double endTime)
		{
			m_PlayableDirector = playableDirector;
			m_VideoPlayer = videoPlayer;
			m_StartTime = startTime;
			m_EndTime = endTime;
		}
		public override void OnPlayableDestroy( Playable playable)
		{
			Stop();
		}
		internal void Prepare( double t)
		{
			if( m_VideoPlayer != null && m_VideoClip != false && m_State == VideoState.None)
			{
				m_VideoPlayer.source = VideoSource.VideoClip;
				m_VideoPlayer.clip = m_VideoClip;
				m_VideoPlayer.playOnAwake = false;
				m_VideoPlayer.waitForFirstFrame = true;
				m_VideoPlayer.playbackSpeed = 1.0f;
				m_VideoPlayer.targetCameraAlpha = 0.0f;
				m_VideoPlayer.isLooping = m_Loop;
				UpdateVolume( null);
				
				m_VideoPlayer.loopPointReached += OnLoopPointReached;
				m_VideoPlayer.time = t;
				m_VideoPlayer.Prepare();
				m_State = VideoState.Prepare;
			}
		}
		internal void Play( double t, double allowableErrorTime)
		{
			if( m_VideoPlayer != null)
			{
				if( m_State == VideoState.None)
				{
					Prepare( t);
				}
				if( m_State == VideoState.Prepare
				||	m_State == VideoState.Pause)
				{
					CorrectMisalign( t, 0, allowableErrorTime);
					m_VideoPlayer.targetCameraAlpha = 1.0f;
					m_VideoPlayer.Play();
					m_State = VideoState.Play;
				}
			}
		}
		internal void Pause( double t, double allowableErrorTime)
		{
			if( m_VideoPlayer != null)
			{
				if( m_State == VideoState.None)
				{
					Prepare( t);
				}
				if( m_State == VideoState.Prepare)
				{
					CorrectMisalign( t, 0, allowableErrorTime);
					m_VideoPlayer.Play();
					m_State = VideoState.Play;
				}
				if( m_State == VideoState.Play)
				{
					CorrectMisalign( t, 0, allowableErrorTime);
					m_VideoPlayer.Pause();
					m_State = VideoState.Pause;
				}
			}
		}
		internal void Stop()
		{
			if( m_VideoPlayer != null)
			{
				if( m_State != VideoState.None)
				{
					m_VideoPlayer.Stop();
					m_VideoPlayer.loopPointReached -= OnLoopPointReached;
					m_State = VideoState.None;
				}
			}
		}
		internal void CorrectMisalign( double t, double ignoreBothEndsTime, double allowableErrorTime)
		{
			if( m_VideoPlayer?.clip != null)
			{
				double p = m_VideoPlayer.time;
				
				if( p >= ignoreBothEndsTime && p < m_VideoPlayer.clip.length - ignoreBothEndsTime)
				{
					if( Math.Abs( p - t) > allowableErrorTime)
					{
						m_VideoPlayer.time = t;
					}
				}
			}
		}
		internal double GetVideoTime( double trackTime)
		{
			if( m_VideoPlayer != null && m_VideoClip != null)
			{
				double clipTime = trackTime - m_StartTime;
				
				if( m_Loop != false)
				{
				#if true
					clipTime = Math.Max( 0.0, clipTime) * m_VideoPlayer.playbackSpeed;
					
					while( clipTime > m_VideoClip.length)
					{
						clipTime -= m_VideoClip.length;
					}
					return clipTime;
				#else
					return Math.Max( 0.0, clipTime) * m_VideoPlayer.playbackSpeed % m_VideoClip.length;
				#endif
				}
				return Math.Clamp( clipTime * m_VideoPlayer.playbackSpeed, 
					0.0, m_VideoClip.length/* - 1.0 / m_VideoClip.frameRate*/);
			}
			return 0.0;
		}
		internal void UpdateVolume( float? volume)
		{
			if( m_VideoPlayer != null && m_VideoPlayer?.clip == m_VideoClip)
			{
				ushort audioTrackCount = m_VideoClip.audioTrackCount;
				
				if( m_VideoPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
				{
					for( ushort i0 = 0; i0 < audioTrackCount; ++i0)
					{
						m_VideoPlayer.SetDirectAudioMute( i0, m_Mute);
						
						if( volume.HasValue != false)
						{
							m_VideoPlayer.SetDirectAudioVolume( i0, volume.Value);
						}
					}
				}
				else if( m_VideoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource)
				{
					for( ushort i0 = 0; i0 < audioTrackCount; ++i0)
					{
						AudioSource audioSource = m_VideoPlayer.GetTargetAudioSource( i0);
						if( audioSource != null)
						{
							audioSource.mute = m_Mute;
							
							if( volume.HasValue != false)
							{
								audioSource.volume = volume.Value;
							}
						}
					}
				}
			}
		}
		void OnLoopPointReached( VideoPlayer player)
		{
		}
		[SerializeField]
		UnityEngine.Video.VideoClip m_VideoClip;
		[SerializeField]
		bool m_Mute = false;
		[SerializeField]
		bool m_Loop = true;
		[NonSerialized]
		internal PlayableDirector m_PlayableDirector;
		[NonSerialized]
		VideoPlayer m_VideoPlayer;
		[NonSerialized]
		internal VideoState m_State;
		[NonSerialized]
		internal double m_StartTime;
		[NonSerialized]
		internal double m_EndTime;
	}
	internal enum VideoState
	{
		None,
		Prepare,
		Play,
		Pause,
	}
}
