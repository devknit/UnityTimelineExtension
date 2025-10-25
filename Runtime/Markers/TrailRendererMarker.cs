
using System;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.TimelineExtension
{
	[Serializable]
	[CustomStyle( "TrailRendererMarker")]
	sealed class TrailRendererMarker : UniversalMarker, INotification
	{
		public override string PropertyName
		{
			get{ return "Zurp.Timeline.TrailRendererMarker"; }
		}
		public override void OnNotify( UniversalMarkerReceiver receiver, Playable playable, object context)
		{
			if( m_TrailRenderer == null)
			{
				m_TrailRenderer = receiver.GetComponent<TrailRenderer>();
			}
			if( m_TrailRenderer != null)
			{
				switch( m_Method)
				{
					case Method.Clear:
					{
						m_TrailRenderer.Clear();
						break;
					}
				}
			}
		#if UNITY_EDITOR
			else if( receiver != null)
			{
				Debug.LogError( $"{receiver.name} に TrailRenderer が付与されていません", receiver);
			}
		#endif
		}
		enum Method
		{
			Clear
		}
		[Space]
		[SerializeField]
		Method m_Method = Method.Clear;
		[NonSerialized]
		PropertyName m_Identifier;
		[NonSerialized]
		TrailRenderer m_TrailRenderer;
	}
}
