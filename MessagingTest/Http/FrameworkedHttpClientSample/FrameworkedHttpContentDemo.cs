using Messaging.Http.Content;

namespace MessagingTest.Http.FrameworkedHttpClientSample;

public class FrameworkedHttpContentDemo(string url) : HttpStringContent(url)
{
}