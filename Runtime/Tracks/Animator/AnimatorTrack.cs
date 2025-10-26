
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.ComponentModel; 
using System.Collections.Generic;

namespace Knit.Timeline
{
	[TrackBindingType( typeof( Animator))]
	[TrackClipType( typeof( AnimatorClip))]
	[TrackColor( 127.0f / 255.0f, 252.0f / 255.0f, 228.0f / 255.0f)]
	[DisplayName( "Knit.Timeline/Animator Track")]
	sealed class AnimatorTrack : TrackAsset, ILayerable
	{
		public override Playable CreateTrackMixer( PlayableGraph graph, GameObject go, int inputCount)
		{
			var playableDirector = go.GetComponent<PlayableDirector>();
			var clipsParTrackCount = new List<int>();
			int humanMotionCount = 0;
			
			clipsParTrackCount.Add( InitializeClips( 
				clipsParTrackCount.Count, ref humanMotionCount));
			
			foreach( var track in GetChildTracks())
			{
				if( track is AnimatorTrack animatorTrack)
				{
					clipsParTrackCount.Add( animatorTrack.InitializeClips( 
						clipsParTrackCount.Count, ref humanMotionCount));
				}
			}
			var trackLayers = new AnimatorTrackLayer[ clipsParTrackCount.Count];
			
			for( int i0 = 0; i0 < trackLayers.Length; ++i0)
			{
				trackLayers[ i0] = new AnimatorTrackLayer( clipsParTrackCount[ i0]);
			}
			var playable = ScriptPlayable<AnimatorMixerBehaviour>.Create( graph, inputCount);
			playable.GetBehaviour().Initialize( playableDirector, trackLayers, humanMotionCount);
			return playable;
		}
		Playable ILayerable.CreateLayerMixer( PlayableGraph graph, GameObject go, int inputCount)
        {
            return Playable.Null;
        }
		int InitializeClips( int trackIndex, ref int humanMotionCount)
		{
			int clipCount = 0;
			
			foreach( var clip in GetClips())
			{
				if( clip.asset is AnimatorClip animatorClip)
				{
					if( animatorClip.Initialize( clip, clipCount, trackIndex) != false)
					{
						++humanMotionCount;
					}
					++clipCount;
				}
			}
			return clipCount;
		}
		public override void GatherProperties( PlayableDirector director, IPropertyCollector driver)
		{
	#if UNITY_EDITOR
			var animator = director.GetGenericBinding( this) as Animator;
			if( animator == null)
			{
				return;
			}
			driver.GatherTransforms( animator.transform);
			driver.AddFromName<Animator>( animator.gameObject, "m_ApplyRootMotion");
			driver.AddFromName<Animator>( animator.gameObject, "m_Controller");
	#endif
			base.GatherProperties( director, driver);
		}
	}
}
