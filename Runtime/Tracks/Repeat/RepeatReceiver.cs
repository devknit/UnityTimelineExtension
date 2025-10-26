
using UnityEngine;

namespace Knit.Timeline
{
	public interface IRepeatTrack
	{
		ISeekTarget GetDefaultTarget();
		ISeekTarget[] GetOtherTargets();
		bool Abort( string targetName);
	}
	public abstract class RepeatReceiver : MonoBehaviour
	{
		internal protected abstract void OnJoin( IRepeatTrack track);
		internal protected abstract void OnLeave();
		internal protected abstract bool OnSeek( ISection target);
	}
}
