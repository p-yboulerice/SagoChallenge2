#import <Foundation/Foundation.h>

char* _SagoLocalization_makeCharArray(NSString *string) {
	return strdup([string UTF8String]);
}

extern "C" {
	char* _SagoLocalization_currentLocaleJson() {
		
		NSDictionary *dictionary;
		dictionary = @{
			@"localeIdentifier" : [[[NSLocale currentLocale] localeIdentifier] stringByReplacingOccurrencesOfString:(NSString *)@"_" withString:(NSString *)@"-"], 
			@"preferredLanguageIdentifiers" : [NSLocale preferredLanguages]
		};
		
		NSError *error;
		error = nil;
		
		NSData *data;
		data = [NSJSONSerialization dataWithJSONObject:dictionary options:NSJSONWritingPrettyPrinted error:&error];
		
		NSString *json;
		json = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
		
		NSLog(@"_SagoLocalization_currentLocaleJson: %@", json);
		return _SagoLocalization_makeCharArray(json);
		
	}
}
