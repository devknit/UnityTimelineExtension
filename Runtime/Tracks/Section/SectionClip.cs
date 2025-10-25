
using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.TimelineExtension
{
	[Conditional( "UNITY_EDITOR")]
	public sealed class SectionParamAttribute : PropertyAttribute 
	{
		public SectionParamAttribute( int index)
		{
			m_Index = index;
		}
		public int Index
		{
			get{ return m_Index; }
		}
        readonly int m_Index;
	}
	[Serializable]
	sealed class SectionClip : PlayableAsset, ITimelineClipAsset, ISeekTarget
	{
		public ClipCaps clipCaps
		{
			get { return ClipCaps.None; }
		}
		public override Playable CreatePlayable( PlayableGraph graph, GameObject owner)
		{
			return Playable.Create( graph);
		}
		internal void Initalize( TimelineClip clip)
		{
			name = clip.displayName;
			m_Start = clip.start;
			m_End = clip.end;
		}
		public string Key
		{
			get{ return name; }
		}
		public double StartTime
		{
			get{ return m_Start; }
		}
		public double EndTime
		{
			get{ return m_End; }
		}
		public string Param0
		{
			get{ return m_Param0; }
		}
		public string Param1
		{
			get{ return m_Param1; }
		}
		public string Param2
		{
			get{ return m_Param2; }
		}
		public string Param3
		{
			get{ return m_Param3; }
		}
		[SerializeField, SectionParam( 0)]
		string m_Param0;
		[SerializeField, SectionParam( 1)]
		string m_Param1;
		[SerializeField, SectionParam( 2)]
		string m_Param2;
		[SerializeField, SectionParam( 3)]
		string m_Param3;
		[NonSerialized]
		double m_Start;
		[NonSerialized]
		double m_End;
	}
}
