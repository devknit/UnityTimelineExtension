
using TMPro;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.ComponentModel; 

namespace Knit.Timeline
{
	[TrackColor( 0.0f, 0.7f, 0.9f)]
	[TrackClipType( typeof( TextClip))]
	[TrackBindingType( typeof( TMP_Text))]
	[DisplayName( "Knit.Timeline/Text Track")]
	sealed class TextTrack : TrackAsset
	{
		public override Playable CreateTrackMixer( PlayableGraph graph, GameObject go, int inputCount)
		{
			foreach( var clip in GetClips())
			{
				if( clip.asset is TextClip textClip)
				{
					textClip.Initialize( clip);
				}
			}
			return ScriptPlayable<TextMixerBehaviour>.Create( graph, inputCount);
		}
		public override void GatherProperties( PlayableDirector director, IPropertyCollector driver)
		{
	#if UNITY_EDITOR
			var text = director.GetGenericBinding( this) as TMP_Text;
			if( text == null)
			{
				return;
			}
			driver.AddFromName<TMP_Text>( text.gameObject, "m_text");
	#endif
			base.GatherProperties( director, driver);
		}
	}
}
