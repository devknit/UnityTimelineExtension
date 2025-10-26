
using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	[Serializable]
	sealed class TransformBehaviour : PlayableBehaviour
	{
		internal void Initialize( IExposedPropertyTable resolver)
		{
			m_ConstraintTransform = m_Constraint.Resolve( resolver);
		}
		internal void BlendWorld( float inputWeight,
			ref Vector3 worldPosition, ref float worldPositionWeight,
			ref Quaternion worldRotation, ref float worldRotationWeight,
			ref Vector3 worldScale, ref float worldScaleWeight)
		{
			if( m_ConstraintTransform != null)
			{
				Transform constraintTransform = m_ConstraintTransform;
				Quaternion desiredRotation;
				float weight;
				
				weight = inputWeight * m_ConstraintWeight;
				worldPositionWeight += weight;
				worldPosition += constraintTransform.position * weight;
				
				weight = inputWeight * m_ConstraintWeight;
				worldRotationWeight += weight;
				desiredRotation = constraintTransform.rotation;
				
				if( Quaternion.Dot( worldRotation, desiredRotation) < 0.0f)
				{
					desiredRotation = desiredRotation.Scale( -1.0f);
				}
				desiredRotation = desiredRotation.Scale( weight);
				worldRotation = worldRotation.Add( desiredRotation);
				
				weight = inputWeight * m_ConstraintWeight;
				worldScaleWeight += weight;
				worldScale += constraintTransform.lossyScale * weight;
			}
		}
		internal void BlendLocal( float inputWeight,
			ref Vector3 localPosition, ref float localPositionWeight,
			ref Quaternion localRotation, ref float localRotationWeight,
			ref Vector3 localScale, ref float localScaleWeight,
			ref Quaternion worldRotation)
		{
			if( m_PositionWeight > 0.0f)
			{
				float weight = inputWeight * m_PositionWeight;
				localPositionWeight += weight;
				
				localPosition += m_LocalPositionAxis switch
				{
					PositionAxis.World => Quaternion.identity,
					PositionAxis.Local => worldRotation,
					_ => Quaternion.identity
				} * m_LocalPosition * weight;
			}
			if( m_RotationWeight > 0.0f)
			{
				float weight = inputWeight * m_RotationWeight;
				Quaternion desiredRotation;
				
				localRotationWeight += weight;
				desiredRotation = Quaternion.Euler( m_LocalRotation);
				
				if( Quaternion.Dot( localRotation, desiredRotation) < 0.0f)
				{
					desiredRotation = desiredRotation.Scale( -1.0f);
				}
				desiredRotation = desiredRotation.Scale( weight);
				localRotation = localRotation.Add( desiredRotation);
			}
			if( m_ScaleWeight > 0)
			{
				float weight = inputWeight * m_ScaleWeight;
				localScaleWeight += weight;
				localScale += m_LocalScale * weight;
			}
		}
		[SerializeField]
		ExposedReference<Transform> m_Constraint;
		[NonSerialized]
		Transform m_ConstraintTransform;
		[SerializeField, Range( 0.0f, 1.0f)]
		float m_ConstraintWeight = 1.0f;
		[SerializeField]
		PositionAxis m_LocalPositionAxis = PositionAxis.Local;
		[SerializeField]
		Vector3 m_LocalPosition = Vector3.zero;
		[SerializeField, Range( 0.0f, 1.0f)]
		float m_PositionWeight = 1.0f;
		[SerializeField]
		Vector3 m_LocalRotation = Vector3.zero;
		[SerializeField, Range( 0.0f, 1.0f)]
		float m_RotationWeight = 1.0f;
		[SerializeField]
		Vector3 m_LocalScale = Vector3.one;
		[SerializeField, Range( 0.0f, 1.0f)]
		float m_ScaleWeight = 1.0f;
	}
}
