
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.TimelineExtension
{
	[TrackClipType( typeof( SectionClip))]
	[TrackColor( 240.0f / 255.0f, 135.0f / 255.0f, 132.0f / 255.0f)]
	public sealed class SectionTrack : TrackAsset
	{
		public override Playable CreateTrackMixer( PlayableGraph graph, GameObject go, int inputCount)
		{
			foreach( var clip in GetClips())
			{
				if( clip.asset is SectionClip asset)
				{
					asset.Initalize( clip);
				}
			}
			return Playable.Create( graph, inputCount);
		}
	}
}
