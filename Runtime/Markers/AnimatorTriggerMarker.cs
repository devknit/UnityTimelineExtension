
using System;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	[Serializable]
	[CustomStyle( "AnimatorTriggerMarker")]
	sealed class AnimatorTriggerMarker : UniversalMarker, INotification
	{
		public override string PropertyName
		{
			get{ return "Zurp.Timeline.AnimatorTriggerMarker"; }
		}
		public override void OnNotify( UniversalMarkerReceiver receiver, Playable playable, object context)
		{
			if( m_Animator == null)
			{
				m_Animator = receiver.GetComponent<Animator>();
			}
			if( m_Animator != null)
			{
				m_Animator.SetTrigger( m_TriggerName);
			}
		#if UNITY_EDITOR
			else if( receiver != null)
			{
				Debug.LogError( $"{receiver.name} に Animator が付与されていません", receiver);
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
		string m_TriggerName;
		[NonSerialized]
		PropertyName m_Identifier;
		[NonSerialized]
		Animator m_Animator;
	}
}
