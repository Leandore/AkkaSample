using Akka.Actor;
using Akka.Configuration;
using KT_Core.Actors;
using System;

namespace KT_HistoryManagement
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.Title = "History Manager";
      var config = ConfigurationFactory.ParseString(@"
      akka
      {
        actor
        {

  provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""

          deployment {
              /Trimmer {
                  router = round-robin-pool
                  resizer {
                      enabled = on
                      lower-bound = 10
                      upper-bound = 5000
                  }
              }
          }
        }
        remote 
        {
          helios.tcp
          {
           port = 5296
           hostname = localhost         
          }
        }
      }
");
      var myActorSystem = ActorSystem.Create("HistoryTrimmerSystem", config);
      var router = myActorSystem.ActorOf(HistoryTrimmer.Props(), $"Trimmer");

      Console.WriteLine("Ready");
      Console.ReadLine();
    }
  }
}
