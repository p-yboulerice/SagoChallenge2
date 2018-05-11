#import "PhotoUtil.h"

extern "C" {
	
	void _savePhotoAtPath(const char *path, int deleteAfter) {
		[PhotoUtil 
			savePhotoAtPath:[NSString stringWithUTF8String:path == NULL ? "" : path]
			deleteAfter:(bool)deleteAfter];
	}
	
}

@implementation PhotoUtil

+ (void)savePhotoAtPath:(NSString *)path deleteAfter:(BOOL)deleteAfter {
	
	UIImage *photo = [UIImage imageWithContentsOfFile:path];
    
    NSData *pngImageData =  UIImagePNGRepresentation(photo);
    photo = [UIImage imageWithData:pngImageData];
	
	if (deleteAfter) {
		NSString *photoPathToDelete = [[NSString alloc] initWithString:path];
		UIImageWriteToSavedPhotosAlbum(
			photo, 
			self, 
			@selector(deletePhotoAfterSave:didFinishSavingWithError:contextInfo:), 
			photoPathToDelete
		);
	} else {
		UIImageWriteToSavedPhotosAlbum(
			photo, 
			nil, 
			nil, 
			NULL
		);
	}
	
}

+ (void)deletePhotoAfterSave:(UIImage *)image didFinishSavingWithError:(NSError *)error contextInfo:(void *)contextInfo {
	NSString *photoPathToDelete = (NSString *)contextInfo;
	[NSFileManager.defaultManager removeItemAtPath:photoPathToDelete error:nil];
	[photoPathToDelete release];
}

@end