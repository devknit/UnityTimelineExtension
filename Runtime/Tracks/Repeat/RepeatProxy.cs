
using UnityEngine;
using UnityEngine.Playables;

namespace Knit.TimelineExtension
{
	public class RepeatProxy : RepeatReceiver
	{
		internal void OnGraphStart( PlayableDirector playableDirector, RepeatMixerBehaviour mixerBehaviour)
		{
			m_PlayableDirector = playableDirector;
			m_MixerBehaviour = mixerBehaviour;
		}
		internal protected override void OnJoin( IRepeatTrack track)
		{
		}
		internal protected override void OnLeave()
		{
		}
		internal protected override bool OnSeek( ISection target)
		{
			return true;
		}
		protected virtual void Update()
		{
			if( m_PlayableDirector != null && m_MixerBehaviour != null)
			{
				double currentTime = m_PlayableDirector.time;
				
				currentTime += m_PlayableDirector.timeUpdateMode switch
				{
					DirectorUpdateMode.GameTime => Time.deltaTime,
					DirectorUpdateMode.UnscaledGameTime => Time.unscaledDeltaTime,
					_ => 0
				};
				m_MixerBehaviour.ProcessFrame( currentTime);
			}
		}
		public PlayableDirector PlayableDirector
		{
			get{ return m_PlayableDirector; }
		}
		PlayableDirector m_PlayableDirector;
		RepeatMixerBehaviour m_MixerBehaviour;
	}
}
