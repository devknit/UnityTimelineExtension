
using System;

namespace Knit.Timeline
{
	public interface IBindableClip
	{
		public int GetAssignFieldNameCount();
		public string GetAssignFieldName( int index);
		public Type GetAssignFieldType( string fieldName);
		public void SetBindObject( UnityEngine.Object obj, string fieldName);
	}
}