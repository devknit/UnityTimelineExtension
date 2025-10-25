#if UNITY_EDITOR

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace Knit.TimelineExtension
{
	public static partial class Extension
	{
		public static void GatherTransform( this IPropertyCollector driver, Transform transform)
		{
			var serializedObject = new SerializedObject( transform);
			SerializedProperty property = serializedObject.GetIterator();
			
			while( property.NextVisible( true) != false)
			{
				if( property.hasVisibleChildren == false)
				{
					driver.AddFromName<Transform>( transform.gameObject, property.propertyPath);
				}
			}
		}
		public static void GatherTransforms( this IPropertyCollector driver, Transform transform)
		{
			driver.GatherTransform( transform);
			
			for( int i0 = transform.childCount - 1; i0 >= 0; --i0)
			{
				driver.GatherTransforms( transform.GetChild( i0));
			}
		}
		public static void SetLastTrackBefore( this TimelineAsset track, TrackAsset asset)
		{
			try
			{
				if( track == null)
				{
					throw new ArgumentNullException("track");
				}
				Type t = track.GetType();
				MethodInfo method = t.GetMethod( "MoveLastTrackBefore", 
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				method.Invoke( track, new[]{ asset });
			}
			catch( Exception e)
			{
				Debug.LogError( e);
			}
		}
		/// <summary>
		/// Private field name of <see cref="TimelineClip.postExtrapolationMode"/>
		/// </summary>
		static readonly string POST_EXTRAP_MODE_INTERNAL_FIELD = "m_PostExtrapolationMode";
		/// <summary>
		/// Private field name of <see cref="TimelineClip.preExtrapolationMode"/>
		/// </summary>
		static readonly string PRE_EXTRAP_MODE_INTERNAL_FIELD = "m_PreExtrapolationMode";
		/// <summary>
		/// Private field name of the "Animation Extrapolation" duration beyond the end of the clip
		/// </summary>
		static readonly string POST_EXTRAP_TIME_INTERNAL_FIELD = "m_PostExtrapolationTime";
		/// <summary>
		/// Private field name of the "Animation Extrapolation" duration before the start of the clip
		/// </summary>
		static readonly string PRE_EXTRAP_TIME_INTERNAL_FIELD = "m_PreExtrapolationTime";  
		/// <summary>
		/// Sets the pre-extrapolation mode for a clip
		/// (uses Unity 2018.2.0f2 internal methods that may break in future versions)
		/// </summary>
		/// <param name="clip">Clip to set the extrapolation for</param>
		/// <param name="extrapolationMode">Extrapolation behaviour</param>
		public static void SetPreExtrapolationMode( this TimelineClip clip, TimelineClip.ClipExtrapolation extrapolationMode)
		{
			clip.SetPrivateFieldValue( PRE_EXTRAP_MODE_INTERNAL_FIELD, extrapolationMode);
		}
		/// <summary>
		/// Sets the pre-extrapolation time for a clip (duration of the extrapolated portion)
		/// (uses Unity 2018.2.0f2 internal methods that may break in future versions)
		/// </summary>
		/// <param name="clip">Clip to set the extrapolation for</param>
		/// <param name="time">How long to extrapolate into the past before the clip starts</param>
		public static void SetPreExtrapolationTime( this TimelineClip clip, double time)
		{
			clip.SetPrivateFieldValue( PRE_EXTRAP_TIME_INTERNAL_FIELD, time);
		}
		/// <summary>
		/// Gets the pre-extrapolation time for a clip (duration of the extrapolated portion)
		/// (uses Unity 2018.2.0f2 internal methods that may break in future versions)
		/// </summary>
		/// <param name="clip">Clip to get the extrapolation for</param>
		/// <param name="time">Duration of the extrapolation into the past before the clip starts</param>
		public static double GetPreExtrapolationTime( this TimelineClip clip)
		{
			return clip.GetPrivateFieldValue<double>( PRE_EXTRAP_TIME_INTERNAL_FIELD);
		}
		/// <summary>
		/// Sets the post-extrapolation mode for a clip
		/// (uses Unity 2018.2.0f2 internal methods that may break in future versions)
		/// </summary>
		/// <param name="clip">Clip to set the extrapolation for</param>
		/// <param name="extrapolationMode">Extrapolation behaviour</param>
		public static void SetPostExtrapolationMode( this TimelineClip clip, TimelineClip.ClipExtrapolation extrapolationMode)
		{
			clip.SetPrivateFieldValue( POST_EXTRAP_MODE_INTERNAL_FIELD, extrapolationMode);
		}
		/// <summary>
		/// Sets the post-extrapolation time for a clip (duration of the extrapolated portion)
		/// (uses Unity 2018.2.0f2 internal methods that may break in future versions)
		/// </summary>
		/// <param name="clip">Clip to set the extrapolation for</param>
		/// <param name="time">How long to extrapolate into the future beyond the clip end</param>
		public static void SetPostExtrapolationTime( this TimelineClip clip, double time)
		{
			clip.SetPrivateFieldValue( POST_EXTRAP_TIME_INTERNAL_FIELD, time);
		}
		/// <summary>
		/// Gets the post-extrapolation time for a clip (duration of the extrapolated portion)
		/// (uses Unity 2018.2.0f2 internal methods that may break in future versions)
		/// </summary>
		/// <param name="clip">Clip to get the extrapolation for</param>
		/// <param name="time">Duration of the extrapolation into the future beyond the clip end</param>
		public static double GetPostExtrapolationTime( this TimelineClip clip)
		{
			return clip.GetPrivateFieldValue<double>( POST_EXTRAP_TIME_INTERNAL_FIELD);
		}
		/// <summary>
		/// Returns a Property Value from a given Object. Uses Reflection.
		/// Throws a ArgumentOutOfRangeException if the Property is not found.
		/// </summary>
		/// <typeparam name="T">Type of the Property</typeparam>
		/// <param name="obj">Object from where the Property Value is returned</param>
		/// <param name="propName">Propertyname as string.</param>
		/// <returns>PropertyValue</returns>
		public static T GetPrivateFieldValue<T>( this object obj, string propName)
		{
			if( obj == null)
			{
				throw new ArgumentNullException( "obj");
			}
			Type t = obj.GetType();
			FieldInfo fi = null;
			
			while( fi == null && t != null)
			{
				fi = t.GetField( propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				t = t.BaseType;
			}
			if( fi == null)
			{
				throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName));
			}
			return (T)fi.GetValue( obj);
		}
		/// <summary>
		/// Set a Property Value on a given Object. Uses Reflection.
		/// </summary>
		/// <typeparam name="T">Type of the Property</typeparam>
		/// <param name="obj">Object from where the Property Value is returned</param>
		/// <param name="propName">Propertyname as string.</param>
		/// <param name="val">the value to set</param>
		/// <exception cref="ArgumentOutOfRangeException">if the Property is not found</exception>
		public static void SetPrivateFieldValue<T>( this object obj, string propName, T val)
		{
			if( obj == null)
			{
				throw new ArgumentNullException( "obj");
			}
			Type t = obj.GetType();
			FieldInfo fi = null;
			
			while( fi == null && t != null)
			{
				fi = t.GetField( propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				t = t.BaseType;
			}
			if( fi == null)
			{
				throw new ArgumentOutOfRangeException( "propName", 
					string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName));
			}
			fi.SetValue( obj, val);
		}
		public static int DestroyIsolatedMarkers( this TimelineAsset timelineAsset)
		{
			int destroyCount = 0;
			
			if( timelineAsset != null)
			{
				string assetPath = AssetDatabase.GetAssetPath( timelineAsset);
				
				if( string.IsNullOrEmpty( assetPath) == false)
				{
					var assets = AssetDatabase.LoadAllAssetsAtPath( assetPath);
					if( assets != null)
					{
						foreach( var asset in assets)
						{
							if( asset is Marker marker)
							{
								if( marker.parent == null)
								{
									UnityEngine.Object.DestroyImmediate( marker, true);
									++destroyCount;
								}
							}
						}
					}
				}
			}
			return destroyCount;
		}
	}
}
#endif
