using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemyObservability.ConsoleApp
{
    internal class ServiceTwo
    {
        internal async Task<int> WriteToFile(string text)
        {
            Activity.Current?.SetTag("Güncel activity", "1");

            using (var activity = ActivitySourceProvider.Source.StartActivity("a"))
            {
                await File.WriteAllTextAsync("myFile.txt", text);
                var a = (await File.ReadAllTextAsync("myFile.txt")).Length;
            }

            using (var activity2 = ActivitySourceProvider.Source.StartActivity("b"))
            {
                await File.WriteAllTextAsync("myFile.txt", text);
                return (await File.ReadAllTextAsync("myFile.txt")).Length;
            }

        }
    }
}
