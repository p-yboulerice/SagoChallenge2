#import "Foundation/Foundation.h"
#if ENABLE_IOS_ON_DEMAND_RESOURCES
	#import "Foundation/NSBundle.h"
#endif


typedef void (*OnDemandResourcesRequestCompleteHandler)(void* handlerData, const char* error);


#if ENABLE_IOS_ON_DEMAND_RESOURCES


struct OnDemandResourcesRequestData
{
	NSBundleResourceRequest* request;
	bool hasCalledCompletionHandler;
	bool wasCancelled;
};


extern "C" OnDemandResourcesRequestData* UnityOnDemandResourcesCreateRequest(NSSet* tags, OnDemandResourcesRequestCompleteHandler handler, void* handlerData)
{
	OnDemandResourcesRequestData* data = new OnDemandResourcesRequestData();
	data->request = [[NSBundleResourceRequest alloc] initWithTags:tags];
	data->hasCalledCompletionHandler = false;
	data->wasCancelled = false;
	[data->request beginAccessingResourcesWithCompletionHandler:^(NSError* error){
		NSLog(@"[ODR:Complete] data->hasCalledCompletionHandler = true");
		data->hasCalledCompletionHandler = true;
		dispatch_async(dispatch_get_main_queue(), ^{
			if (data->wasCancelled) {
				NSLog(@"[ODR:Complete] data->wasCancelled = true");
				NSLog(@"[ODR:Complete] [data->request endAccessingResources]");
				[data->request endAccessingResources];
				NSLog(@"[ODR:Complete] delete data");
				delete data;
			} else {
				NSLog(@"[ODR:Complete] data->wasCancelled = false");
				NSLog(@"[ODR:Complete] handler(handlerData, errorMessage)");
				const char* errorMessage = error ? [[error localizedDescription] UTF8String] : NULL;
				handler(handlerData, errorMessage);
			}
		});
	}];
	return data;
}


extern "C" void UnityOnDemandResourcesRelease(OnDemandResourcesRequestData* data)
{
	if (data->hasCalledCompletionHandler) {
		NSLog(@"[ODR:Release] data->hasCalledCompletionHandler = true");
		NSLog(@"[ODR:Release] [data->request endAccessingResources]");
		[data->request endAccessingResources];
		NSLog(@"[ODR:Release] delete data");
		delete data;
	} else {
		NSLog(@"[ODR:Release] data->hasCalledCompletionHandler = true");
		data->wasCancelled = true;
		NSLog(@"[ODR:Release] [data->request.progress cancel]");
		[data->request.progress cancel];
	}
}


extern "C" float UnityOnDemandResourcesGetProgress(OnDemandResourcesRequestData* data)
{
	return data->request.progress.fractionCompleted;
}


extern "C" float UnityOnDemandResourcesGetLoadingPriority(OnDemandResourcesRequestData* data)
{
	float priority = (float)data->request.loadingPriority;
	return priority;
}


extern "C" void UnityOnDemandResourcesSetLoadingPriority(OnDemandResourcesRequestData* data, float priority)
{
	if (priority < 0.0f)
		priority = 0.0f;

	if (priority > 1.0f)
		data->request.loadingPriority = NSBundleResourceRequestLoadingPriorityUrgent;
	else
		data->request.loadingPriority = (double)priority;
}


extern "C" NSString* UnityOnDemandResourcesGetResourcePath(OnDemandResourcesRequestData* data, const char* resource)
{
	NSString* path = [[data->request bundle] pathForResource:[NSString stringWithUTF8String:resource] ofType:nil];
	return path;
}


#else // ENABLE_IOS_ON_DEMAND_RESOURCES


struct OnDemandResourcesRequestData
{
};


extern "C" OnDemandResourcesRequestData* UnityOnDemandResourcesCreateRequest(NSSet* tags, OnDemandResourcesRequestCompleteHandler handler, void* handlerData)
{
	OnDemandResourcesRequestData* data = new OnDemandResourcesRequestData();
	if (handler)
		handler(handlerData, NULL);
	return data;
}


extern "C" void UnityOnDemandResourcesRelease(OnDemandResourcesRequestData* data)
{
	delete data;
}


extern "C" float UnityOnDemandResourcesGetProgress(OnDemandResourcesRequestData* data)
{
	return 1.0f;
}


extern "C" float UnityOnDemandResourcesGetLoadingPriority(OnDemandResourcesRequestData* data)
{
	return 0.5;
}


extern "C" void UnityOnDemandResourcesSetLoadingPriority(OnDemandResourcesRequestData* data, float priority)
{
}


extern "C" NSString* UnityOnDemandResourcesGetResourcePath(OnDemandResourcesRequestData* data, const char* resource)
{
	return [NSString stringWithUTF8String:resource];
}


#endif // ENABLE_IOS_ON_DEMAND_RESOURCES
