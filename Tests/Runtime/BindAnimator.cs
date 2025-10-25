
using UnityEngine;

namespace Knit.TimelineExtension.Test
{
	public class BindAnimator : MonoBehaviour, IPlayableAssetInfo
	{
		public bool TryGetValue( string key, out Object hitObject)
		{
			hitObject = null;
			
			Animator animator = GetComponent<Animator>();
			
			if( animator != null)
			{
				foreach( var clip in animator.runtimeAnimatorController.animationClips)
				{
					if( clip.name.Equals( key) != false)
					{
						hitObject = clip;
						return true;
					}
				}
			}
			return false;
		}
	}
}