
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using System.Linq;
using System.Collections.Generic;

namespace Knit.TimelineExtension.Test
{
	[RequireComponent(typeof(PlayableDirector))]
	sealed internal class PlayableBinder : MonoBehaviour
	{
		public void Bind()
		{
			Dictionary<string, HashSet<BindableObject>> testDic = FindObject();
			
			m_Director.Bind( testDic);
			m_Objects = testDic.Values.SelectMany(x => x).Select( x => x.Object).ToArray();
		}
		public void UnBind()
		{
			m_Director.UnBind();
		}
		public Dictionary<string, HashSet<BindableObject>> FindObject()
		{
			Dictionary<string, HashSet<BindableObject>> testObjects = new Dictionary<string, HashSet<BindableObject>>();
			
			var findParam = new Binder.FindParam( true, null);
		#if false
			findParam.AddNameRules( "Test", typeof(GameObject));
			findParam.AddNameRules( "ABC", typeof(Transform));
			findParam.AddTypeRules( typeof(RepeatReceiver), typeof(RepeatReceiver));
			findParam.AddTypeRules( typeof(TimeScaleReceiver), typeof(TimeScaleReceiver));
			findParam.AddTypeRules( typeof(VideoPlayer), typeof(VideoPlayer));
			findParam.AddTypeRules( typeof(Light), typeof(Light));
			findParam.AddTypeRules( typeof(Animator), typeof(Animator));
			findParam.AddTypeRules( typeof(UniversalMarkerReceiver), typeof(UniversalMarkerReceiver));
		#endif
			Binder.FindObjects( testObjects, findParam, true);
			
			return testObjects;
		}
		void Start()
		{
			Debug.LogWarning( "RepeatTrackReceiver はサンプルとしての使用にのみ使用できます");
		}
		void Reset()
		{
			m_Director = GetComponent<PlayableDirector>();
		}
		[SerializeField]
		PlayableDirector m_Director;
		[SerializeField]
		Object[] m_Objects;
	}
}