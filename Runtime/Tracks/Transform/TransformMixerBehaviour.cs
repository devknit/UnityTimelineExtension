#define WITH_BEHAVIOUR_CACHE

using UnityEngine;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	sealed class TransformMixerBehaviour : PlayableBehaviour
	{
		internal void Initialize( PositionVolume positionVolume)
		{
			m_PositionVolume = positionVolume;
		}
		public override void ProcessFrame( Playable playable, FrameData info, object playerData)
		{
			if( playerData is Transform component)
			{
				Transform parentTransform = component?.parent;
				Quaternion parentRotation = Quaternion.identity;
				Vector3 parentPosition = Vector3.zero;
				Vector3 parentScale = Vector3.one;
				
				if( parentTransform != null)
				{
					parentRotation = parentTransform.rotation;
					parentPosition = parentTransform.position;
					parentScale = parentTransform.lossyScale;
				}
				if( m_Component == null)
				{
					m_Component = component;
					m_DefaultLocalRotation = component.localRotation;
					m_DefaultLocalPosition = component.localPosition;
					m_DefaultLocalScale = component.localScale;
				}
				if( m_Component != null)
				{
					var worldRotation = new Quaternion( 0, 0, 0, 0);
					var localRotation = new Quaternion( 0, 0, 0, 0);
					Vector3 worldPosition = Vector3.zero;
					Vector3 localPosition = Vector3.zero;
					Vector3 worldScale = Vector3.zero;
					Vector3 localScale = Vector3.zero;
					float worldPositionWeight = 0.0f;
					float worldRotationWeight = 0.0f;
					float worldScaleWeight = 0.0f;
					float localPositionWeight = 0.0f;
					float localRotationWeight = 0.0f;
					float localScaleWeight = 0.0f;
					
				#if WITH_BEHAVIOUR_CACHE
					for( int i0 = 0; i0 < m_Behaviours.Length; ++i0)
					{
						TransformBehaviour behaviour = m_Behaviours[ i0];
				#else
					int inputCount = playable.GetInputCount();
					
					for( int i0 = 0; i0 < inputCount; ++i0)
					{
						var inputPlayable = (ScriptPlayable<TransformBehaviour>)playable.GetInput( i0);
						TransformBehaviour behaviour = inputPlayable.GetBehaviour();
				#endif
						float inputWeight = playable.GetInputWeight( i0);
						
						if( inputWeight > 0.0f)
						{
							behaviour.BlendWorld( inputWeight,
								ref worldPosition, ref worldPositionWeight,
								ref worldRotation, ref worldRotationWeight,
								ref worldScale, ref worldScaleWeight);
						}
					}
					worldPosition = Vector3.Lerp( parentPosition, worldPosition, worldPositionWeight);
					worldRotation = QuaternionUtility.Lerp( parentRotation, worldRotation, worldRotationWeight, true);
					worldScale = Vector3.Lerp( parentScale, worldScale, worldScaleWeight);
					
				#if WITH_BEHAVIOUR_CACHE
					for( int i0 = 0; i0 < m_Behaviours.Length; ++i0)
					{
						TransformBehaviour behaviour = m_Behaviours[ i0];
				#else
					for( int i0 = 0; i0 < inputCount; ++i0)
					{
						var inputPlayable = (ScriptPlayable<TransformBehaviour>)playable.GetInput( i0);
						TransformBehaviour behaviour = inputPlayable.GetBehaviour();
				#endif
						float inputWeight = playable.GetInputWeight( i0);
						
						if( inputWeight > 0.0f)
						{
							behaviour.BlendLocal( inputWeight,
								ref localPosition, ref localPositionWeight,
								ref localRotation, ref localRotationWeight,
								ref localScale, ref localScaleWeight, ref worldRotation);
						}
					}
					switch( m_PositionVolume)
					{
						case PositionVolume.World:
						{
							localPosition += Vector3.Scale( 
								m_DefaultLocalPosition, parentScale) * (1.0f - localPositionWeight);
							break;
						}
						case PositionVolume.Local:
						{
							localPosition = Vector3.Scale( localPosition + 
								m_DefaultLocalPosition * (1.0f - localPositionWeight), parentScale);
							break;
						}
						case PositionVolume.LocalWithConstraint:
						{
							localPosition = Vector3.Scale( localPosition + 
								m_DefaultLocalPosition * (1.0f - localPositionWeight), 
								Vector3.Lerp( parentScale, worldScale, worldScaleWeight));
							break;
						}
					}
					worldPosition += localPosition;
					worldRotation *= QuaternionUtility.Lerp( m_DefaultLocalRotation, localRotation, localRotationWeight, false);
					m_Component.SetPositionAndRotation( worldPosition, worldRotation);
					
					localScale += m_DefaultLocalScale * (1.0f - localScaleWeight);
					m_Component.localScale = localScale;
				}
			}
		}
		public override void OnPlayableDestroy( Playable playable)
		{
			if( m_Component != null)
			{
				m_Component.localPosition = m_DefaultLocalPosition;
				m_Component.localRotation = m_DefaultLocalRotation;
				m_Component.localScale = m_DefaultLocalScale;
				m_Component = null;
			}
		}
	#if WITH_BEHAVIOUR_CACHE
		public override void OnGraphStart( Playable playable)
		{
			int inputCount = playable.GetInputCount();
			
			m_Behaviours = new TransformBehaviour[ inputCount];
			
			for( int i0 = 0; i0 < inputCount; ++i0)
			{
				m_Behaviours[ i0] = ((ScriptPlayable<TransformBehaviour>)playable.GetInput( i0)).GetBehaviour();
			}
		}
		TransformBehaviour[] m_Behaviours;
	#endif
		PositionVolume m_PositionVolume;
		Quaternion m_DefaultLocalRotation;
		Vector3 m_DefaultLocalPosition;
		Vector3 m_DefaultLocalScale;
		Transform m_Component;
	}
	internal enum PositionVolume
	{
		World,
		Local,
		LocalWithConstraint,
	}
	internal enum PositionAxis
	{
		World,
		Local,
	}
}
