
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Knit.Timeline.Editor
{
	[CustomPropertyDrawer( typeof( LightBehaviour))]
	sealed class LightEmissionBehaviourDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight( SerializedProperty property, GUIContent label)
		{
			return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 7;
		}
		public override void OnGUI( Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty color = property.FindPropertyRelative( "m_Color");
			SerializedProperty intensity = property.FindPropertyRelative( "m_Intensity");
			SerializedProperty range = property.FindPropertyRelative( "m_Range");
			SerializedProperty spotAngle = property.FindPropertyRelative( "m_SpotAngle");
			SerializedProperty innerSpotAngle = property.FindPropertyRelative( "m_InnerSpotAngle");
			SerializedProperty bounceIntensity = property.FindPropertyRelative( "m_BounceIntensity");
			SerializedProperty colorTemperature = property.FindPropertyRelative( "m_ColorTemperature");
			
			position.height = EditorGUIUtility.singleLineHeight;
			
			EditorGUI.PropertyField( position, color, new GUIContent( "Color or Filter"));
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			
			if( m_Internal == null)
			{
				m_Internal = new LightEditorInernal();
			}
			if( m_Internal.SliderWithTexture( position, EditorGUIUtility.TrTextContent( "Temperature"), colorTemperature) != false)
			{
				position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			}
			EditorGUI.PropertyField( position, intensity);
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			
			EditorGUI.PropertyField( position, bounceIntensity, new GUIContent( "Indirect Multiplier"));
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			
			EditorGUI.PropertyField( position, range);
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			
			float innerSpotAngleValue = innerSpotAngle.floatValue;
			float spotAngleValue = spotAngle.floatValue;
			EditorGUI.BeginChangeCheck();
			EditorGUI.MinMaxSlider( position, "Inner / Outer Spot Angle", ref innerSpotAngleValue, ref spotAngleValue, 0, 179);
			
			if( EditorGUI.EndChangeCheck() != false)
			{
				innerSpotAngle.floatValue = innerSpotAngleValue;
				spotAngle.floatValue = spotAngleValue;
			}
		}
		sealed class LightEditorInernal
		{
			public LightEditorInernal()
			{
				var lightEditorSettingsType = typeof( LightEditor.Settings);
				m_SliderPower = (float)lightEditorSettingsType.GetField( "kSliderPower", BindingFlags.Static | BindingFlags.NonPublic).GetRawConstantValue();
				m_MinKelvin = (float)lightEditorSettingsType.GetField( "kMinKelvin", BindingFlags.Static | BindingFlags.NonPublic).GetRawConstantValue();
				m_MaxKelvin = (float)lightEditorSettingsType.GetField( "kMaxKelvin", BindingFlags.Static | BindingFlags.NonPublic).GetRawConstantValue();
				
				m_SliderWithTextureMethod = typeof(EditorGUI).GetMethod( 
					"SliderWithTexture", 
					BindingFlags.Static | BindingFlags.NonPublic,
					null,
					CallingConventions.Any,
					new[]
					{
						typeof(Rect),
						typeof(GUIContent),
						typeof(SerializedProperty),
						typeof(float),
						typeof(float),
						typeof(float),
						typeof(Texture2D) 
					},
					null);
				var CreateKelvinGradientTexture = typeof( LightEditor.Settings).GetMethod( 
					"CreateKelvinGradientTexture", 
					BindingFlags.Static | BindingFlags.NonPublic,
					null,
					CallingConventions.Any,
					new[]
					{
						typeof(string),
						typeof(int),
						typeof(int),
						typeof(float),
						typeof(float) 
					},
					null);
				m_KelvinGradientTexture = CreateKelvinGradientTexture?.Invoke( 
					null, new object[]{ "KelvinGradientTexture", 300, 16, m_MinKelvin, m_MaxKelvin }) as Texture2D;
			}
			public bool SliderWithTexture( Rect position, GUIContent label, SerializedProperty property)
			{
				if( m_SliderWithTextureMethod != null)
				{
					m_SliderWithTextureMethod.Invoke( null, new object[]
					{
						position,
						label,
						property, 
						m_MinKelvin,
						m_MaxKelvin,
						m_SliderPower,
						m_KelvinGradientTexture
					});
					return true;
				}
				return false;
			}
			float m_SliderPower;
			float m_MinKelvin;
			float m_MaxKelvin;
            readonly MethodInfo m_SliderWithTextureMethod;
            readonly Texture2D m_KelvinGradientTexture;
		}
		static LightEditorInernal m_Internal;
	}
}
