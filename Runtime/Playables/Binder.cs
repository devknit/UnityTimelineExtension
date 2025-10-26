
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace Knit.Timeline
{
	public static partial class Binder
	{
		public static void FindObjects( Dictionary<string, HashSet<BindableObject>> objects, FindParam param, bool includeInactive)
		{
			for( int i0 = SceneManager.sceneCount - 1; i0 >= 0; --i0)
			{
				FindObjects( SceneManager.GetSceneAt( i0), objects, param, includeInactive);
			}
		}
		public static void FindObjects( Scene scene, Dictionary<string, HashSet<BindableObject>> objects, FindParam param, bool includeInactive)
		{
			if( scene.IsValid() == false)
			{
				throw new ArgumentException( "scene is invalid", nameof(scene));
			}
			else if( objects == null)
			{
				throw new ArgumentNullException( "objects cannot be null");
			}
			else
			{
				param ??= new FindParam( true, null);
				
				var gameObjects = scene.GetRootGameObjects();
				
				for( int i0 = gameObjects.Length - 1; i0 >= 0; --i0)
				{
					if( includeInactive != false || gameObjects[ i0].activeSelf != false)
					{
						FindObjects( gameObjects[ i0].transform, objects, param, null);
					}
				}
			}
		}
		public static void Bind( this PlayableDirector playableDirector, Dictionary<string, HashSet<BindableObject>> objects)
		{
			if( playableDirector != null && objects != null)
			{
				if( playableDirector?.playableAsset is TimelineAsset timeline)
				{
					foreach( var track in timeline.GetTracks( null))
					{
						track.BindTrack( playableDirector, objects);
						
						foreach( var clip in track.GetClips())
						{
							clip.BindClip( objects, playableDirector.GetGenericBinding( track));
						}
					}
				}
			}
		}
		public static void UnBind( this PlayableDirector playableDirector)
		{
			if( playableDirector?.playableAsset is TimelineAsset timeline)
			{
				foreach( var track in timeline.GetTracks( null))
				{
					string[] targetCommand = track.name.Split( kBindableSymbol);
					
					if( targetCommand.Length > 1)
					{
						track.UnBindTrack( playableDirector);
					}
					foreach( var clip in track.GetClips())
					{
						clip.UnBindClip();
					}
				}
			}
		}
		internal static void FindObjects( Transform transform, Dictionary<string, HashSet<BindableObject>> objects, FindParam param, BindableObject parent)
		{            
			var components = transform.GetComponents<Behaviour>();
			bool finding = true;
			
			for( int i0 = components.Length -1; i0 >= 0; --i0)
			{
				if( components[ i0] is PlayableDirector)
				{
					finding = false;
					break;
				}
			}
			if( finding != false)
			{
				BindableObject addObj = param.Include( objects, transform, components, parent);
				
				if( addObj != null)
				{
					parent = addObj;
				}
				
				for( int i0 = transform.childCount - 1; i0 >= 0; --i0)
				{
					FindObjects( transform.GetChild( i0), objects, param, parent);
				}
			}
		}
		static UnityEngine.Object FindBindingObject( string targetCommand, Dictionary<string, HashSet<BindableObject>> objects, string attributionObjName, bool checkTraceBack, out string assignObjName, out string assignDestName)
		{
			assignObjName = null;
			
			UnityEngine.Object bindableObject = null;
			
			if( CommandSplit( targetCommand, attributionObjName, 
				out assignDestName, out string[] objectNames, 
				out string componentFullName) != false)
			{
				assignObjName = objectNames[ ^1];
				bindableObject = FindObjectToBind( objects, objectNames, checkTraceBack, componentFullName);
			}
			return bindableObject;
		}
		static bool CommandSplit( string targetCommand, string attributionObjName, out string assignDestName, out string[] objectNames, out string componentName)
		{
			assignDestName = null;
			objectNames = null;
			componentName = null;
			
			string[] command = targetCommand.Split( kCommandSymbol);
			string[] directoryCommand = null;
			
			for( int i0 = command.Length - 1; i0 >= 0; --i0)
			{
				if( command[ i0].Length > 0)
				{
					if( command[ i0][ ^1] == kComponentSymbol)
					{
						componentName = command[ i0][ ..^1];
					}
					else
					{
						directoryCommand = command[ i0].Split( kDirectoryCommandSymbol);
					}
				}
			}
			if( directoryCommand != null)
			{
				string directory;
				
				if( directoryCommand.Length > 1)
				{
					assignDestName = directoryCommand[ 0];
					directory = directoryCommand[ 1];
				}
				else
				{
					assignDestName = null;
					directory = directoryCommand[ 0];
				}
				objectNames = directory.Split( kDirectorySymbol);
				
				if( attributionObjName != null)
				{
					for( int i0 = objectNames.Length - 1; i0 >= 0; --i0)
					{
						if( objectNames[ i0].Length == 0)
						{
							objectNames[ i0] = attributionObjName;
						}
					}
				}
				return true;
			}
			return false;
		}
		static UnityEngine.Object FindObjectToBind( Dictionary<string, HashSet<BindableObject>> objects, string[] directory, bool checkTraceBack, string componentFullName, int count = 0)
		{
			UnityEngine.Object hitObject = null;
			
			if( directory[ ^(count + 1)].Length > 0)
			{
				foreach( var obj in objects)
				{
					if( IsMatchName( obj.Key, directory[ ^(count + 1)]) != false)
					{
						foreach( var param in obj.Value)
						{
							if( IsMatchBindCondition( param, directory, componentFullName, count) != false)
							{
								hitObject = param.Object;
								break;
							}
						}
					}
					if( hitObject != null)
					{
						break;
					}
				}
				if( checkTraceBack != false && hitObject == null && count < 1)
				{
					if( ++count < directory.Length)
					{
						hitObject = FindObjectToBind( objects, directory, checkTraceBack, componentFullName, count);
					}
				}
			}
			return hitObject;
		}
		static bool IsMatchBindCondition( BindableObject objParam, string[] directory, string componentFullName, int count, bool first = true)
		{
			bool result = false;
			int checkIndex = directory.Length - 1 - count;
			
			if( directory[ checkIndex].Length > 0)
			{
				// ディレクトリの指定より親子付けの数が少ない場合があるためnullチェックを行う //
				if( objParam != null)
				{
					UnityEngine.Object obj = objParam.Object;
					string objName = objParam.ObjectName;
					
					if( IsMatchName( objName, directory[ checkIndex]) != false && IsMatchComponent( obj, componentFullName) != false)
					{
						result = true;
						
						if( ++count < directory.Length)
						{
							result = IsMatchBindCondition( objParam.Parent, directory, null, count, false);
						}
					}
					else if( first == false && count < directory.Length && objParam.Parent != null)
					{
						result = IsMatchBindCondition( objParam.Parent, directory, componentFullName, count, false);
					}
				}
			}
			return result;
		}
		static bool IsMatchName( string objectName, string findName)
		{
			bool result = false;
			
			switch( findName[ ^1])
			{
				case kNameEqualsSymbol:
				{
					if( findName.IndexOf( objectName, 0, findName.Length - 1) == 0 &&
						objectName.Length == (findName.Length - 1))
					{
						result = true;
					}
					break;
				}
				case kNameStartsWithSymbol:
				{
					if( objectName.StartsWith( findName[ ..^1]) != false)
					{
						result = true;
					}
					break;
				}
				case kNameEndsWithSymbol:
				{
					if( objectName.EndsWith( findName[ ..^1]) != false)
					{
						result = true;
					}
					break;
				}
				case kNameContainsSymbol:
				{
					if( objectName.Contains( findName[ ..^1]) != false)
					{
						result = true;
					}
					break;
				}
				default:
				{
					if( objectName.Equals( findName) != false)
					{
						result = true;
					}
					break;
				}
			}
			return result;
		}
		static bool IsMatchComponent( UnityEngine.Object obj, string findComponent)
		{
			return findComponent?.Equals( obj.GetType().FullName) ?? true;
		}
		enum BindableType
		{
			Other,
			GameObject,
			Transform,
		}
		internal const char kBindableSymbol = '$';
		const char kDirectorySymbol = '/';
		const char kCommandSymbol = '^';
		const char kDirectoryCommandSymbol = ':';
		const char kNameEqualsSymbol = '=';
		const char kNameStartsWithSymbol = '<';
		const char kNameEndsWithSymbol = '>';
		const char kNameContainsSymbol = '@';
		const char kComponentSymbol = '+';
	}
}