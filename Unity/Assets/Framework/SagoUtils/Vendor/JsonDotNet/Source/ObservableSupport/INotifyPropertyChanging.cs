#if !UNITY_WSA && !UNITY_EDITOR_WIN

#if !UNITY_WINRT || UNITY_EDITOR || (UNITY_WP8 &&  !UNITY_WP_8_1)
namespace Newtonsoft.Json.ObservableSupport
{
	public interface INotifyPropertyChanging
	{
		event PropertyChangingEventHandler PropertyChanging;
	}
}

#endif

#endif
