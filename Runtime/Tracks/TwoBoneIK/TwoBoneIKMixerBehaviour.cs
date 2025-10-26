
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations.Rigging;

namespace Knit.Timeline
{
	sealed class TwoBoneIKMixerBehaviour : PlayableBehaviour
	{
		public override void ProcessFrame( Playable playable, FrameData info, object playerData)
		{
			if( playerData is TwoBoneIKConstraint component)
			{
				int inputCount = playable.GetInputCount();
				
				if( m_Component == null)
				{
					m_TargetTransform = component.data.target;
					m_HintTransform = component.data.hint;
					m_TargetTransform?.GetPositionAndRotation( 
						out m_DefaultTargetPosition, out m_DefaultTargetRotation);
					m_HintTransform?.GetPositionAndRotation(
						out m_DefaultHintPosition, out m_DefaultHintRotation);
					m_DefaultTargetPositionWeight = component.data.targetPositionWeight;
					m_DefaultTargetRotationWeight = component.data.targetRotationWeight;
					m_DefaultHintWeight = component.data.hintWeight;
					m_DefaultWeight = component.weight;
					m_Component = component;
				}
				if( m_Component != null)
				{
					Vector3 targetPosition = Vector3.zero;
					var targetRotation = new Quaternion( 0, 0, 0, 0);
					Vector3 hintPosition = Vector3.zero;
					var hintRotation = new Quaternion( 0, 0, 0, 0);
					float targetTransformWeight = 0.0f;
					float hintTransformWeight = 0.0f;
					float targetPositionWeight = 0.0f;
					float targetRotationWeight = 0.0f;
					float hintWeight = 0.0f;
					float totalWeight = 0.0f;
					float weight = 0.0f;
					
					for( int i0 = 0; i0 < inputCount; ++i0)
					{
						var inputPlayable = (ScriptPlayable<TwoBoneIKBehaviour>)playable.GetInput( i0);
						TwoBoneIKBehaviour behaviour = inputPlayable.GetBehaviour();
						float inputWeight = playable.GetInputWeight( i0);
						
						behaviour.Blend( inputWeight, ref weight,
							ref targetPositionWeight, ref targetRotationWeight,
							ref targetPosition, ref targetRotation, ref targetTransformWeight,
							ref hintPosition, ref hintRotation, ref hintTransformWeight, ref hintWeight);
						totalWeight += inputWeight;
					}
					if( m_TargetTransform != null)
					{
						targetPosition = Vector3.Lerp( m_DefaultTargetPosition, targetPosition, targetTransformWeight);
						targetRotation = QuaternionUtility.Lerp( m_DefaultTargetRotation, targetRotation, targetTransformWeight, true);
						m_TargetTransform.SetPositionAndRotation( targetPosition, targetRotation);
					}
					if( m_HintTransform != null)
					{
						hintPosition = Vector3.Lerp( m_DefaultHintPosition, hintPosition, hintTransformWeight);
						hintRotation = QuaternionUtility.Lerp( m_DefaultHintRotation, hintRotation, hintTransformWeight, true);
						m_HintTransform.SetPositionAndRotation( hintPosition, hintRotation);
					}
					totalWeight = 1.0f - totalWeight;
					m_Component.data.targetPositionWeight = targetPositionWeight + m_DefaultTargetPositionWeight * totalWeight;
					m_Component.data.targetRotationWeight = targetRotationWeight + m_DefaultTargetRotationWeight * totalWeight;
					m_Component.data.hintWeight = hintWeight + m_DefaultHintWeight * totalWeight;
					m_Component.weight = weight + m_DefaultWeight * totalWeight;
				}
			}
		}
		public override void OnPlayableDestroy( Playable playable)
		{
			if( m_Component != null)
			{
				m_TargetTransform?.SetPositionAndRotation( m_DefaultTargetPosition, m_DefaultTargetRotation);
				m_HintTransform?.SetPositionAndRotation( m_DefaultHintPosition, m_DefaultHintRotation);
				m_Component.data.hintWeight = m_DefaultHintWeight;
				m_Component.weight = m_DefaultWeight;
				m_Component = null;
			}
		}
		Transform m_TargetTransform;
		Transform m_HintTransform;
		Vector3 m_DefaultTargetPosition;
		Quaternion m_DefaultTargetRotation;
		Vector3 m_DefaultHintPosition;
		Quaternion m_DefaultHintRotation;
		float m_DefaultTargetPositionWeight;
		float m_DefaultTargetRotationWeight;
		float m_DefaultHintWeight;
		float m_DefaultWeight;
		TwoBoneIKConstraint m_Component;
	}
}
