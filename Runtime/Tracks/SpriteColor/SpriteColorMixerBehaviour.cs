
using UnityEngine;
using UnityEngine.Playables;

namespace Knit.TimelineExtension
{
	sealed class SpriteColorMixerBehaviour : PlayableBehaviour
	{
		public override void ProcessFrame( Playable playable, FrameData info, object playerData)
		{
			if( playerData is SpriteRenderer component)
			{
				if( m_Component == null)
				{
					m_DefaultColor = component.color;
					m_Component = component;
				}
				if( m_Component != null)
				{
					int inputCount = playable.GetInputCount();
					Color blendedColor = Color.clear;
					float totalWeight = 0;
					
					for( int i0 = 0; i0 < inputCount; ++i0)
					{
						float weight = playable.GetInputWeight( i0);
						
						if( playable.GetInputWeight( i0) > 0.0f)
						{
							var inputPlayable = (ScriptPlayable<SpriteColorBehaviour>)playable.GetInput( i0);
							blendedColor += inputPlayable.GetBehaviour().m_Color * weight;
							totalWeight += weight;
						}
					}
					m_Component.color = blendedColor + m_DefaultColor * (1.0f - totalWeight);
				}
			}
		}
		public override void OnPlayableDestroy( Playable playable)
		{
			if( m_Component != null)
			{
				m_Component.color = m_DefaultColor;
				m_Component = null;
			}
		}
		Color m_DefaultColor;
		SpriteRenderer m_Component;
	}
}
