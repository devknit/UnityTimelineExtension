
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

namespace Knit.TimelineExtension
{
	sealed class TextMixerBehaviour : PlayableBehaviour
	{
		public override void ProcessFrame( Playable playable, FrameData info, object playerData)
		{
			if( playerData is TMP_Text component)
			{
				if( m_Component == null)
				{
					m_Component = component;
					m_DefaultText = component.text;
				}
				if( m_Component != null)
				{
					int inputCount = playable.GetInputCount();
					string text = m_DefaultText;
					
					for( int i0 = 0; i0 < inputCount; ++i0)
					{
						if( playable.GetInputWeight( i0) > 0.0f)
						{
							var inputPlayable = (ScriptPlayable<TextBehaviour>)playable.GetInput( i0);
							text = inputPlayable.GetBehaviour().m_Text;
							break;
						}
					}
					if( m_CurrentText != text)
					{
						m_CurrentText = text;
						m_Component.SetText( text);
					}
				}
			}
		}
		public override void OnPlayableDestroy( Playable playable)
		{
			if( m_Component != null)
			{
				m_Component.SetText( m_DefaultText);
				m_Component = null;
			}
		}
		string m_DefaultText;
		string m_CurrentText;
		TMP_Text m_Component;
	}
}
