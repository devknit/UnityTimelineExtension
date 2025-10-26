
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.ComponentModel; 

namespace Knit.Timeline
{
	[TrackColor( 0.9f, 0.9f, 0.9f)]
	[TrackClipType( typeof( TransformClip))]
	[TrackBindingType( typeof( Transform))]
	[DisplayName( "Knit.Timeline/Transform Track")]
	sealed class TransformTrack : TrackAsset
	{
		public override Playable CreateTrackMixer( PlayableGraph graph, GameObject go, int inputCount)
		{
			var playable = ScriptPlayable<TransformMixerBehaviour>.Create( graph, inputCount);
			playable.GetBehaviour().Initialize( m_PositionVolume);
			return playable;
		}
		public override void GatherProperties( PlayableDirector director, IPropertyCollector driver)
		{
	#if UNITY_EDITOR
			var transform = director.GetGenericBinding( this) as Transform;
			if( transform == null)
			{
				return;
			}
			driver.GatherTransform( transform);
	#endif
			base.GatherProperties( director, driver);
		}
		[SerializeField]
		PositionVolume m_PositionVolume = PositionVolume.Local;
	}
}
