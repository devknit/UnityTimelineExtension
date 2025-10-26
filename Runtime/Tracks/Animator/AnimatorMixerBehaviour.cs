
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEngine.Experimental.Animations;

namespace Knit.Timeline
{
	sealed class AnimatorMixerBehaviour : PlayableBehaviour
	{
		AnimatorTrackLayer[] m_TrackLayers;
		
		internal void Initialize( PlayableDirector playableDirector, AnimatorTrackLayer[] trackLayers, int bHumanMotionCount)
		{
			m_PlayableDirector = playableDirector;
			m_TrackLayers = trackLayers;
		#if UNITY_EDITOR
			if( Application.isPlaying == false && bHumanMotionCount > 0)
			{
				m_LayerMixerOffset = 1;
			}
		#endif
		}
		public override void OnPlayableDestroy( Playable playable)
		{
			if( m_PlayableGraph.IsValid() != false)
			{
				m_PlayableGraph.Stop();
				m_PlayableGraph.Destroy();
			}
			if( m_LayerMixerPlayable.IsValid() != false)
			{
				m_LayerMixerPlayable.Destroy();
			}
			for( int i0 = 0; i0 < m_TrackLayers.Length; ++i0)
			{
				m_TrackLayers[ i0]?.Destroy();
			}
		#if UNITY_EDITOR
			if( m_DefaultClipPlayable.IsValid() != false)
			{
				m_DefaultClipPlayable.Destroy();
			}
		#endif
		}
		public override void ProcessFrame( Playable playable, FrameData info, object playerData)
		{
			if( playerData is Animator component)
			{
				int inputCount = playable.GetInputCount();
				
				if( m_Component == null && inputCount > 0)
				{
					m_PlayableGraph = PlayableGraph.Create( component.name + ".AnimatorTrack");
					m_LayerMixerPlayable = AnimationLayerMixerPlayable.Create( 
						m_PlayableGraph, m_TrackLayers.Length + m_LayerMixerOffset);
					
				#if UNITY_EDITOR
					if( m_LayerMixerOffset > 0)
					{
						var defaultHumanoidClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationClip>(
							"Packages/com.unity.timeline/Editor/StyleSheets/res/HumanoidDefault.anim");
						m_DefaultClipPlayable = AnimationClipPlayable.Create( 
							m_PlayableGraph, defaultHumanoidClip);
						m_DefaultClipPlayable.SetRemoveStartOffset( true);
						m_DefaultClipPlayable.SetApplyFootIK( true);
						m_DefaultClipPlayable.SetSpeed( 0.0f);
						m_LayerMixerPlayable.ConnectInput( 0, m_DefaultClipPlayable, 0);
						m_LayerMixerPlayable.SetInputWeight( 0, 1.0f);
					}
				#endif
					for( int i0 = 0; i0 < m_TrackLayers.Length; ++i0)
					{
						AnimatorTrackLayer trackLayer = m_TrackLayers[ i0];
						trackLayer.CreateAnimationMixerPlayable( 
							ref m_PlayableGraph, ref m_LayerMixerPlayable, i0 + m_LayerMixerOffset);
					}
					for( int i0 = 0; i0 < inputCount; ++i0)
					{
						var inputPlayable = (ScriptPlayable<AnimatorBehaviour>)playable.GetInput( i0);
						AnimatorBehaviour behaviour = inputPlayable.GetBehaviour();
						AnimatorTrackLayer trackLayer = m_TrackLayers[ behaviour.m_TrackIndex];
						behaviour.CreatePlayable( m_PlayableGraph, trackLayer, i0);
						trackLayer.SetAnimatorBehaviour( behaviour);
					}
					m_PlayableOutput = AnimationPlayableOutput.Create( 
						m_PlayableGraph, "AnimatorTrack", component);
					m_PlayableOutput.SetAnimationStreamSource( AnimationStreamSource.PreviousInputs);
					m_PlayableOutput.SetSortingOrder( 500); // default( 100) + 400
					m_PlayableOutput.SetSourcePlayable( m_LayerMixerPlayable);
					m_PlayableGraph.SetTimeUpdateMode( DirectorUpdateMode.GameTime);
					m_PlayableGraph.Play();
					m_Component = component;
				}
				if( m_Component != null)
				{
					double trackTime = m_PlayableDirector.time;
					float deltaTime = info.deltaTime;
					float totalWeight = 0.0f;
					bool paused = 
				#if !UNITY_EDITOR
						false;
				#else
						Application.isPlaying == false && m_PlayableDirector.state == PlayState.Paused;
				#endif
					m_BehaviourIndex = (m_BehaviourIndex + 1) % 2;
					
					for( int i0 = 0; i0 < m_TrackLayers.Length; ++i0)
					{
						m_TrackLayers[ i0].Process( ref playable, ref m_LayerMixerPlayable, paused, 
							trackTime, deltaTime, m_BehaviourIndex, ref totalWeight, i0 + m_LayerMixerOffset);
					}
					if( totalWeight > 0.0f && totalWeight < 1.0f && m_LayerMixerOffset == 0)
					{
						float f = 1.0f / totalWeight;
						
						for( int i0 = 0; i0 < m_TrackLayers.Length; ++i0)
						{
							m_LayerMixerPlayable.SetInputWeight( i0, m_TrackLayers[ i0].TotalWeight * f);
						}
					}
				#if UNITY_EDITOR
					if( m_LayerMixerOffset == 0)
				#endif
					{
						m_PlayableOutput.SetWeight( totalWeight);
					}
                }
			}
		}
		Animator m_Component;
		PlayableGraph m_PlayableGraph;
		PlayableDirector m_PlayableDirector;
		AnimationPlayableOutput m_PlayableOutput;
		AnimationLayerMixerPlayable m_LayerMixerPlayable;
		int m_BehaviourIndex;
		int m_LayerMixerOffset;
	#if UNITY_EDITOR
		AnimationClipPlayable m_DefaultClipPlayable;
	#endif
	}
}
