
using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	[Serializable]
	sealed class TextBehaviour : PlayableBehaviour
	{
		[SerializeField, TextArea( 1, 6)]
		internal string m_Text;
	}
}
