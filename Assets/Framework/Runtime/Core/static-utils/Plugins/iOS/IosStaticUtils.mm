
extern "C"
{
    void _IosStaticUtils_OpenStorePage(const char* appId)
    {
        NSString* url = [NSString stringWithFormat:@"http://itunes.apple.com/app/id%s?mt=8",appId];
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:url]];
    }
}
