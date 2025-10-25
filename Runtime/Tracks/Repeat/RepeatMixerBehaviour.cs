
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.Collections.Generic;

namespace Knit.TimelineExtension
{
	public interface ISection
	{
		double StartTime{ get; }
		double EndTime{ get; }
	}
	public interface ISeekTarget : ISection
	{
		string Key{ get; }
		string Param0{ get; }
		string Param1{ get; }
		string Param2{ get; }
		string Param3{ get; }
	}
	[TrackClipType( typeof( RepeatClip))]
	sealed class RepeatMixerBehaviour : PlayableBehaviour
	{
		internal void Initalize( PlayableDirector playableDirector, RepeatReceiver receiver, RepeatTrack track)
		{
			m_PlayableDirector = playableDirector;
			m_Receiver = receiver;
			m_Track = track;
		}
		internal ISeekTarget GetDefaultTarget()
		{
			if( m_CurrentBehaviour != null)
			{
				return m_CurrentBehaviour.GetDefaultTarget();
			}
			return null;
		}
		internal ISeekTarget[] GetOtherTargets()
		{
			if( m_CurrentBehaviour != null)
			{
				return m_CurrentBehaviour.GetOtherTargets().ToArray();
			}
			return new ISeekTarget[ 0];
		}
		internal bool Abort( string targetName)
		{
			if( m_CurrentBehaviour != null)
			{
				if( Abort( m_CurrentBehaviour, targetName) != false)
				{
					m_CurrentBehaviour = null;
					return true;
				}
			}
			return false;
		}
		public override void OnGraphStart( Playable playable)
		{
			int inputCount = playable.GetInputCount();
			m_Behaviours.Clear();
			
			for( int i0 = 0; i0 < inputCount; ++i0)
			{
				var inputPlayable = (ScriptPlayable<RepeatBehaviour>)playable.GetInput( i0);
				RepeatBehaviour behaviour = inputPlayable.GetBehaviour();
				m_Behaviours.Add( behaviour);
			}
			if( m_Receiver is RepeatProxy proxy)
			{
			#if UNITY_EDITOR
				if(	m_PlayableDirector.timeUpdateMode == DirectorUpdateMode.DSPClock)
				{
					Debug.LogError( "RepeatProxy は PlayableDirector の Update Method - DPS Clock に対応していません", m_PlayableDirector);
				}
			#endif
				proxy.OnGraphStart( m_PlayableDirector, this);
			}
		}
		public override void ProcessFrame( Playable playable, FrameData info, object playerData)
		{
		#if UNITY_EDITOR
			if( Application.isPlaying == false
			&&	m_PlayableDirector.state != PlayState.Playing)
			{
				m_CurrentBehaviour = null;
				return;
			}
			if( Application.isPlaying != false && m_Receiver is RepeatProxy)
			{
				return;
			}
		#endif
			ProcessFrame( m_PlayableDirector.time);
		}
		internal void ProcessFrame( double currentTime)
		{
			if( m_Behaviours.Count <= 0)
			{
				return;
			}
			if( m_CurrentBehaviour != null)
			{
				if( m_CurrentBehaviour.EndTime <= currentTime)
				{
					if( OnLeave( m_CurrentBehaviour, ref currentTime) != false)
					{
						m_CurrentBehaviour = null;
					}
				}
			}
			if( m_CurrentBehaviour == null)
			{
				for( int i0 = 0; i0 < m_Behaviours.Count; ++i0)
				{
					switch( m_Behaviours[ i0])
					{
						case RepeatBehaviour behaviour:
						{
							if( OnJoin( behaviour, currentTime) != false)
							{
								i0 = m_Behaviours.Count;
							}
							break;
						}
						case DelayTarget delay:
						{
							if( delay.EndTime <= currentTime)
							{
								if( Seek( SeekMode.Start, delay.SeekTarget) != false)
								{
									currentTime = m_PlayableDirector.time;
								}
								m_Behaviours.RemoveAt( i0--);
							}
							break;
						}
					}
				}
			}
		}
		bool OnJoin( RepeatBehaviour behaviour, double currentTime)
		{
			if( behaviour.StartTime <= currentTime && behaviour.EndTime > currentTime)
			{
				m_CurrentCount = behaviour.PlayCount;
				m_CurrentBehaviour = behaviour;
				
				if( behaviour.JoinEventCallAtEnd == false)
				{
					m_Receiver?.OnJoin( m_Track);
					m_JoinState |= JoinState.Called;
				}
				return true;
			}
			return false;
		}
		bool OnLeave( RepeatBehaviour behaviour, ref double currentTime)
		{
			if( m_CurrentCount.HasValue != false)
			{
				if( m_CurrentCount < 0)
				{
					if( (m_JoinState & JoinState.Called) == 0)
					{
						m_Receiver?.OnJoin( m_Track);
						m_JoinState |= JoinState.Called;
					}
					if( Seek( SeekMode.Start, behaviour) != false)
					{
						currentTime = m_PlayableDirector.time;
					}
				}
				else if( m_CurrentCount > 0)
				{
					--m_CurrentCount;
					
					if( m_CurrentCount > 0)
					{
						if( (m_JoinState & JoinState.Called) == 0)
						{
							m_Receiver?.OnJoin( m_Track);
							m_JoinState |= JoinState.Called;
						}
						if( Seek( SeekMode.Start, behaviour) != false)
						{
							currentTime = m_PlayableDirector.time;
						}
					}
					else if( behaviour.DefaultSeekMode != SeekMode.None)
					{
						ISection target = behaviour.DefaultTarget;
						
						if( target == null)
						{
							target = behaviour;
						}
						if( Seek( behaviour.DefaultSeekMode, target) != false)
						{
							currentTime = m_PlayableDirector.time;
						}
					}
				}
				if( m_CurrentCount == 0)
				{
					if( (m_JoinState & JoinState.Called) != 0)
					{
						m_Receiver?.OnLeave();
						m_JoinState = JoinState.None;
					}
					m_CurrentCount = null;
					return true;
				}
			}
			return false;
		}
		bool Seek( SeekMode mode, ISection target)
		{
			if( (m_Receiver?.OnSeek( target) ?? true) != false)
			{
				double time = mode switch
				{
					SeekMode.Start => target?.StartTime ?? -1,
					SeekMode.End => target?.EndTime ?? -1,
					_ => -1
				};
				if( time >= 0)
				{
					m_PlayableDirector.time = time;
					
					if( m_Receiver is RepeatProxy)
					{
						m_PlayableDirector.DeferredEvaluate();
					}
					else
					{
						m_PlayableDirector.Evaluate();
					}
					return true;
				}
			}
			return false;
		}
		bool Abort( RepeatBehaviour behaviour, string targetName)
		{
			if( m_CurrentCount.HasValue != false)
			{
				if( m_CurrentCount != 0)
				{
					m_CurrentCount = 0;
					
					ISection target = null;
					
					if( string.IsNullOrEmpty( targetName) == false)
					{
						target = behaviour.GetTarget( targetName);
					}
					if( target != null)
					{
						if( behaviour.EntrustTarget != null)
						{
							m_Behaviours.Add( new DelayTarget( behaviour.EntrustTarget, target));
							m_Behaviours.Sort( (a, b) => Math.Sign( a.StartTime - b.StartTime));
							Seek( SeekMode.End, behaviour);
						}
						else
						{
							Seek( SeekMode.Start, target);
						}
					}
					// else if( behaviour.DefaultTarget != null)
					// {
					// 	target = behaviour.DefaultTarget;
						
					// 	if( behaviour.EntrustTarget != null)
					// 	{
					// 		m_Behaviours.Add( new DelayTarget( behaviour.EntrustTarget, target));
					// 		m_Behaviours.Sort( (a, b) => Math.Sign( a.StartTime - b.StartTime));
					// 		Seek( SeekMode.End, behaviour);
					// 	}
					// 	else
					// 	{
					// 		Seek( SeekMode.Start, target);
					// 	}
					// }
					else
					{
						Seek( SeekMode.End, behaviour);
					}
					if( (m_JoinState & JoinState.Called) != 0)
					{
						m_Receiver?.OnLeave();
						m_JoinState = JoinState.None;
					}
					return true;
				}
			}
			return false;
		}
		internal sealed class DelayTarget : ISection
		{
			internal DelayTarget( ISection delayTarget, ISection seekTarget)
			{
				m_EntrustSection = delayTarget;
				m_SeekTarget = seekTarget;
			}
			public double StartTime
			{
				get{ return m_EntrustSection.StartTime; }
			}
			public double EndTime
			{
				get{ return m_EntrustSection.EndTime; }
			}
			internal ISection SeekTarget
			{
				get{ return m_SeekTarget; }
			}
			ISection m_EntrustSection;
			ISection m_SeekTarget;
		}
		[Flags]
		internal enum JoinState
		{
			None = 0,
			Called = 1 << 0,
		}
		PlayableDirector m_PlayableDirector;
		RepeatReceiver m_Receiver;
		RepeatTrack m_Track;
		
		readonly List<ISection> m_Behaviours = new();
		RepeatBehaviour m_CurrentBehaviour;
		JoinState m_JoinState;
		int? m_CurrentCount;
	}
}
