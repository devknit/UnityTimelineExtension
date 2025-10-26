
using UnityEngine;

namespace Knit.Timeline
{
	public sealed class BindableObject
	{
		internal BindableObject( Object obj, BindableObject parent)
		{
			m_Object = obj;
			m_Parent = parent;
			
			string objectName = obj.name;
			
			if( objectName.Length > 0 && objectName[ 0] == Binder.kBindableSymbol)
			{
				objectName = objectName[ 1..];
			}
			
			m_ObjectName = objectName;
		}
		internal BindableObject Parent
		{
			get { return m_Parent; }
		}
		public Object Object
		{
			get { return m_Object; }
		}
		internal string ObjectName
		{
			get { return m_ObjectName; }
		}
		BindableObject m_Parent;
		Object m_Object;
		string m_ObjectName;
	}
}