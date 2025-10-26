
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

namespace Knit.Timeline.Editor
{
	[CustomPropertyDrawer( typeof( PlayableBehaviour), true)]
	sealed class PlayableBehaviourDrawer : PropertyDrawer 
	{
		public override float GetPropertyHeight( SerializedProperty property, GUIContent label)
		{
			property = property.serializedObject.FindProperty( property.propertyPath);
			float height = 0;
			int depth = 0;
			
			if( property.hasChildren != false)
			{
				if( property.NextVisible( true) != false)
				{
					depth = property.depth;
					height += EditorGUI.GetPropertyHeight( property, true);
					height += EditorGUIUtility.standardVerticalSpacing;
					
					while( property.NextVisible( false) != false)
					{
						if( property.depth == depth)
						{
							height += EditorGUI.GetPropertyHeight( property, true);
							height += EditorGUIUtility.standardVerticalSpacing;
						}
					}
				}
			}
			return height;
		}
		public override void OnGUI( Rect position, SerializedProperty property, GUIContent label)
		{
			property = property.serializedObject.FindProperty( property.propertyPath);
			position.height = 0;
			int depth = 0;
			
			using( new EditorGUI.PropertyScope( position, label, property)) 
			{
				if( property.hasChildren != false)
				{
					using( new EditorGUI.IndentLevelScope( 0))
					{
						if( property.NextVisible( true) != false)
						{
							depth = property.depth;
							position.height = EditorGUI.GetPropertyHeight( property, true);
							EditorGUI.PropertyField( position, property, true);
							position.y += position.height;
							position.y += EditorGUIUtility.standardVerticalSpacing;
							
							while( property.NextVisible( false) != false)
							{
								if( property.depth == depth)
								{
									position.height = EditorGUI.GetPropertyHeight( property, true);
									EditorGUI.PropertyField( position, property, true);
									position.y += position.height;
									position.y += EditorGUIUtility.standardVerticalSpacing;
								}
							}
						}
					}
				}
			}
		}
	}
}
