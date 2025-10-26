
using UnityEngine;

namespace Knit.Timeline
{
	public abstract class TimeScaleReceiver : MonoBehaviour
	{
		internal protected abstract float GetDefaultTimeScale();
		internal protected abstract void SetTimeScale( float timeScale);
	}
}
