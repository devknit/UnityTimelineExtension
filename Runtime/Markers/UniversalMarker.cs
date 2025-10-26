
using System;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	public abstract class UniversalMarker : Marker, INotification, INotificationOptionProvider
    {
		public PropertyName id
		{
			get
			{
				if( m_Identifier == null)
				{
					m_Identifier = new PropertyName( PropertyName);
				}
				return m_Identifier;
			}
		}
		public abstract string PropertyName{ get; }
		public abstract void OnNotify( UniversalMarkerReceiver receiver, Playable playable, object context);
		
		NotificationFlags INotificationOptionProvider.flags
		{
			get
			{
				NotificationFlags flags = 0;
				
				if( m_InEditMode != false)
				{
					flags |= NotificationFlags.TriggerInEditMode;
				}
				if( m_Retroactive != false)
				{
					flags |= NotificationFlags.Retroactive;
				}
				if( m_EmitOnce != false)
				{
					flags |= NotificationFlags.TriggerOnce;
				}
				return flags;				
			}
		}
		[SerializeField]
		bool m_InEditMode = true;
		[SerializeField]
		bool m_Retroactive = false;
		[SerializeField]
		bool m_EmitOnce = false;
		[NonSerialized]
		PropertyName m_Identifier;
	}
}
