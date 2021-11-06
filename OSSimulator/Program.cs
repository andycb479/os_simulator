using System;
using System.Threading;
using System.Timers;

namespace OSSimulator
{
     class Program
     {
          static void Main(string[] args)
          {
               var os = new OperatingSystem();
               os.Run();
          }
     }
}
