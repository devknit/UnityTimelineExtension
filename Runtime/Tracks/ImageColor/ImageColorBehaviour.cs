
using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Knit.TimelineExtension
{
	[Serializable]
	sealed class ImageColorBehaviour : PlayableBehaviour
	{
		[SerializeField]
		internal Color m_Color = Color.white;
	}
}
