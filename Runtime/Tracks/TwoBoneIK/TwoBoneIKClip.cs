
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	[System.Serializable]
	sealed class TwoBoneIKClip : PlayableAsset, ITimelineClipAsset, IBindableClip
	{
		public ClipCaps clipCaps
		{
			get { return ClipCaps.Extrapolation | ClipCaps.Blending; }
		}
		public override Playable CreatePlayable( PlayableGraph graph, GameObject owner)
		{
			var playable = ScriptPlayable<TwoBoneIKBehaviour>.Create( graph, m_Source);
			playable.GetBehaviour().Initialize( m_Target.Resolve( graph.GetResolver()), m_Hint.Resolve( graph.GetResolver()));
			return playable;
		}
		public int GetAssignFieldNameCount()
		{
			return 2;
		}
		public string GetAssignFieldName( int index)
		{
			return index switch
			{
				0 => "Target",
				1 => "Hint",
				_ => string.Empty
			};
		}
		public System.Type GetAssignFieldType( string fieldName)
		{
			return typeof(Transform);
		}
		public void SetBindObject( Object obj, string fieldName)
		{
			switch( fieldName)
			{
				case "Target":
				{
					m_Target = new ExposedReference<Transform>{ defaultValue = obj as Transform };
					break;
				}
				case "Hint":
				{
					m_Hint = new ExposedReference<Transform>{ defaultValue = obj as Transform };
					break;
				}
			}
		}
		[SerializeField]
		TwoBoneIKBehaviour m_Source = new();
		[SerializeField]
		ExposedReference<Transform> m_Target;
		[SerializeField]
		ExposedReference<Transform> m_Hint;
	}
}
