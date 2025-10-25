
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knit.TimelineExtension
{
	public static partial class Binder
	{
		public class FindParam
		{
			public FindParam( bool defaultSettings, IncludeDelegate includeFunc)
			{
				if( defaultSettings != false)
				{
					AddNameRules( kBindableSymbol.ToString(), typeof(GameObject));
				}
				m_IncludeFunc = includeFunc;
			}
			public void AddNameRules( string name, Type storeType)
			{
				if( string.IsNullOrEmpty( name) == false)
				{
					m_NameTypes ??= new Dictionary<string, HashSet<Type>>();
					
					if( m_NameTypes.TryGetValue( name, out HashSet<Type> typeHash) == false)
					{
						typeHash = new HashSet<Type>();
						m_NameTypes.Add( name, typeHash);
					}
					typeHash.Add( storeType); 
				}
			}
			public void AddTypeRules( Type checkType, Type storeType)
			{
				if( checkType != null)
				{
					m_CheckTypes ??= new Dictionary<Type, HashSet<Type>>();
					
					if( m_CheckTypes.TryGetValue( checkType, out HashSet<Type> typeHash) == false)
					{
						typeHash = new HashSet<Type>();
						m_CheckTypes.Add( checkType, typeHash);
					}
					typeHash.Add( storeType);
				}
			}
			internal BindableObject Include( 
				Dictionary<string, HashSet<BindableObject>> objects, 
				Transform obj, Behaviour[] components, BindableObject parent)
			{
				BindableObject addBindableObject = null;
				string objName = obj.name;
				
				if( (objName?.Length ?? 0) > 1)
				{
					string addName = objName;
					
					if( addName[ 0] == kBindableSymbol)
					{
						addName = addName[1..];
					}
					void AddObjectCallback( UnityEngine.Object storeObj)
					{
						if( objects.TryGetValue( addName, out HashSet<BindableObject> objHash) == false)
						{
							objHash = new HashSet<BindableObject>();
							objects.Add( addName, objHash);
						}
						addBindableObject = new BindableObject( storeObj, parent);
						objHash.Add( addBindableObject);
					}
					bool includeSkip = m_IncludeFunc?.Invoke( obj, components, AddObjectCallback) ?? false;
					
					if( includeSkip == false)
					{
						if( m_NameTypes != null)
						{
							foreach( var name in m_NameTypes.Keys)
							{
								if( objName.StartsWith( name) != false)
								{
									if( AddObject( objects, m_NameTypes[ name], obj, addName, 
										components, parent, out BindableObject addObj) != false)
									{
										addBindableObject = addObj;
									}
									break;
								}
							}
						}
						if( m_CheckTypes != null)
						{
							foreach( var checkType in m_CheckTypes.Keys)
							{
								for( int i1 = components.Length - 1; i1 >= 0; --i1)
								{
									if( checkType.IsAssignableFrom( components[ i1].GetType()) != false)
									{
										if( AddObject( objects, m_CheckTypes[ checkType], obj, 
											addName, components, parent, out BindableObject addObj) != false)
										{
											addBindableObject = addObj;
										}
									}
								}
							}
						}
					}
				}
				return addBindableObject;
			}
			bool AddObject( Dictionary<string, HashSet<BindableObject>> objects, 
				HashSet<Type> addTypes, Transform transform, string addName, 
				Behaviour[] components, BindableObject parent, out BindableObject addBindableObject)
			{
				addBindableObject = null;
				
				if( objects.TryGetValue( addName, out HashSet<BindableObject> objHash) == false)
				{
					objHash = new HashSet<BindableObject>();
					objects.Add( addName, objHash);
				}
				foreach( Type addType in addTypes)
				{
					if( addType == typeof(GameObject))
					{
						addBindableObject = new BindableObject( transform.gameObject, parent);
						objHash.Add( addBindableObject);
					}
					else if( addType == typeof(Transform))
					{
						addBindableObject = new BindableObject( transform, parent);
						objHash.Add( addBindableObject);
					}
					else
					{
						for( int i0 = components.Length - 1; i0 >= 0; --i0)
						{
							if( addType.IsAssignableFrom( components[ i0].GetType()) != false)
							{
								addBindableObject = new BindableObject( components[ i0], parent);
								objHash.Add( addBindableObject);
								break;
							}
						}
					}
				}
				return addBindableObject != null;
			}
			public static explicit operator FindParam( IncludeDelegate callback)
			{
				return new FindParam( false, callback);
			}
			public delegate bool IncludeDelegate( Transform trs, Behaviour[] components, AddObjectDelegate addObjectFunc); 
			public delegate void AddObjectDelegate( UnityEngine.Object addObject);
			
			Dictionary<string, HashSet<Type>> m_NameTypes;
			Dictionary<Type, HashSet<Type>> m_CheckTypes;
			readonly IncludeDelegate m_IncludeFunc;
		}
	}
}