
using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.Timeline.Editor
{
	[CustomPropertyDrawer( typeof( ClipDropdownAttribute))]
	sealed class ClipAssetDrawer : PropertyDrawer 
	{
		public override void OnGUI( Rect position, SerializedProperty property, GUIContent label)
		{
			var director = property.serializedObject.context as PlayableDirector;
			
			if( director == null)
			{
				director = property.serializedObject.targetObject switch
				{
					GameObject gameObjecct => gameObjecct.GetComponent<PlayableDirector>(),
					Component component => component.GetComponent<PlayableDirector>(),
					_ => null
				};
			}
			position.height = EditorGUIUtility.singleLineHeight;
			
			if( director?.playableAsset is TimelineAsset timelineAsset)
			{
				int index = 0;
				Type trackAssetType = null;
				Type clipAssetType = null;
				
				if( attribute is ClipDropdownAttribute clip)
				{
					trackAssetType = clip.TrackAssetType;
					clipAssetType = clip.ClipAssetType;
				}
				var clips = timelineAsset.GetClips( trackAssetType, clipAssetType).Prepend( null).ToArray();
				var clipNames = clips.Select( x => new GUIContent( GetClipName( index++, x, clipAssetType))).ToArray();
				
				if( clips.Length > 0)
				{
					var clipAsset = property.objectReferenceValue as PlayableAsset;
					
					for( index = 0; index < clips.Length; ++index)
					{
						if( clips[ index]?.asset == clipAsset)
						{
							break;
						}
					} 
					if( index == clips.Length)
					{
						index = 0;
					}
					index = EditorGUI.Popup( position, label, index, clipNames);
					property.objectReferenceValue = clips[ index]?.asset;
					return;
				}
			}
			EditorGUI.PropertyField( position, property, true);
		}
		static string GetClipName( int index, TimelineClip clip, Type clipAssetType)
		{
			var clipAsset = clip?.asset;
			
			if( clipAsset == null)
			{
				return "None";
			}
			if( clipAssetType == typeof( PlayableAsset))
			{
				return $"#{index}: {clip.displayName} <{clipAsset.GetType().Name}>";
			}
			return $"#{index}: {clip.displayName}";
		}
	}
}
