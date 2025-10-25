
using System;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.TimelineExtension
{
	[TrackClipType( typeof( RepeatClip))]
	[TrackBindingType( typeof( RepeatReceiver))]
	[TrackColor( 235.0f / 255.0f, 51.0f / 255.0f, 36.0f / 255.0f)]
	sealed class RepeatTrack : TrackAsset, IRepeatTrack
	{
		public ISeekTarget GetDefaultTarget()
		{
			return m_MixerBehaviour?.GetDefaultTarget();
		}
		public ISeekTarget[] GetOtherTargets()
		{
			return m_MixerBehaviour?.GetOtherTargets() ?? new ISeekTarget[ 0];
		}
		public bool Abort( string targetName)
		{
			return m_MixerBehaviour?.Abort( targetName) ?? false;
		}
		public override Playable CreateTrackMixer( PlayableGraph graph, GameObject go, int inputCount)
		{
			var playableDirector = go.GetComponent<PlayableDirector>();
			var receiver = playableDirector.GetGenericBinding( this) as RepeatReceiver;
			
			foreach( var clip in GetClips())
			{
				if( clip.asset is RepeatClip asset)
				{
					asset.Initalize( playableDirector, receiver, this, clip);
				}
			}
			var playable = ScriptPlayable<RepeatMixerBehaviour>.Create( graph, inputCount);
			m_MixerBehaviour = playable.GetBehaviour();
			m_MixerBehaviour.Initalize( playableDirector, receiver, this);
			return playable;
		}
		[NonSerialized]
		RepeatMixerBehaviour m_MixerBehaviour;
	}
}
