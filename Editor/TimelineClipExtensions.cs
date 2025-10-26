
using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;

namespace Knit.Timeline.Editor
{
	static class TimelineClipExtensions
	{
		[InitializeOnLoadMethod]
		static void OverrideAnimationClipProperty()
		{
			unsafe
			{
				var pointer0 = typeof( TimelineClip).GetProperty( "animationClip").GetMethod.MethodHandle.Value.ToPointer();
				var pointer1 = typeof( TimelineClipExtensions).GetMethod( "GetAnimationClip").MethodHandle.Value.ToPointer();
				*(int*)new IntPtr((int*)pointer0).ToPointer() = *(int*)new IntPtr((int*)pointer1).ToPointer();
			}
		}
		public static AnimationClip GetAnimationClip( this TimelineClip timelineClip)
		{
			return timelineClip.asset switch
			{
				AnimationPlayableAsset animation => animation.clip,
				AnimatorClip animator => animator.AnimationClip,
				_ => null
			};
		}
	}
}
