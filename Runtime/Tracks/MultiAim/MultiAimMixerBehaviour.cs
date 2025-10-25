
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations.Rigging;

namespace Knit.TimelineExtension
{
	sealed class MultiAimMixerBehaviour : PlayableBehaviour
	{
		public override void ProcessFrame( Playable playable, FrameData info, object playerData)
		{
			if( playerData is MultiAimConstraint component)
			{
				int inputCount = playable.GetInputCount();
				
				if( m_Component == null)
				{
					if( component.data.sourceObjects.Count > 0)
					{
						var sourceObjects = component.data.sourceObjects;
						
						m_TargetTransform = sourceObjects.GetTransform( 0);
						m_DefaultTargetPosition = m_TargetTransform.position;
						
						m_DefaultTransformWeights = new float[ sourceObjects.Count];
						
						for( int i0 = 0; i0 < m_DefaultTransformWeights.Length; ++i0)
						{
							m_DefaultTransformWeights[ i0] = sourceObjects.GetWeight( i0);
							sourceObjects.SetWeight( i0, 0.0f);
						}
						component.data.sourceObjects = sourceObjects;
					}
					m_DefaultOffsetPosition = component.data.offset;
					m_DefaultWeight = component.weight;
					m_Component = component;
				}
				if( m_Component != null)
				{
					Vector3 offsetPosition = Vector3.zero;
					Vector3 targetPosition = Vector3.zero;
					float targetPositionWeight = 0.0f;
					float totalWeight = 0.0f;
					float weight = 0.0f;
					
					for( int i0 = 0; i0 < inputCount; ++i0)
					{
						var inputPlayable = (ScriptPlayable<MultiAimBehaviour>)playable.GetInput( i0);
						MultiAimBehaviour behaviour = inputPlayable.GetBehaviour();
						float inputWeight = playable.GetInputWeight( i0);
						
						behaviour.Blend( inputWeight, 
							ref weight, ref offsetPosition, 
							ref targetPosition, ref targetPositionWeight);
						totalWeight += inputWeight;
					}
					if( m_TargetTransform != null)
					{
						m_TargetTransform.position = targetPosition + 
							m_DefaultTargetPosition * (1.0f - targetPositionWeight);
						
						var sourceObjects = component.data.sourceObjects;
						sourceObjects.SetWeight( 0, targetPositionWeight);
						component.data.sourceObjects = sourceObjects;
					}
					m_Component.data.offset = offsetPosition + m_DefaultOffsetPosition * (1.0f - totalWeight);
					m_Component.weight = weight + m_DefaultWeight * (1.0f - totalWeight);
				}
			}
		}
		public override void OnPlayableDestroy( Playable playable)
		{
			if( m_Component != null)
			{
				var sourceObjects = m_Component.data.sourceObjects;
				
				if( m_TargetTransform != null)
				{
					m_TargetTransform.position = m_DefaultTargetPosition;
					m_TargetTransform = null;
				}
				for( int i0 = 0; i0 < m_DefaultTransformWeights.Length; ++i0)
				{
					sourceObjects.SetWeight( i0, m_DefaultTransformWeights[ i0]);
				}
				m_Component.data.sourceObjects = sourceObjects;
				m_Component.data.offset = m_DefaultOffsetPosition;
				m_Component.weight = m_DefaultWeight;
				m_Component = null;
			}
		}
		MultiAimConstraint m_Component;
		Transform m_TargetTransform;
		Vector3 m_DefaultTargetPosition;
		Vector3 m_DefaultOffsetPosition;
		float[] m_DefaultTransformWeights;
		float m_DefaultWeight;
	}
}
