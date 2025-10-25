
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.TimelineExtension
{
	[System.Serializable]
	sealed class MultiAimClip : PlayableAsset, ITimelineClipAsset, IBindableClip
	{
		public ClipCaps clipCaps
		{
			get { return ClipCaps.Extrapolation | ClipCaps.Blending; }
		}
		public override Playable CreatePlayable( PlayableGraph graph, GameObject owner)
		{
			var playable = ScriptPlayable<MultiAimBehaviour>.Create( graph, m_Source);
			playable.GetBehaviour().Initialize( m_Target.Resolve( graph.GetResolver()));
			return playable;
		}
		public int GetAssignFieldNameCount()
		{
			return 1;
		}
		public string GetAssignFieldName( int index)
		{
			return "Target";
		}
		public System.Type GetAssignFieldType( string fieldName)
		{
			return typeof(Transform);
		}
		public void SetBindObject( Object obj, string fieldName)
		{
			m_Target = new ExposedReference<Transform>{ defaultValue = obj as Transform };
		}
		[SerializeField]
		MultiAimBehaviour m_Source = new();
		[SerializeField]
		ExposedReference<Transform> m_Target;
	}
}
