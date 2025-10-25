
using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Timeline;

namespace Knit.TimelineExtension
{
	[Conditional( "UNITY_EDITOR")]
	public sealed class TrackDropdownAttribute : PropertyAttribute 
	{
		public TrackDropdownAttribute()
		{
		}
		public TrackDropdownAttribute( Type trackAssetType)
		{
			if( trackAssetType.IsSubclassOf( typeof( TrackAsset)) != false)
			{
				m_TrackAssetType = trackAssetType;
			}
		}
		public Type TrackAssetType
		{
			get{ return m_TrackAssetType ?? typeof( TrackAsset); }
		}
		readonly Type m_TrackAssetType;
	}
}