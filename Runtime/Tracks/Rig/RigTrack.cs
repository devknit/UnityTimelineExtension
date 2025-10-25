
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.Animations.Rigging;

namespace Knit.TimelineExtension
{
	[TrackColor( 0.9f, 0.9f, 0.9f)]
	[TrackClipType( typeof( RigClip))]
	[TrackBindingType( typeof( Rig))]
	sealed class RigTrack : TrackAsset
	{
		public override Playable CreateTrackMixer( PlayableGraph graph, GameObject go, int inputCount)
		{
			return ScriptPlayable<RigMixerBehaviour>.Create( graph, inputCount);
		}
	}
}
