
using System;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

namespace Knit.TimelineExtension
{
	[Serializable]
	// [CustomStyle( "ImpulseGenerateMarker")]
	sealed class ImpulseGenerateMarker : UniversalMarker, INotification
	{
		public override string PropertyName
		{
			get{ return "Zurp.Timeline.ImpulseGenerateMarker"; }
		}
		public override void OnNotify( UniversalMarkerReceiver receiver, Playable playable, object context)
		{
			if( m_ImpulseSource == null)
			{
				m_ImpulseSource = receiver.GetComponent<CinemachineImpulseSource>();
			}
			if( m_ImpulseSource != null)
			{
				switch( m_Method)
				{
					case Method.Default:
					{
						m_ImpulseSource.GenerateImpulse();
						break;
					}
					case Method.Velocity:
					{
						m_ImpulseSource.GenerateImpulseWithVelocity( m_Velocity);
						break;
					}
					case Method.VelocityWithPosition:
					{
						m_ImpulseSource.GenerateImpulseAtPositionWithVelocity( m_Position, m_Velocity);
						break;
					}
				}
			}
		#if UNITY_EDITOR
			else if( receiver != null)
			{
				Debug.LogError( $"{receiver.name} に CinemachineImpulseSource が付与されていません", receiver);
			}
		#endif
		}
		enum Method
		{
			Default,
			Velocity,
			VelocityWithPosition,
		}
		[Space]
		[SerializeField]
		Method m_Method = Method.Velocity;
		[SerializeField]
		Vector3 m_Velocity = Vector3.down;
		[SerializeField]
		Vector3 m_Position = Vector3.zero;
		[NonSerialized]
		CinemachineImpulseSource m_ImpulseSource;
	}
}
