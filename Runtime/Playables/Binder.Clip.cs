
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace Knit.Timeline
{
	public static partial class Binder
	{
		public static void BindClip( this TimelineClip clip, Dictionary<string, HashSet<BindableObject>> objects, UnityEngine.Object trackBindableObject)
		{
			if( clip != null && (objects != null || trackBindableObject != null))
			{
				if( clip.asset is IBindableClip || clip.asset is AnimationPlayableAsset)
				{
					string[] targetCommands = clip.displayName.Split( kBindableSymbol);
					
					for( int i0 = 1; i0 < targetCommands.Length; ++i0)
					{
						Bind( clip, targetCommands[ i0], objects, trackBindableObject);
					}
				}
			}
		}
		public static void UnBindClip( this TimelineClip clip)
		{
			string[] targetCommands;
			
			if( clip.asset is IBindableClip bindableClip)
			{
				targetCommands = clip.displayName.Split( kBindableSymbol);
				
				if( targetCommands.Length > 1)
				{
					bindableClip.SetBindObject( null, null);
				}
			}
			else if( clip.asset is AnimationPlayableAsset animationPlayable)
			{
				targetCommands = clip.displayName.Split( kBindableSymbol);
				
				if( targetCommands.Length > 1)
				{
					animationPlayable.clip = null;
				}
			}
		}   
		static void Bind( TimelineClip clip, string targetCommand, Dictionary<string, HashSet<BindableObject>> objects, UnityEngine.Object trackBindableObject)
		{
			string attributionObjName = null;
			
			if( trackBindableObject != null)
			{
				string trackObjName = trackBindableObject.name;
				
				if( trackObjName.Length > 0 && trackObjName[ 0] == kBindableSymbol)
				{
					attributionObjName = trackObjName[ 1..];
				}
				else
				{
					attributionObjName = trackObjName;
				}
			}
			UnityEngine.Object collectObj = FindBindingObject( targetCommand, objects, 
				attributionObjName, true, out string assignObjName, out string assignDestName);
			
			if( collectObj != null)
			{
				UnityEngine.Object bindableObject = null;
				BindableType bindableType;
				Type assignType = null;
				
				if( clip.asset is IBindableClip bindingClip)
				{
					assignType = bindingClip.GetAssignFieldType( assignDestName);
					
					if( assignType == typeof(GameObject))
					{
						bindableType = BindableType.GameObject;
					}
					else if( assignType == typeof(Transform))
					{
						bindableType = BindableType.Transform;
					}
					else
					{
						bindableType = BindableType.Other;
					}
				}
				else
				{
					bindableType = BindableType.Other;
				}
				if( bindableType == BindableType.GameObject)
				{
					bindableObject = collectObj switch
					{
						GameObject gameObject => gameObject,
						Transform transform => transform.gameObject,
						Component component => component.gameObject,
						_ => null,
					};
				}
				else if( bindableType == BindableType.Transform)
				{
					bindableObject = collectObj switch
					{
						GameObject gameObject => gameObject.transform,
						Transform transform => transform,
						Component component => component.transform,
						_ => null,
					};
				}
				else
				{
					IPlayableAssetInfo assetInfoComponent = collectObj switch
					{
						GameObject gameObject => gameObject.GetComponent<IPlayableAssetInfo>(),
						Transform transform => transform.GetComponent<IPlayableAssetInfo>(), // Component型でtransformってとれるのでは？
						Component component => component.GetComponent<IPlayableAssetInfo>(),
						_ => null,
					};
					assetInfoComponent?.TryGetValue( assignObjName, out bindableObject);
				}
				if( bindableObject != null) // 外側でやることになる
				{
					if( clip.asset is AnimationPlayableAsset animationPlayable)
					{
						animationPlayable.clip = bindableObject as AnimationClip;
					}
					else if( clip.asset is IBindableClip bindableClip)
					{
						if( bindableObject.GetType() == assignType)
						{
							bindableClip.SetBindObject( bindableObject, assignDestName);
						}
					}
				}
			}
		}
	} 
}