namespace SagoApp.Content {
	
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	
	[iOSSettingsGroup(typeof(iOSSettingsGroupGeneral), "GetiOSSettingsGroupConfig")]
	internal class iOSSettingsGroupGeneral {
		
		
		static private iOSSettingsGroupConfig GetiOSSettingsGroupConfig() {
			iOSSettingsGroupConfig config;
			config.TitleKey = "General.Title";
			config.TitleLocalization = new Dictionary<string,string>() {
				{ "en","Settings" },
				{ "de","Einstellungen" },
				{ "fr","Paramètres" },
				{ "ja","設定" },
				{ "ru","Настройки" },
				{ "zh-Hans","设定" },
				{ "zh-Hant","設定" },
				{ "es","Configuraciones" },
				{ "pt-BR","Configurações" }
			};
			config.Priority = 999;
			return config; 
		}
		
		
		[iOSToggleSwitch(typeof(iOSSettingsGroupGeneral), "GetiOSToggleSwitchConfig_SagoNews")]
		static public string sagoNewsKey = "sagoNews";
		
		static private iOSToggleSwitchConfig GetiOSToggleSwitchConfig_SagoNews() {
			iOSToggleSwitchConfig config;
			config.TitleKey = "General.News";
			config.TitleLocalization = new Dictionary<string, string> {
				{ "en","Sago News" },
				{ "de","Sago-Neuigkeiten" },
				{ "fr","Nouvelles Sago" },
				{ "ja","サゴニュース" },
				{ "ru","Новости Sago" },
				{ "zh-Hans","Sago新闻" },
				{ "zh-Hant","Sago新聞" },
				{ "es","Novedades de Sago" },
				{ "pt-BR","Novidades Sago" }
			};
			config.DefaultValue = true;
			return config; 
		}
		
		
		[iOSToggleSwitch(typeof(iOSSettingsGroupGeneral), "GetiOSToggleSwitchConfig_ForParents")]
		static public string forParentsKey = "forParents";
		
		static private iOSToggleSwitchConfig GetiOSToggleSwitchConfig_ForParents() {
			iOSToggleSwitchConfig config;
			config.TitleKey = "General.ForParents";
			config.TitleLocalization = new Dictionary<string, string> {
				{ "en","For Parents" },
				{ "de","Für Eltern" },
				{ "fr","Pour les parents" },
				{ "ja","大人の方へ" },
				{ "ru","Родителям" },
				{ "zh-Hans","父母专用" },
				{ "zh-Hant","家長專用" },
				{ "es","Para los padres" },
				{ "pt-BR","Para os pais" }
			};
			config.DefaultValue = true;
			return config; 
		}
		
		
	}
}