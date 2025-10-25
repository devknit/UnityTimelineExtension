#if false

using UnityEngine;
using UnityEditor.Timeline;
using UnityEditor.Timeline.Actions;

namespace Knit.TimelineExtension.Editor
{
	[MenuEntry("Custom Actions/Sample Timeline Action")]
	sealed class AnimatorClipAction : TimelineAction
	{
		public override bool Execute( ActionContext context)
		{
			GameObject gameObject = null;
            if (TimelineEditor.inspectedDirector != null)
                gameObject = TimelineUtility.GetSceneGameObject(TimelineEditor.inspectedDirector, clip.GetParentTrack());
			
            var timeController = TimelineAnimationUtilities.CreateTimeController(clip);
            TimelineAnimationUtilities.EditAnimationClipWithTimeController(
                clipToEdit, timeController, clip.animationClip != null ? gameObject : null);
			
			throw new System.NotImplementedException();
		}
		public override ActionValidity Validate( ActionContext context)
		{
			AnimatorClip animatorClip = null;
			
			foreach( var timelineClip in context.clips)
			{
				if( animatorClip != null)
				{
					return ActionValidity.Invalid;
				}
				if( timelineClip.asset is AnimatorClip clipAsset)
				{
					animatorClip = clipAsset;
				}
			}
			return (animatorClip != null)? ActionValidity.Valid : ActionValidity.NotApplicable;
		}
	}
}
#endif
