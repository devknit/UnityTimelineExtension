
using UnityEngine;

namespace Knit.TimelineExtension
{
	public abstract class TimeScaleReceiver : MonoBehaviour
	{
		internal protected abstract float GetDefaultTimeScale();
		internal protected abstract void SetTimeScale( float timeScale);
	}
}
