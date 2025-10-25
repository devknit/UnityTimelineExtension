
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.Animations.Rigging;

namespace Knit.TimelineExtension
{
	[TrackColor( 0.9f, 0.9f, 0.9f)]
	[TrackClipType( typeof( MultiAimClip))]
	[TrackBindingType( typeof( MultiAimConstraint))]
	sealed class MultiAimTrack : TrackAsset
	{
		public override Playable CreateTrackMixer( PlayableGraph graph, GameObject go, int inputCount)
		{
			return ScriptPlayable<MultiAimMixerBehaviour>.Create( graph, inputCount);
		}
	}
}
