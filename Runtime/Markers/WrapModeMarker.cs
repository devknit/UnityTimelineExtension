
using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Knit.TimelineExtension
{
	[Serializable]
	public sealed class WrapModeMarker : UniversalMarker, INotification
	{
		public override string PropertyName
		{
			get{ return "Knit.Timeline.WrapModeMarker"; }
		}
		public override void OnNotify( UniversalMarkerReceiver receiver, Playable playable, object context)
		{
			if( m_PlayableDirector == null)
			{
				m_PlayableDirector = receiver.GetComponent<PlayableDirector>();
			}
            
			if( m_PlayableDirector != null)
			{
				m_PlayableDirector.extrapolationMode = m_WrapMode;
			}
		#if UNITY_EDITOR
			else if( receiver != null)
			{
				Debug.LogError( $"{receiver.name} に PlayableDirector が付与されていません", receiver);
			}
		#endif
		}

		[Space]
		[SerializeField]
		DirectorWrapMode m_WrapMode = DirectorWrapMode.Hold;
		[NonSerialized]
		PlayableDirector m_PlayableDirector;
	}
}
