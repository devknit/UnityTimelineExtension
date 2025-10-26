
using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace Knit.Timeline
{
	public static partial class Extension
	{
		static readonly Type s_AnimationClipPlayable = typeof( AnimationClipPlayable);
		static readonly MethodInfo s_SetRemoveStartOffset;
		static readonly MethodInfo s_SetOverrideLoopTime;
		static readonly MethodInfo s_SetLoopTime;
		
		static Extension()
		{
			s_SetRemoveStartOffset = s_AnimationClipPlayable.GetMethod( 
				"SetRemoveStartOffset", BindingFlags.Instance | BindingFlags.NonPublic);
			s_SetOverrideLoopTime = s_AnimationClipPlayable.GetMethod( 
				"SetOverrideLoopTime", BindingFlags.Instance | BindingFlags.NonPublic);
			s_SetLoopTime = s_AnimationClipPlayable.GetMethod( 
				"SetLoopTime", BindingFlags.Instance | BindingFlags.NonPublic);
			
		#if false
			var paramSelf = Expression.Parameter( typeof( AnimationClipPlayable));
			var paramValue = Expression.Parameter( typeof( bool), "value");
			MethodInfo SetRemoveStartOffsetInfo = s_AnimationClipPlayable.GetMethod( 
				"SetRemoveStartOffset",
				BindingFlags.Instance | BindingFlags.NonPublic,
				null, 
				CallingConventions.Any,
				new[]{ typeof( bool) },
				null);
			MethodCallExpression SetRemoveStartOffsetCall = Expression.Call( SetRemoveStartOffsetInfo, paramSelf, paramValue);
			s_SetRemoveStartOffset = Expression.Lambda<Action<AnimationClipPlayable, bool>>( SetRemoveStartOffsetCall, paramValue).Compile();
		#endif
		}
		public static void SetRemoveStartOffset( this AnimationClipPlayable clipPlayable, bool value)
		{
			s_SetRemoveStartOffset.Invoke( clipPlayable, new object[]{ value });
		}
		public static void SetOverrideLoopTime( this AnimationClipPlayable clipPlayable, bool value)
		{
			s_SetOverrideLoopTime.Invoke( clipPlayable, new object[]{ value });
		}
		public static void SetLoopTime( this AnimationClipPlayable clipPlayable, bool value)
		{
			s_SetLoopTime.Invoke( clipPlayable, new object[]{ value });
		}
		public static IEnumerable<TrackAsset> GetTracks( this TimelineAsset timelineAsset, Type trackAssetType)
		{
			if( timelineAsset != null)
			{
				foreach( var track in timelineAsset.GetRootTracks())
				{
					if( (trackAssetType?.IsAssignableFrom( track.GetType()) ?? true) != false)
					{
						yield return track;
					}
					var it = track.GetTracks( trackAssetType).GetEnumerator();
					
					while( it.MoveNext() != false)
					{
						yield return it.Current;
					}
				}
			}
		}
		public static IEnumerable<TrackAsset> GetTracks( this TrackAsset trackAsset, Type trackAssetType)
		{
			if( trackAsset != null)
			{
				foreach( var track in trackAsset.GetChildTracks())
				{
					if( (trackAssetType?.IsAssignableFrom( track.GetType()) ?? true) != false)
					{
						yield return track;
					}
					var it = track.GetTracks( trackAssetType).GetEnumerator();
					
					while( it.MoveNext() != false)
					{
						yield return it.Current;
					}
				}
			}
		}
		public static IEnumerable<TimelineClip> GetClips( this TimelineAsset timelineAsset, Type trackAssetType, Type clipAssetType)
		{
			if( timelineAsset != null)
			{
				foreach( var track in timelineAsset.GetTracks( trackAssetType))
				{
					foreach( var clip in track.GetClips().OrderBy( x => x.start))
					{
						if( (clipAssetType?.IsAssignableFrom( clip.asset?.GetType()) ?? true) != false)
						{
							yield return clip;
						}
					}
				}
			}
		}
		public static IEnumerable<TimelineClip> GetClips( this TrackAsset trackAsset, Type trackAssetType, Type clipAssetType)
		{
			if( trackAsset != null)
			{
				foreach( var track in trackAsset.GetTracks( trackAssetType))
				{
					foreach( var clip in track.GetClips().OrderBy( x => x.start))
					{
						if( (clipAssetType?.IsAssignableFrom( clip.asset?.GetType()) ?? true) != false)
						{
							yield return clip;
						}
					}
				}
			}
		}
	}
}
