#if !UNITY_WSA && !UNITY_EDITOR_WIN

#if !UNITY_WINRT || UNITY_EDITOR || (UNITY_WP8 &&  !UNITY_WP_8_1)
using System;

namespace Newtonsoft.Json.ObservableSupport
{
	public delegate void PropertyChangingEventHandler(Object sender, PropertyChangingEventArgs e);

}

#endif

#endif
