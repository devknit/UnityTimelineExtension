
using System;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.TimelineExtension
{
	[Serializable]
	[CustomStyle( "ParticleSystemMarker")]
	sealed class ParticleSystemMarker : UniversalMarker, INotification
	{
		public override string PropertyName
		{
			get{ return "Zurp.Timeline.ParticleSystemMarker"; }
		}
		public override void OnNotify( UniversalMarkerReceiver receiver, Playable playable, object context)
		{
			if( m_ParticleSystem == null)
			{
				m_ParticleSystem = receiver.GetComponent<ParticleSystem>();
			}
			if( m_ParticleSystem != null)
			{
				switch( m_Method)
				{
					case Method.Play:
					{
						m_ParticleSystem.Play( m_WithChildren);
						break;
					}
					case Method.Pause:
					{
						m_ParticleSystem.Pause( m_WithChildren);
						break;
					}
					case Method.Stop:
					{
						m_ParticleSystem.Stop( m_WithChildren);
						break;
					}
				}
			}
		#if UNITY_EDITOR
			else if( receiver != null)
			{
				Debug.LogError( $"{receiver.name} に ParticleSystem が付与されていません", receiver);
			}
		#endif
		}
		enum Method
		{
			Play,
			Pause,
			Stop,
		}
		[Space]
		[SerializeField]
		Method m_Method = Method.Stop;
		[SerializeField]
		bool m_WithChildren = true;
		[NonSerialized]
		PropertyName m_Identifier;
		[NonSerialized]
		ParticleSystem m_ParticleSystem;
	}
}
