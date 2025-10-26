
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.ComponentModel; 

namespace Knit.Timeline
{
	[TrackBindingType( typeof( Light))]
	[TrackClipType( typeof( LightClip))]
	[TrackColor( 240.0f / 255.0f, 250.0f / 255.0f, 100.0f / 255.0f)]
	[DisplayName( "Knit.Timeline/Light Track")]
	sealed class LightTrack : TrackAsset
	{
		public override Playable CreateTrackMixer( PlayableGraph graph, GameObject go, int inputCount)
		{
			return ScriptPlayable<LightMixerBehaviour>.Create( graph, inputCount);
		}
		public override void GatherProperties( PlayableDirector director, IPropertyCollector driver)
		{
	#if UNITY_EDITOR
			var light = director.GetGenericBinding( this) as Light;
			if( light == null)
			{
				return;
			}
			driver.AddFromName<Light>( light.gameObject, "m_Color");
			driver.AddFromName<Light>( light.gameObject, "m_Intensity");
			driver.AddFromName<Light>( light.gameObject, "m_Range");
			driver.AddFromName<Light>( light.gameObject, "m_SpotAngle");
			driver.AddFromName<Light>( light.gameObject, "m_InnerSpotAngle");
			driver.AddFromName<Light>( light.gameObject, "m_BounceIntensity");
			driver.AddFromName<Light>( light.gameObject, "m_ColorTemperature");
	#endif
			base.GatherProperties( director, driver);
		}
	}
}
