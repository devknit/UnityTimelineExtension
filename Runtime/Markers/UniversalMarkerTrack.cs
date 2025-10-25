
using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace Knit.TimelineExtension
{
	[Serializable]
    [ExcludeFromPreset]
    [TrackColor( 0.75f, 0.75f, 0.75f)]
    [TrackBindingType( typeof( UniversalMarkerReceiver))]
	sealed class UniversalMarkerTrack : MarkerTrack{}
}
