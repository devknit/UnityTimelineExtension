
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.ComponentModel; 

namespace Knit.Timeline
{
	[TrackColor( 194.0f / 255.0f, 194.0f / 255.0f, 194.0f / 255.0f)]
	[TrackClipType( typeof( ImageColorClip))]
	[TrackBindingType( typeof( UnityEngine.UI.Graphic))]
	[DisplayName( "Knit.Timeline/Image Color Track")]
	sealed class ImageColorTrack : TrackAsset
	{
		public override Playable CreateTrackMixer( PlayableGraph graph, GameObject go, int inputCount)
		{
			return ScriptPlayable<ImageColorMixerBehaviour>.Create( graph, inputCount);
		}
		public override void GatherProperties( PlayableDirector director, IPropertyCollector driver)
		{
	#if UNITY_EDITOR
			var graphic = director.GetGenericBinding( this) as UnityEngine.UI.Graphic;
			if( graphic == null)
			{
				return;
			}
			driver.AddFromName( graphic, "m_Color");
	#endif
			base.GatherProperties( director, driver);
		}
	}
}
