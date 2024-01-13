namespace UdemyObservability.ConsoleApp
{
    internal class ServiceHelper
    {
        internal async Task Work1()
        {
            using var activity = ActivitySourceProvider.Source.StartActivity();

            ServiceOne serviceOne = new();
            Console.WriteLine($"Google Response Length: {await serviceOne.MakeRequestToGoogle()}");
            Console.WriteLine("Work1 completed!");

            ServiceTwo serviceTwo = new();
            var fileLength = await serviceTwo.WriteToFile("Hello World!");
        }
    }
}
