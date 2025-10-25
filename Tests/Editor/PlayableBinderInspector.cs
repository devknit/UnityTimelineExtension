#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;

namespace Knit.TimelineExtension.Test.Editor
{
	[CustomEditor(typeof(PlayableBinder))]
	sealed partial class PlayableBinderEditor : UnityEditor.Editor
	{		
		public override void OnInspectorGUI()
		{
			if( target is PlayableBinder binder)
			{
				using(new EditorGUILayout.HorizontalScope())
				{
					if( GUILayout.Button( "Bind") != false)
					{
						binder.Bind();
						TimelineEditor.Refresh( RefreshReason.WindowNeedsRedraw | RefreshReason.SceneNeedsUpdate | RefreshReason.ContentsModified | RefreshReason.ContentsAddedOrRemoved);
						AssetDatabase.Refresh();
					}
					
					if( GUILayout.Button( "UnBind") != false)
					{
						binder.UnBind();
						TimelineEditor.Refresh( RefreshReason.WindowNeedsRedraw | RefreshReason.SceneNeedsUpdate | RefreshReason.ContentsModified | RefreshReason.ContentsAddedOrRemoved);
						AssetDatabase.Refresh();
					}
				}
			}
			base.OnInspectorGUI();
		}
	}
}
#endif