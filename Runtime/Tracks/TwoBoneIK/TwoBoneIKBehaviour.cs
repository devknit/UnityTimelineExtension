
using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Knit.TimelineExtension
{
	[Serializable]
	sealed class TwoBoneIKBehaviour : PlayableBehaviour
	{
		internal void Initialize( Transform target, Transform hint)
		{
			m_TargetTransform = target;
			m_HintTransform = hint;
		}
		internal void Blend( float inputWeight, ref float weight,
			ref float targetPositionWeight, ref float targetRotationWeight, 
			ref Vector3 targetPosition, ref Quaternion targetRotation, ref float targetTransformWeight,
			ref Vector3 hintPosition, ref Quaternion hintRotation, ref float hintTransformWeight, ref float hintWeight)
		{
			Quaternion desiredRotation;
			
			weight += m_Weight * inputWeight;
			
			if( m_TargetTransform != null)
			{
				targetTransformWeight += inputWeight;
				targetPosition += m_TargetTransform.position * inputWeight;
				
				desiredRotation = m_TargetTransform.rotation;
				
				if( Quaternion.Dot( targetRotation, desiredRotation) < 0.0f)
				{
					desiredRotation = desiredRotation.Scale( -1.0f);
				}
				desiredRotation = desiredRotation.Scale( inputWeight);
				targetRotation = targetRotation.Add( desiredRotation);
			}
			targetPositionWeight += m_TargetPositionWeight * inputWeight;
			targetRotationWeight += m_TargetRotationWeight * inputWeight;
			
			if( m_HintTransform != null)
			{
				hintTransformWeight += inputWeight;
				hintPosition += m_HintTransform.position * inputWeight;
				
				desiredRotation = m_HintTransform.rotation;
				
				if( Quaternion.Dot( hintRotation, desiredRotation) < 0.0f)
				{
					desiredRotation = desiredRotation.Scale( -1.0f);
				}
				desiredRotation = desiredRotation.Scale( inputWeight);
				hintRotation = hintRotation.Add( desiredRotation);
			}
			hintWeight += m_HintWeight * inputWeight;
		}
		[SerializeField, Range( 0.0f, 1.0f)]
		float m_Weight = 1.0f;
		[SerializeField, Range( 0.0f, 1.0f)]
		float m_TargetPositionWeight;
		[SerializeField, Range( 0.0f, 1.0f)]
		float m_TargetRotationWeight;
		[SerializeField, Range( 0.0f, 1.0f)]
		float m_HintWeight;
		[NonSerialized]
		Transform m_TargetTransform;
		[NonSerialized]
		Transform m_HintTransform;
	}
}
