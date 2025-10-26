
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	[Serializable]
	sealed class RepeatBehaviour : PlayableBehaviour, ISection
	{
		internal void Initalize( RepeatClip playableAsset)
		{
			m_StartTime = playableAsset.Clip.start;
			m_EndTime = playableAsset.Clip.end;
		}
		internal ISection GetTarget( string key)
		{
			if( m_DefaultTarget != null && m_DefaultTarget.Key == key)
			{
				return m_DefaultTarget;
			}
			return m_OtherTargets.FirstOrDefault( x => x.Key == key);
		}
		internal ISeekTarget GetDefaultTarget()
		{
			return m_DefaultTarget;
		}
		internal IEnumerable<ISeekTarget> GetOtherTargets()
		{
			foreach( var section in m_OtherTargets)
			{
				if( section is ISeekTarget target)
				{
					if( string.IsNullOrEmpty( target.Key) == false)
					{
						yield return target;
					}
				}
			}
		}
		internal int PlayCount
		{
			get{ return (m_PlayCount > 0) ? m_PlayCount : -1; }
		}
		internal bool JoinEventCallAtEnd
		{
			get{ return m_JoinEventCallAtEnd; }
		}
		public double StartTime
		{
			get{ return m_StartTime; }
		}
		public double EndTime
		{
			get{ return m_EndTime; }
		}
		internal SeekMode DefaultSeekMode
		{
			get{ return m_DefaultSeekMode; }
		}
		internal ISection DefaultTarget
		{
			get{ return m_DefaultTarget; }
		}
		internal ISection EntrustTarget
		{
			get{ return m_EntrustTarget; }
		}
		[SerializeField, Min( 0)]
		int m_PlayCount = 0;
		[SerializeField]
		bool m_JoinEventCallAtEnd = false;
		[SerializeField]
		SeekMode m_DefaultSeekMode = SeekMode.End;
		[SerializeField, ClipDropdown( typeof( SectionClip))]
		SectionClip m_DefaultTarget;
		[SerializeField, ClipDropdown( typeof( SectionClip))]
		SectionClip m_EntrustTarget;
		[SerializeField, ClipDropdown( typeof( SectionClip))]
		SectionClip[] m_OtherTargets;
		[NonSerialized]
		double m_StartTime = double.MaxValue;
		[NonSerialized]
		double m_EndTime = -1;
	}
	public enum SeekMode
	{
		None,
		Start,
		End,
	}
}
