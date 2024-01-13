namespace UdemyObservability.ConsoleApp
{
    internal class ServiceOne
    {
        static HttpClient httpClient = new();

        internal async Task<int> MakeRequestToGoogle()
        {
            using var activity = ActivitySourceProvider.Source.StartActivity(kind: System.Diagnostics.ActivityKind.Producer, name: "CustomMakeRequestToGoogle");
            var result = await httpClient.GetAsync("https://www.google.com");
            var responseContent = await result.Content.ReadAsStringAsync();
            return responseContent.Length;
        }
    }
}