using System.Diagnostics;

namespace BuildingBlocks.Diagnostics;

public class Instrumentation
{
   public static ActivitySource Source = new ActivitySource("notifications");
}