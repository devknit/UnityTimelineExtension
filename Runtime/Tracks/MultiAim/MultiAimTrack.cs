
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.Animations.Rigging;
using System.ComponentModel; 

namespace Knit.Timeline
{
	[TrackColor( 0.9f, 0.9f, 0.9f)]
	[TrackClipType( typeof( MultiAimClip))]
	[TrackBindingType( typeof( MultiAimConstraint))]
	[DisplayName( "Knit.Timeline/Rigging/Multi Aim Track")]
	sealed class MultiAimTrack : TrackAsset
	{
		public override Playable CreateTrackMixer( PlayableGraph graph, GameObject go, int inputCount)
		{
			return ScriptPlayable<MultiAimMixerBehaviour>.Create( graph, inputCount);
		}
	}
}
