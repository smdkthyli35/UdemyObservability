using System.Diagnostics;

namespace UdemyObservability.ConsoleApp
{
    internal class ServiceOne
    {
        static HttpClient httpClient = new();

        internal async Task<int> MakeRequestToGoogle()
        {
            using var activity = ActivitySourceProvider.Source.StartActivity(kind: System.Diagnostics.ActivityKind.Producer, name: "CustomMakeRequestToGoogle");

            try
            {
                ActivityTagsCollection eventTags = new();
                activity?.AddEvent(new("Google'a istek başladı!", tags: eventTags));
                activity?.AddTag("request.schema", "https");
                activity?.AddTag("request.method", "get");

                var result = await httpClient.GetAsync("https://www.google.com");
                var responseContent = await result.Content.ReadAsStringAsync();

                activity?.AddTag("response.length", responseContent.Length);

                eventTags.Add("Google Response Lengst", responseContent.Length);
                activity?.AddEvent(new("Google'a istek tamamlandı!", tags: eventTags));
                return responseContent.Length;
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return -1;
            }
        }
    }
}