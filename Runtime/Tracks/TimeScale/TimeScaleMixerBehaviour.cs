
using UnityEngine;
using UnityEngine.Playables;

namespace Knit.TimelineExtension
{
	sealed class TimeScaleMixerBehaviour : PlayableBehaviour
	{
		public override void ProcessFrame( Playable playable, FrameData info, object playerData)
		{
			int inputCount = playable.GetInputCount();
			float blendedTimeScale = 0.0f;
			float totalWeight = 0.0f;
			
			for( int i0 = 0; i0 < inputCount; ++i0)
			{
				float inputWeight = playable.GetInputWeight( i0);
				
				if( inputWeight > 0.0f)
				{
					var inputPlayable = (ScriptPlayable<TimeScaleBehaviour>)playable.GetInput( i0);
					blendedTimeScale += inputPlayable.GetBehaviour().m_TimeScale * inputWeight;
					totalWeight += inputWeight;
				}
			}
			if( m_TimeScaleReceiver == null)
			{
				m_TimeScaleReceiver = playerData as TimeScaleReceiver;
			}
			if( m_TimeScaleReceiver != null)
			{
				m_TimeScaleReceiver.SetTimeScale( blendedTimeScale + 
					m_TimeScaleReceiver.GetDefaultTimeScale() * (1.0f - totalWeight));
			}
			else
			{
				Time.timeScale = blendedTimeScale + 1.0f * (1.0f - totalWeight);;
			}
		}
		public override void OnBehaviourPause( Playable playable, FrameData info)
		{
			SetDefaultTimeScale();
		}
		public override void OnGraphStop( Playable playable)
		{
			SetDefaultTimeScale();
		}
		public override void OnPlayableDestroy( Playable playable)
		{
			if( SetDefaultTimeScale() != false)
			{
				m_TimeScaleReceiver = null;
			}
		}
		bool SetDefaultTimeScale()
		{
			if( m_TimeScaleReceiver != null)
			{
				m_TimeScaleReceiver.SetTimeScale( m_TimeScaleReceiver.GetDefaultTimeScale());
				return true;
			}
			else
			{
				Time.timeScale = 1.0f;
			}
			return false;
		}
		TimeScaleReceiver m_TimeScaleReceiver;
	}
}
