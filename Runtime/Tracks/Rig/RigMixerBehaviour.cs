
using UnityEngine.Playables;
using UnityEngine.Animations.Rigging;

namespace Knit.TimelineExtension
{
	sealed class RigMixerBehaviour : PlayableBehaviour
	{
		public override void ProcessFrame( Playable playable, FrameData info, object playerData)
		{
			if( playerData is Rig component)
			{
				if( m_Component == null)
				{
					m_DefaultWeight = component.weight;
					m_Component = component;
				}
				if( m_Component != null)
				{
					int inputCount = playable.GetInputCount();
					float blendedWeight = 0.0f;
					float totalWeight = 0.0f;
					
					for( int i0 = 0; i0 < inputCount; ++i0)
					{
						float inputWeight = playable.GetInputWeight( i0);
						
						if( inputWeight > 0.0f)
						{
							var inputPlayable = (ScriptPlayable<RigBehaviour>)playable.GetInput( i0);
							blendedWeight += inputPlayable.GetBehaviour().m_Weight * inputWeight;
							totalWeight += inputWeight;
						}
					}
					m_Component.weight = blendedWeight + m_DefaultWeight * (1.0f - totalWeight);
				}
			}
		}
		public override void OnPlayableDestroy( Playable playable)
		{
			if( m_Component != null)
			{
				m_Component.weight = m_DefaultWeight;
				m_Component = null;
			}
		}
		Rig m_Component;
		float m_DefaultWeight;
	}
}
