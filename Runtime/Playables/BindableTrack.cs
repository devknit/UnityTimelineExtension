
using UnityEngine;
using UnityEngine.Timeline;
using System.Collections.Generic;

namespace Knit.Timeline
{
	sealed class BindableTrack
	{
		internal static BindableTrack MakeCondition( TrackAsset trackAsset, string[] conditions)
		{
			BindableTrack bindableTrack = null;
			int equalsCount = 0;
			int startsWithCount = 0;
			int endsWithCount = 0;
			int containsWithCount = 0;
			int componentCount = 0;
			
			for( int i0 = 0; i0 < conditions.Length; ++i0)
			{
				string condition = conditions[ i0];
				int needLength = i0 == 0 ? 1 : 2;
				
				if( (condition?.Length ?? 0) >= needLength)
				{
					switch( condition[^1])
					{
						case kNameEqualsSymbol:
						{
							++equalsCount;
							break;
						}
						case kNameStartsWithSymbol:
						{
							++startsWithCount;
							break;
						}
						case kNameEndsWithSymbol:
						{
							++endsWithCount;
							break;
						}
						case kNameContainsSymbol:
						{
							++containsWithCount;
							break;
						}
						case kComponentSymbol:
						{
							++componentCount;
							break;
						}
						default:
						{
							if( i0 == 0)
							{
								++equalsCount;
							}
							break;
						}
					}
				}
			}
			if( equalsCount > 0
			||	startsWithCount > 0
			||	endsWithCount > 0
			||	containsWithCount > 0
			||	componentCount > 0
			)
			{
				bindableTrack = new BindableTrack( trackAsset, equalsCount + startsWithCount + endsWithCount + containsWithCount);
				
				if( equalsCount > 0)
				{
					bindableTrack.m_NameEquals = new HashSet<string>( equalsCount);
				}
				if( startsWithCount > 0)
				{
					bindableTrack.m_NameStartsWith = new string[ startsWithCount];
				}
				if( endsWithCount > 0)
				{
					bindableTrack.m_NameEndsWith = new string[ endsWithCount];
				}
				if( containsWithCount > 0)
				{
					bindableTrack.m_NameContainsWith = new string[ containsWithCount];
				}
				if( componentCount > 0)
				{
					bindableTrack.m_ComponentConditions = new HashSet<string>( componentCount);
				}
				startsWithCount = 0;
				endsWithCount = 0;
				containsWithCount = 0;
				
				for( int i0 = 0; i0 < conditions.Length; ++i0)
				{
					string condition = conditions[ i0];
					int needLength = i0 == 0 ? 1 : 2;
					
					if( (condition?.Length ?? 0) >= needLength)
					{                    
						switch( condition[^1])
						{
							case kNameEqualsSymbol:
							{
								bindableTrack.m_NameEquals.Add( condition.Substring( 0, conditions[ i0].Length - 1));
								break;
							}
							case kNameStartsWithSymbol:
							{
								bindableTrack.m_NameStartsWith[ startsWithCount++] = condition.Substring( 0, conditions[ i0].Length - 1);
								break;
							}
							case kNameEndsWithSymbol:
							{
								bindableTrack.m_NameEndsWith[ endsWithCount++] = condition.Substring( 0, conditions[ i0].Length - 1);
								break;
							}
							case kNameContainsSymbol:
							{
								bindableTrack.m_NameContainsWith[ containsWithCount++] = condition.Substring( 0, conditions[ i0].Length - 1);
								break;
							}
							case kComponentSymbol:
							{
								bindableTrack.m_ComponentConditions.Add( condition.Substring( 0, conditions[ i0].Length - 1));
								break;
							}
							default:
							{
								if( i0 == 0)
								{
									bindableTrack.m_NameEquals.Add( condition);
								}
								break;
							}
						}
					}
				}
			}
			return bindableTrack;
		}
		internal bool TryGetValue( KeyValuePair<string, HashSet<BindableObject>> objectPair, out Object hitObject, out TrackAsset track)
		{
			hitObject = null;
			track = null;
			
			foreach( var obj in objectPair.Value)
			{
				if( IsMatchName( objectPair.Key) != false && IsMatchComponent( obj.GetType().FullName) != false)
				{
					hitObject = obj.Object;
					track = m_Track;
					return true;
				}
			}
			return false;
		}
		bool IsMatchName( string objectName)
		{
			if( m_NameConditionCount == 0)
			{
				return true;
			}
			if( (m_NameEquals?.Contains( objectName) ?? false) != false)
			{
				return true;
			}
			for( int i0 = m_NameStartsWith?.Length - 1 ?? -1; i0 >= 0; --i0)
			{
				if( objectName.StartsWith( m_NameStartsWith[ i0]) != false)
				{
					return true;
				}
			}
			for( int i0 = m_NameEndsWith?.Length - 1 ?? -1; i0 >= 0; --i0)
			{
				if( objectName.EndsWith( m_NameEndsWith[ i0]) != false)
				{
					return true;
				}
			}
			for( int i0 = m_NameContainsWith?.Length - 1 ?? -1; i0 >= 0; --i0)
			{
				if( objectName.Contains( m_NameContainsWith[ i0]) != false)
				{
					return true;
				}
			}
			return false;
		}
		bool IsMatchComponent( string componentFullName)
		{
			return m_ComponentConditions?.Contains( componentFullName) ?? true;
		}
		BindableTrack( TrackAsset track, int conditionCount)
		{
			m_Track = track;
			m_NameConditionCount = conditionCount;
		}
		const char kNameEqualsSymbol = '=';
		const char kNameStartsWithSymbol = '<';
		const char kNameEndsWithSymbol = '>';
		const char kNameContainsSymbol = '@';
		const char kComponentSymbol = '+';
		
		readonly TrackAsset m_Track;
		HashSet<string> m_NameEquals;
		string[] m_NameStartsWith;
		string[] m_NameEndsWith;
		string[] m_NameContainsWith;
		HashSet<string> m_ComponentConditions;
		readonly int m_NameConditionCount;
	}
}