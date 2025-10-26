
using UnityEngine;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	sealed class LightMixerBehaviour : PlayableBehaviour
	{
		public override void ProcessFrame( Playable playable, FrameData info, object playerData)
		{
			if( playerData is Light component)
			{
				if( m_Component == null)
				{
					m_Component = component;
					m_DefaultColor = component.color;
					m_DefaultIntensity = component.intensity;
					m_DefaultRange = component.range;
					m_DefaultSpotAngle = component.spotAngle;
					m_DefaultInnerSpotAngle = component.innerSpotAngle;
					m_DefaultBounceIntensity = component.bounceIntensity;
					m_DefaultColorTemperature = component.colorTemperature;
				}
				if( m_Component != null)
				{
				#if UNITY_EDITOR
					if( m_Component.lightmapBakeType != LightmapBakeType.Realtime)
					{
						Debug.LogWarning( "Light の Mode が Realtime ではないため制御できません");
						return;
					}
				#endif
					int inputCount = playable.GetInputCount();
					
					Color blendedColor = Color.clear;
					float blendedIntensity = 0.0f;
					float blendedRange = 0.0f;
					float blendedSpotAngle = 0.0f;
					float blendedInnerSpotAngle = 0.0f;
					float blendedBounceIntensity = 0.0f;
					float blendedColorTemperature = 0.0f;
					float totalWeight = 0.0f;
					
					for( int i0 = 0; i0 < inputCount; ++i0)
					{
						float inputWeight = playable.GetInputWeight( i0);
						
						if( inputWeight > 0.0f)
						{
							var inputPlayable = (ScriptPlayable<LightBehaviour>)playable.GetInput( i0);
							LightBehaviour behaviour = inputPlayable.GetBehaviour();
							
							blendedColor += behaviour.m_Color * inputWeight;
							blendedIntensity += behaviour.m_Intensity * inputWeight;
							blendedRange += behaviour.m_Range * inputWeight;
							blendedSpotAngle += behaviour.m_SpotAngle * inputWeight;
							blendedInnerSpotAngle += behaviour.m_InnerSpotAngle * inputWeight;
							blendedBounceIntensity += behaviour.m_BounceIntensity * inputWeight;
							blendedColorTemperature += behaviour.m_ColorTemperature * inputWeight;
							totalWeight += inputWeight;
						}
					}
					totalWeight = 1.0f - totalWeight;
					m_Component.color = blendedColor + m_DefaultColor * totalWeight;
					m_Component.intensity = blendedIntensity + m_DefaultIntensity * totalWeight;
					m_Component.range = blendedRange + m_DefaultRange * totalWeight;
					m_Component.spotAngle = blendedSpotAngle + m_DefaultSpotAngle * totalWeight;
					m_Component.innerSpotAngle = blendedInnerSpotAngle + m_DefaultInnerSpotAngle * totalWeight;
					m_Component.bounceIntensity = blendedBounceIntensity + m_DefaultBounceIntensity * totalWeight;
					m_Component.colorTemperature = blendedColorTemperature + m_DefaultColorTemperature * totalWeight;
				}
			}
		}
		public override void OnPlayableDestroy( Playable playable)
		{
			if( m_Component != null)
			{
				m_Component.color = m_DefaultColor;
				m_Component.intensity = m_DefaultIntensity;
				m_Component.range = m_DefaultRange;
				m_Component.spotAngle = m_DefaultSpotAngle;
				m_Component.innerSpotAngle = m_DefaultInnerSpotAngle;
				m_Component.bounceIntensity = m_DefaultBounceIntensity;
				m_Component.colorTemperature = m_DefaultColorTemperature;
				m_Component = null;
			}
		}
		Light m_Component;
		Color m_DefaultColor;
		float m_DefaultIntensity;
		float m_DefaultRange;
		float m_DefaultSpotAngle;
		float m_DefaultInnerSpotAngle;
		float m_DefaultBounceIntensity;
		float m_DefaultColorTemperature;
	}
}
