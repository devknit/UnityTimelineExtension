
using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.TimelineExtension
{
	[Conditional( "UNITY_EDITOR")]
	public sealed class ClipDropdownAttribute : PropertyAttribute 
	{
		public ClipDropdownAttribute()
		{
		}
		public ClipDropdownAttribute( Type clipAssetType)
		{
			if( clipAssetType.IsSubclassOf( typeof( PlayableAsset)) != false)
			{
				m_ClipAssetType = clipAssetType;
			}
		}
		public ClipDropdownAttribute( Type tracAssetkType, Type clipAssetType)
		{
			if( tracAssetkType.IsSubclassOf( typeof( TrackAsset)) != false)
			{
				m_TrackAssetType = tracAssetkType;
			}
			if( clipAssetType.IsSubclassOf( typeof( PlayableAsset)) != false)
			{
				m_ClipAssetType = clipAssetType;
			}
		}
		public Type TrackAssetType
		{
			get{ return m_TrackAssetType ?? typeof( TrackAsset); }
		}
		public Type ClipAssetType
		{
			get{ return m_ClipAssetType ?? typeof( PlayableAsset); }
		}
		readonly Type m_TrackAssetType;
		readonly Type m_ClipAssetType;
	}
}
