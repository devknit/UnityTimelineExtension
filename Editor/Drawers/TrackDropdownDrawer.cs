
using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.Timeline.Editor
{
	[CustomPropertyDrawer( typeof( TrackDropdownAttribute))]
	sealed class TrackAssetDrawer : PropertyDrawer 
	{
		public override void OnGUI( Rect position, SerializedProperty property, GUIContent label)
		{
			PlayableDirector director = property.serializedObject.targetObject switch
			{
				GameObject gameObjecct => gameObjecct.GetComponent<PlayableDirector>(),
				Component component => component.GetComponent<PlayableDirector>(),
				_ => null
			};
			position.height = EditorGUIUtility.singleLineHeight;
			
			if( director?.playableAsset is TimelineAsset timelineAsset)
			{
				int index = 0;
				Type trackAssetType = (attribute as TrackDropdownAttribute).TrackAssetType;
				var tracks = timelineAsset.GetTracks( trackAssetType).Prepend( null).ToArray();
				var trackNames = tracks.Select( x => new GUIContent( GetTrackName( index++, x, trackAssetType))).ToArray();
				
				if( tracks.Length > 0)
				{
					var trackAsset = property.objectReferenceValue as TrackAsset;
					
					for( index = 0; index < tracks.Length; ++index)
					{
						if( tracks[ index] == trackAsset)
						{
							break;
						}
					} 
					if( index == tracks.Length)
					{
						index = 0;
					}
					index = EditorGUI.Popup( position, label, index, trackNames);
					property.objectReferenceValue = tracks[ index];
					return;
				}
			}
			EditorGUI.PropertyField( position, property, true);
		}
		static string GetTrackName( int index, TrackAsset trackAsset, Type trackType)
		{
			if( trackAsset == null)
			{
				return "None";
			}
			if( trackType == typeof( TrackAsset))
			{
				return $"#{index}: {trackAsset.name} <{trackAsset.GetType().Name}>";
			}
			return $"#{index}: {trackAsset.name}";
		}
	}
}
