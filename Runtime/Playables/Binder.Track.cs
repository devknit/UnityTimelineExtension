
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

namespace Knit.Timeline
{
	public static partial class Binder
	{
		public static void BindTrack( this TrackAsset track, PlayableDirector playableDirector, Dictionary<string, HashSet<BindableObject>> objects)
		{
			string[] targetCommands = track.name.Split( kBindableSymbol);
			
			for( int i0 = 1; i0 < targetCommands.Length; ++i0)
			{
				Object collectObj = FindBindingObject( targetCommands[ i0], objects, null, false, out _, out _);
				
				if( collectObj != null)
				{
					Bind( track, playableDirector, collectObj);
					break;
				}
			}
		}
		public static void UnBindTrack( this TrackAsset track, PlayableDirector playableDirector)
		{
			if( track != null && playableDirector != null)
			{
				string[] targetCommands = track.name.Split( kBindableSymbol);
				
				if( targetCommands.Length > 1)
				{
					foreach( var binding in track.outputs)
					{
						playableDirector.SetGenericBinding( binding.sourceObject, null);
					}
				}
			}
		}
		static void Bind( TrackAsset track, PlayableDirector playableDirector, Object collectObj)
		{
			foreach( var binding in track.outputs)
			{
				if( binding.outputTargetType != null)
				{
					if( binding.outputTargetType == typeof( GameObject))
					{
						GameObject gameObject = collectObj switch
						{
							GameObject source => source,
							Component component => component.gameObject,
							_ => null
						};
						playableDirector.SetGenericBinding( binding.sourceObject, gameObject);
					}
					else if( binding.outputTargetType == typeof( Transform))
					{
						Transform transform = collectObj switch
						{
							Transform source => source,
							GameObject gameObject => gameObject.transform,
							Component component => component.transform,
							_ => null
						};
						playableDirector.SetGenericBinding( binding.sourceObject, transform);
					}
					else if( binding.outputTargetType == typeof( Animator))
					{
						Animator animator = collectObj switch
						{
							Animator source => source,
							GameObject gameObject => gameObject.GetComponent<Animator>(),
							Component component => component.GetComponent<Animator>(),
							_ => null,
						};
						playableDirector.SetGenericBinding( binding.sourceObject, animator);
					}
					else if( binding.outputTargetType.IsAssignableFrom( collectObj.GetType()) != false)
					{
						playableDirector.SetGenericBinding( binding.sourceObject, collectObj);
					}
					else if( collectObj is Component component)
					{
						playableDirector.SetGenericBinding( binding.sourceObject, component.GetComponent( binding.outputTargetType));
					}
					else if( collectObj is GameObject gameObject)
					{
						playableDirector.SetGenericBinding( binding.sourceObject, gameObject.GetComponent( binding.outputTargetType));
					}
				}
			}
		}
	}
}