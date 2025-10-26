
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

namespace Knit.Timeline
{
	sealed class VideoMixerBehaviour : PlayableBehaviour
	{
		internal void Initalize( PlayableDirector playableDirector, VideoPlayer player,
			double preloadTime, double ignoreBothEndsTime, double allowableErrorTime)
		{
			m_PlayableDirector = playableDirector;
			m_VideoPlayer = player;
			m_PreloadTime = preloadTime;
			m_IgnoreBothEndsTime = ignoreBothEndsTime;
			m_AllowableErrorTime = allowableErrorTime;
		}
		public override void OnPlayableCreate( Playable playable)
		{
			if( m_VideoPlayer != null)
			{
				m_DefaultSource = m_VideoPlayer.source;
				m_DefaultClip = m_VideoPlayer.clip;
				m_DefaultURL = m_VideoPlayer.url;
				m_DefaultPlayOnAwake = m_VideoPlayer.playOnAwake;
				m_DefaultWaitForFirstFrame = m_VideoPlayer.waitForFirstFrame;
				m_DefaultIsLooping = m_VideoPlayer.isLooping;
				m_DefaultSkipOnDrop = m_VideoPlayer.skipOnDrop;
				m_DefaultPlaybackSpeed = m_VideoPlayer.playbackSpeed;
				m_DefaultTargetCameraAlpha = m_VideoPlayer.targetCameraAlpha;
				
				if( m_VideoPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
				{
					m_DefaultMuteVolumes = new (bool, float)[ m_VideoPlayer.audioTrackCount];
					
					for( ushort i0 = 0; i0 < m_VideoPlayer.audioTrackCount; ++i0)
					{
						m_DefaultMuteVolumes[ i0] =  
							(m_VideoPlayer.GetDirectAudioMute( i0),
							m_VideoPlayer.GetDirectAudioVolume( i0));
					}
				}
				else if( m_VideoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource)
				{
					m_DefaultMuteVolumes = new (bool, float)[ m_VideoPlayer.audioTrackCount];
					
					for( ushort i0 = 0; i0 < m_VideoPlayer.audioTrackCount; ++i0)
					{
						AudioSource audioSource = m_VideoPlayer.GetTargetAudioSource( i0);
						if( audioSource != null)
						{
							m_DefaultMuteVolumes[ i0] = (audioSource.mute, audioSource.volume);
						}
					}
				}
			}
		}
		public override void OnPlayableDestroy( Playable playable)
		{
			if( m_VideoPlayer != null)
			{
				m_VideoPlayer.source = m_DefaultSource;
				m_VideoPlayer.clip = m_DefaultClip;
				m_VideoPlayer.url = m_DefaultURL;
				m_VideoPlayer.playOnAwake = m_DefaultPlayOnAwake;
				m_VideoPlayer.waitForFirstFrame = m_DefaultWaitForFirstFrame;
				m_VideoPlayer.isLooping = m_DefaultIsLooping;
				m_VideoPlayer.skipOnDrop = m_DefaultSkipOnDrop;
				m_VideoPlayer.playbackSpeed = m_DefaultPlaybackSpeed;
				m_VideoPlayer.targetCameraAlpha = m_DefaultTargetCameraAlpha;
				
				if( m_VideoPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
				{
					for( ushort i0 = 0; i0 < m_VideoPlayer.audioTrackCount; ++i0)
					{
						m_VideoPlayer.SetDirectAudioMute( i0, m_DefaultMuteVolumes[ i0].mute);
						m_VideoPlayer.SetDirectAudioVolume( i0, m_DefaultMuteVolumes[ i0].volume);
					}
				}
				else if( m_VideoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource)
				{
					m_DefaultMuteVolumes = new (bool, float)[ m_VideoPlayer.audioTrackCount];
					
					for( ushort i0 = 0; i0 < m_VideoPlayer.audioTrackCount; ++i0)
					{
						AudioSource audioSource = m_VideoPlayer.GetTargetAudioSource( i0);
						if( audioSource != null)
						{
							audioSource.mute = m_DefaultMuteVolumes[ i0].mute;
							audioSource.volume = m_DefaultMuteVolumes[ i0].volume;
						}
					}
				}
			}
		}
		public override void OnBehaviourPause( Playable playable, FrameData info)
		{
			Process( playable);
		}
		public override void PrepareFrame( Playable playable, FrameData info)
		{
			Process( playable);
		}
		void Process( Playable playable)
		{
			VideoState timelineState = GetState();
			int inputCount = playable.GetInputCount();
			double currentTime = playable.GetTime();
			bool isClipPlaying = false;
			
			for( int i0 = 0; i0 < inputCount; ++i0)
			{
				var inputPlayable = (ScriptPlayable<VideoBehaviour>)playable.GetInput( i0);
				VideoBehaviour behaviour = inputPlayable.GetBehaviour();
				double t = behaviour.GetVideoTime( currentTime);
				VideoState clipState = VideoState.None;
				
				if( currentTime < behaviour.m_EndTime)
				{
					if( behaviour.m_StartTime <= currentTime)
					{
						clipState = timelineState switch
						{
							VideoState.Play => VideoState.Play,
							VideoState.Pause => VideoState.Pause,
							_ => VideoState.None
						};
					}
					else if( Math.Max( 0.0, behaviour.m_StartTime - m_PreloadTime) <= currentTime)
					{
						if( isClipPlaying == false)
						{
							clipState = VideoState.Prepare;
						}
					}
				}
				if( behaviour.m_State != clipState)
				{
					switch( clipState)
					{
						case VideoState.None: behaviour.Stop(); break;
						case VideoState.Prepare: behaviour.Prepare( t); break;
						case VideoState.Play: behaviour.Play( t, m_AllowableErrorTime); break;
						case VideoState.Pause: behaviour.Pause( t, m_AllowableErrorTime); break;
					}
				}
				else if( behaviour.m_State == VideoState.Play || behaviour.m_State == VideoState.Pause)
				{
					behaviour.CorrectMisalign( t, m_IgnoreBothEndsTime, m_AllowableErrorTime);
				}
				isClipPlaying = clipState != VideoState.None;
			}
		}
		VideoState GetState()
		{
		#if UNITY_EDITOR
			if( Application.isPlaying == false)
			{
				if( m_PlayableDirector.state == PlayState.Playing)
				{
					return VideoState.Play;
				}
				return VideoState.Pause;
			}
			else
		#endif
			{
				if( m_PlayableDirector.state == PlayState.Playing)
				{
					return VideoState.Play;
				}
			}
			return VideoState.Pause;
		}
		PlayableDirector m_PlayableDirector;
		VideoPlayer m_VideoPlayer;
		double m_PreloadTime;
		double m_IgnoreBothEndsTime;
		double m_AllowableErrorTime;
		
		VideoSource m_DefaultSource;
		UnityEngine.Video.VideoClip m_DefaultClip;
		string m_DefaultURL;
		bool m_DefaultPlayOnAwake;
		bool m_DefaultWaitForFirstFrame;
		bool m_DefaultIsLooping;
		bool m_DefaultSkipOnDrop;
		float m_DefaultPlaybackSpeed;
		float m_DefaultTargetCameraAlpha;
		(bool mute, float volume)[] m_DefaultMuteVolumes;
	}
}
