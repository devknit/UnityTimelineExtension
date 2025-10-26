
using UnityEngine;

namespace Knit.Timeline.Test
{
	sealed class RepeatTrackReceiver : RepeatProxy
	{
		protected override void OnJoin( IRepeatTrack track)
		{
			m_RepeatTrack = track;
		}
		protected override void OnLeave()
		{
			m_RepeatTrack = null;
		}
		void Start()
		{
			Debug.LogWarning( "RepeatTrackReceiver はサンプルとしての使用にのみ使用できます");
		}
		void OnGUI()
		{
			if( m_RepeatTrack != null)
			{
				foreach( var target in m_RepeatTrack.GetOtherTargets())
				{
					if( GUILayout.Button( target.Key) != false)
					{
						m_RepeatTrack.Abort( target.Key);
					}
				}
				if( GUILayout.Button( "Abort") != false)
				{
					m_RepeatTrack.Abort( null);
				}
			}
		}
		IRepeatTrack m_RepeatTrack;
	}
}
