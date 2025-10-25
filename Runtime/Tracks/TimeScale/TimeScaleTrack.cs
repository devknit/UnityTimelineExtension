
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.TimelineExtension
{
	[TrackClipType( typeof( TimeScaleClip))]
	[TrackBindingType( typeof( TimeScaleReceiver))]
	[TrackColor( 128.0f / 255.0f, 128.0f / 255.0f, 128.0f / 255.0f)]
	sealed class TimeScaleTrack : TrackAsset
	{
		public override Playable CreateTrackMixer( PlayableGraph graph, GameObject go, int inputCount)
		{
			return ScriptPlayable<TimeScaleMixerBehaviour>.Create( graph, inputCount);
		}
	}
}
