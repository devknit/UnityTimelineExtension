
using UnityEngine;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	internal static class QuaternionUtility
	{
		internal static Quaternion Add( this Quaternion first, Quaternion second)
		{
			first.w += second.w;
			first.x += second.x;
			first.y += second.y;
			first.z += second.z;
			return first;
		}
		internal static Quaternion Scale( this Quaternion rotation, float multiplier)
		{
			rotation.w *= multiplier;
			rotation.x *= multiplier;
			rotation.y *= multiplier;
			rotation.z *= multiplier;
			return rotation;
		}
		internal static Quaternion Lerp( Quaternion p, Quaternion q, float t, bool shortest)
		{
			if( shortest != false)
			{
				if( Quaternion.Dot( p, q) < 0.0f)
				{
					return Lerp( Scale( p, -1.0f), q, t, true);
				}
			}
			Quaternion r = Quaternion.identity;
			float it = 1.0f - t;
			r.x = p.x * it + q.x * t;
			r.y = p.y * it + q.y * t;
			r.z = p.z * it + q.z * t;
			r.w = p.w * it + q.w * t;
			return r;
		}
		internal static Quaternion Slerp( Quaternion p, Quaternion q, float t, bool shortest)
		{
			float dot = Quaternion.Dot( p, q);
			
			if( shortest != false)
			{
				if( dot < 0.0f)
				{
					return Slerp( Scale( p, -1.0f), q, t, true);
				}
			}
			float angle = Mathf.Acos( dot);
			Quaternion first = Scale( p, Mathf.Sin( (1.0f - t) * angle));
			Quaternion second = Scale( q, Mathf.Sin( t * angle));
			float division = 1.0f / Mathf.Sin( angle);
			return Scale( Add( first, second), division);
		}
		internal static float Magnitude( Quaternion rotation)
		{
			return Mathf.Sqrt( Quaternion.Dot( rotation, rotation));
		}
		internal static Quaternion Normalize( Quaternion rotation)
		{
			float magnitude = Magnitude( rotation);
			
			if( magnitude > 0.0f)
			{
				return Scale( rotation, 1.0f / magnitude);
			}
			Debug.LogWarning( "Cannot normalize a quaternion with zero magnitude.");
			return Quaternion.identity;
		}
	}
}
