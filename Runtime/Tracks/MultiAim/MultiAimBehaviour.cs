
using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Knit.TimelineExtension
{
	[Serializable]
	sealed class MultiAimBehaviour : PlayableBehaviour
	{
		internal void Initialize( Transform target)
		{
			m_TargetTransform = target;
		}
		internal void Blend( float inputWeight, 
			ref float weight, ref Vector3 offsetPosition, 
			ref Vector3 targetPosition, ref float targetPositionWeight)
		{
			if( m_TargetTransform != null)
			{
				targetPosition += m_TargetTransform.position * inputWeight;
				targetPositionWeight += inputWeight;
			}
			weight += m_Weight * inputWeight;
			offsetPosition += m_Offset * inputWeight;
		}
		[SerializeField, Range( 0.0f, 1.0f)]
		float m_Weight = 1.0f;
		[SerializeField]
		Vector3 m_Offset = Vector3.zero;
		[NonSerialized]
		Transform m_TargetTransform;
	}
}
