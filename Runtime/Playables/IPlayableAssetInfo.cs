
namespace Knit.TimelineExtension
{
	public interface IPlayableAssetInfo
	{
		public bool TryGetValue( string name, out UnityEngine.Object asset);
	}
}
