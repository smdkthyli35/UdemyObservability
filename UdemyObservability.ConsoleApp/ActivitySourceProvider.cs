﻿using System.Diagnostics;

namespace UdemyObservability.ConsoleApp
{
    internal static class ActivitySourceProvider
    {
        internal static ActivitySource Source = new(OpenTelemetryConstants.ActivitySourceName);
    }
}
