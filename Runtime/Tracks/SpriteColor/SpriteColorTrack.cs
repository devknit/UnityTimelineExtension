
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.TimelineExtension
{
	[TrackColor( 173.0f / 255.0f, 144.0f / 255.0f, 242.0f / 255.0f)]
	[TrackClipType( typeof( SpriteColorClip))]
	[TrackBindingType( typeof( SpriteRenderer))]
	sealed class SpriteColorTrack : TrackAsset
	{
		public override Playable CreateTrackMixer( PlayableGraph graph, GameObject go, int inputCount)
		{
			return ScriptPlayable<SpriteColorMixerBehaviour>.Create( graph, inputCount);
		}
		public override void GatherProperties( PlayableDirector director, IPropertyCollector driver)
		{
	#if UNITY_EDITOR
			var spriteRenderer = director.GetGenericBinding( this) as SpriteRenderer;
			if( spriteRenderer == null)
			{
				return;
			}
			driver.AddFromName( spriteRenderer, "m_Color");
	#endif
			base.GatherProperties( director, driver);
		}
	}
}
