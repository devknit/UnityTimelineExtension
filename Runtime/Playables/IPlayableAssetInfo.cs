
namespace Knit.Timeline
{
	public interface IPlayableAssetInfo
	{
		public bool TryGetValue( string name, out UnityEngine.Object asset);
	}
}
