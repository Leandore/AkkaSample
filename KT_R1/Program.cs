using System;
using Akka.Actor;
using Akka.Configuration;
using KT_Core.Actors;
using KT_Core;
using Akka.Routing;
using System.Collections.Generic;

namespace KT_R1
{
  using Akka.Actor;
  using Akka.Configuration;
  using KT_Core.Actors;
  using System;

  namespace KT_R2
  {
    class Program
    {
      private static uint GetTotalColumns()
      {
        Console.WriteLine("How many columns?");
        var userInput = Console.ReadLine();
        uint totalColumns;
        if (!uint.TryParse(userInput, out totalColumns))
        {
          GetTotalColumns();
        }
        return totalColumns;
      }

      static void Main(string[] args)
      {
        Console.Title = "Validator Clone 1";

        var columnTotals = GetTotalColumns();
        short totalRows = 3;

        var config = ConfigurationFactory.ParseString(@"
      akka
      {
        actor
        {
          provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
          deployment {
              /Executor {
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
           port = 5395
           hostname = localhost         
          }
        }


      }

");
        var myActorSystem = ActorSystem.Create("Validator", config);
        var router = myActorSystem.ActorOf(Validator.Props(columnTotals), $"Executor");

        Console.WriteLine("Validator ready");
        Console.ReadLine();
      }
    }
  }

}
