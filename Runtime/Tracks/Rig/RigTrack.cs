
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.Animations.Rigging;
using System.ComponentModel; 

namespace Knit.Timeline
{
	[TrackColor( 0.9f, 0.9f, 0.9f)]
	[TrackClipType( typeof( RigClip))]
	[TrackBindingType( typeof( Rig))]
	[DisplayName( "Knit.Timeline/Rigging/Rig Track")]
	sealed class RigTrack : TrackAsset
	{
		public override Playable CreateTrackMixer( PlayableGraph graph, GameObject go, int inputCount)
		{
			return ScriptPlayable<RigMixerBehaviour>.Create( graph, inputCount);
		}
	}
}
