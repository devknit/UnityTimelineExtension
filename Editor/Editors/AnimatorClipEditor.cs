
using UnityEngine;
using UnityEngine.Timeline;
using UnityEditor;
using UnityEditor.Timeline;

namespace Knit.TimelineExtension.Editor
{
	[CustomTimelineEditor( typeof( AnimatorClip))]
	sealed class AnimatorClipEditor : ClipEditor
	{
		public override void OnCreate( TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
		{
			if( clip?.asset is AnimatorClip asset && (asset?.AnimationClip?.legacy ?? false) != false)
			{
				asset.AnimationClip = null;
				Debug.LogError("Legacy Animation Clips are not supported");
			}
		}
		public override ClipDrawOptions GetClipOptions( TimelineClip clip)
		{
			var clipOptions = base.GetClipOptions( clip);
			
			if( clip.asset is AnimatorClip asset)
			{
				clipOptions.errorText = GetErrorText( asset, clip.GetParentTrack() as AnimatorTrack, clipOptions.errorText);
			}
			return clipOptions;
		}
		static string GetErrorText( AnimatorClip clip, AnimatorTrack track, string defaultError)
		{
			AnimationClip animationClip = clip?.AnimationClip;
			
			if( animationClip == null)
			{
				return k_NoClipAssignedError;
			}
			if( animationClip.legacy != false)
			{
				return k_LegacyClipError;
			}
			if( animationClip.hasMotionCurves != false || animationClip.hasRootCurves != false)
			{
				// if (track != null && track.trackOffset == TrackOffset.Auto)
				// {
				// 	var animator = track.GetBinding(TimelineEditor.inspectedDirector);
				// 	if (animator != null && !animator.applyRootMotion && !animationAsset.clip.hasGenericRootTransform)
				// 	{
				// 		if (animationAsset.clip.hasMotionCurves)
				// 			return k_MotionCurveError;
				// 		return k_RootCurveError;
				// 	}
				// }
			}
			return defaultError;
		}
		public static readonly string k_NoClipAssignedError = L10n.Tr( "No animation clip assigned");
		public static readonly string k_LegacyClipError = L10n.Tr( "Legacy animation clips are not supported");
		// static readonly string k_MotionCurveError = L10n.Tr( "You are using motion curves without applyRootMotion enabled on the Animator. The root transform will not be animated");
		// static readonly string k_RootCurveError = L10n.Tr( "You are using root curves without applyRootMotion enabled on the Animator. The root transform will not be animated");
	}
}
