
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System.Collections.Generic;

namespace Knit.Timeline
{
	internal class AnimatorTrackLayer
	{
		internal AnimatorTrackLayer( int clipCount)
		{
			m_Behaviours = new AnimatorBehaviour[ clipCount];
			m_ActiveBehaviours = new Dictionary<AnimationClip, AnimatorBehaviour>[]{ new(), new() };
		}
		internal void CreateAnimationMixerPlayable( ref PlayableGraph playableGraph,
			ref AnimationLayerMixerPlayable layerMixerPlayable, int inputIndex)
		{
			if( m_MixerPlayable.IsValid() == false)
			{
				m_MixerPlayable = AnimationMixerPlayable.Create( playableGraph, m_Behaviours.Length);
				layerMixerPlayable.ConnectInput( inputIndex, m_MixerPlayable, 0);
				layerMixerPlayable.SetInputWeight( inputIndex, 1.0f);
			}
		}
		internal void ConnectClipPlayable( int inputIndex, ref AnimationClipPlayable clipPlayable, int outputIndex)
		{
			m_MixerPlayable.ConnectInput( inputIndex, clipPlayable, outputIndex);
		}
		internal void SetAnimatorBehaviour( AnimatorBehaviour behaviour)
		{
			m_Behaviours[ behaviour.m_ClipIndex] = behaviour;
		}
		internal void Process( ref Playable playable, ref AnimationLayerMixerPlayable layerMixerPlayable, 
			bool paused, double trackTime, double deltaTime, int swapIndex, ref float totalWeight, int inputIndex)
		{
			AnimatorBehaviour activeBehaviour = null;
			Dictionary<AnimationClip, AnimatorBehaviour> prevBehaviours = m_ActiveBehaviours[ swapIndex];
			Dictionary<AnimationClip, AnimatorBehaviour> nextBehaviours = m_ActiveBehaviours[ (swapIndex + 1) % 2];
			int activeBehaviourCount = 0;
			
			m_TotalWeight = 0.0f;
			nextBehaviours.Clear();
			
			for( int i1 = 0; i1 < m_Behaviours.Length; ++i1)
			{
				AnimatorBehaviour behaviour = m_Behaviours[ i1];
				float inputWeight = playable.GetInputWeight( behaviour.m_InputIndex);
				
				if( behaviour.Process( paused, 
					prevBehaviours, nextBehaviours,
					trackTime, deltaTime, inputWeight) != false)
				{
					activeBehaviour = behaviour;
					++activeBehaviourCount;
				}
				m_MixerPlayable.SetInputWeight( i1, inputWeight);
				m_TotalWeight += inputWeight;
			}
			if( totalWeight < m_TotalWeight)
			{
				totalWeight = m_TotalWeight;
			}
			if( activeBehaviourCount == 1)
			{
				m_MixerPlayable.SetInputWeight( activeBehaviour.m_ClipIndex, 1.0f);
			}
			layerMixerPlayable.SetInputWeight( inputIndex, m_TotalWeight);
		}
		internal void Destroy()
		{
			if( m_MixerPlayable.IsValid() != false)
			{
				m_MixerPlayable.Destroy();
			}
		}
		internal float TotalWeight
		{
			get{ return m_TotalWeight; }
		}
        readonly internal AnimatorBehaviour[] m_Behaviours;
        readonly Dictionary<AnimationClip, AnimatorBehaviour>[] m_ActiveBehaviours;
		AnimationMixerPlayable m_MixerPlayable;
		float m_TotalWeight;
	}
}
