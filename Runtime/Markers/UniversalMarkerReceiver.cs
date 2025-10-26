
using UnityEngine;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	public class UniversalMarkerReceiver : MonoBehaviour, INotificationReceiver
	{
		public virtual void OnNotify( Playable playable, INotification notification, object context)
		{
			if( notification is UniversalMarker marker)
			{
				marker.OnNotify( this, playable, context);
			}
		}
	}
}
