
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.ComponentModel; 

namespace Knit.Timeline
{
	[TrackClipType( typeof( VideoClip))]
	[TrackBindingType( typeof( VideoPlayer))]
	[TrackColor( 2.0f / 255.0f, 178.0f / 255.0f, 167.0f / 255.0f)]
	[DisplayName( "Knit.Timeline/Video Track")]
	sealed class VideoTrack : TrackAsset
	{
		public override Playable CreateTrackMixer( PlayableGraph graph, GameObject go, int inputCount)
		{
			var playableDirector = go.GetComponent<PlayableDirector>();
			var player = playableDirector.GetGenericBinding( this) as VideoPlayer;
			
			foreach( var clip in GetClips())
			{
				if( clip.asset is VideoClip videoClip)
				{
					videoClip.Initalize( playableDirector, player, clip.start, clip.end);
				}
			}
			var playable = ScriptPlayable<VideoMixerBehaviour>.Create( graph, inputCount);
			playable.GetBehaviour().Initalize( playableDirector, player,
				m_PreloadTime, m_IgnoreBothEndsTime, m_AllowableErrorTime);
			return playable;
		}
		public override void GatherProperties( PlayableDirector director, IPropertyCollector driver)
		{
	#if UNITY_EDITOR
			var videoPlayer = director.GetGenericBinding( this) as VideoPlayer;
			if( videoPlayer == null)
			{
				return;
			}
			driver.AddFromName<VideoPlayer>( videoPlayer.gameObject, "m_VideoClip");
			driver.AddFromName<VideoPlayer>( videoPlayer.gameObject, "m_TargetCameraAlpha");
			driver.AddFromName<VideoPlayer>( videoPlayer.gameObject, "m_TimeReference");
			driver.AddFromName<VideoPlayer>( videoPlayer.gameObject, "m_DataSource");
			driver.AddFromName<VideoPlayer>( videoPlayer.gameObject, "m_PlaybackSpeed");
			driver.AddFromName<VideoPlayer>( videoPlayer.gameObject, "m_Url");
			driver.AddFromName<VideoPlayer>( videoPlayer.gameObject, "m_PlayOnAwake");
			driver.AddFromName<VideoPlayer>( videoPlayer.gameObject, "m_SkipOnDrop");
			driver.AddFromName<VideoPlayer>( videoPlayer.gameObject, "m_Looping");
			driver.AddFromName<VideoPlayer>( videoPlayer.gameObject, "m_WaitForFirstFrame");
			driver.AddFromName<VideoPlayer>( videoPlayer.gameObject, "m_FrameReadyEventEnabled");
			driver.AddFromName<VideoPlayer>( videoPlayer.gameObject, "m_DirectAudioMutes.Array.size");
			driver.AddFromName<VideoPlayer>( videoPlayer.gameObject, "m_DirectAudioVolumes.Array.size");
			
			for( ushort i0 = 0; i0 < videoPlayer.audioTrackCount; ++i0)
			{
				driver.AddFromName<VideoPlayer>( videoPlayer.gameObject, $"m_DirectAudioMutes.Array.data[{i0}]");
				driver.AddFromName<VideoPlayer>( videoPlayer.gameObject, $"m_DirectAudioVolumes.Array.data[{i0}]");
				
				AudioSource audioSource = videoPlayer.GetTargetAudioSource( i0);
				if( audioSource != null)
				{
					driver.AddFromName<AudioSource>( audioSource.gameObject, "Mute");
					driver.AddFromName<AudioSource>( audioSource.gameObject, "m_Volume");
				}
			}
			var serializedObject = new UnityEditor.SerializedObject( videoPlayer);
			UnityEditor.SerializedProperty property = serializedObject.GetIterator();
	#endif
			base.GatherProperties( director, driver);
		}
		[SerializeField, Min( 0.0f)]
		double m_PreloadTime = 0.3;
		[SerializeField, Min( 0.1f)]
		double m_IgnoreBothEndsTime = 1.0;
		[SerializeField, Min( 0.1f)]
		double m_AllowableErrorTime = 0.5;
	}
}
