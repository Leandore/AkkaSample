using System;
using Akka.Actor;
using System.Collections.Generic;
using Akka.Configuration;
using KT_Core;
using KT_Core.Actors;

namespace KT_2
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
      Console.Title = "Initiator";
      var columnTotals = GetTotalColumns();
      short totalRows = 3;

      var config = ConfigurationFactory.ParseString(@"
      akka
      {
        actor
        {

  provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""

          deployment {
              /Initiator {
                  router = round-robin-pool
                  resizer {
                      enabled = on
                      lower-bound = 10
                      upper-bound = 500
                  }
              }
          }
        }
        remote 
        {
          helios.tcp
          {
           port = 5290
           hostname = localhost         
          }
        }


      }

");
      var myActorSystem = ActorSystem.Create("KingsRouteSystem", config);
      var router = myActorSystem.ActorOf(Node.Props(columnTotals), $"Initiator");
     
      var q = new Queue<string>();
      q.Enqueue("1-1");
      q.Enqueue("1-2");
      router.Tell(new InitNextMove(q, 1, 2, 2));

      q = new Queue<string>();
      q.Enqueue("1-1");
      q.Enqueue("2-1");
      router.Tell(new InitNextMove(q, 2, 1, 2));

      q = new Queue<string>();
      q.Enqueue("1-1");
      q.Enqueue("2-2");
      router.Tell(new InitNextMove(q, 2, 2, 2));

      Console.ReadLine();
    }
  }


}
