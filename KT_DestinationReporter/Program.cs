using System;
using Akka.Actor;
using Akka.Configuration;
using Akka.Routing;

namespace KT_DestinationReporter
{
  public class DestinationReporter : ReceiveActor
  {
    private long _totalHits;
    public DestinationReporter()
    {
      //Receive<ReportDestinationReached>(h =>
      //{
      //  _totalHits = _totalHits + 1;
      //  Console.WriteLine($"!!!!!!!!!!!!!!!!! ##############****************############## !!!!!!!!!!!!!!!!! Total hits : {_totalHits}");
      //});

      Receive<int>(h =>
      {
        _totalHits = _totalHits + 1;
        Console.WriteLine($"Total hits : {_totalHits}");
      });
    }

    #region Messages
    public struct ReportDestinationReached
    {

    }
    #endregion Messages

    #region Construct Node

    public static Props Props()
    {

      return Akka.Actor.Props.Create(() => new DestinationReporter())
        .WithRouter(FromConfig.Instance);
    }

    #endregion Construct Node
  }

  class Program
  {
    static void Main(string[] args)
    {
      var config = ConfigurationFactory.ParseString(@"
      akka
      {
        actor
        {
          provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""

          deployment {
              /my-router1 {
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
           port = 5300
           hostname = localhost         
          }
        }


      }

");

      string actorSystemName = "DestinationReporterSystem";
      Console.Title = actorSystemName;

      try
      {
        using (var actorSystem = ActorSystem.Create(actorSystemName, config))
        {
          actorSystem.ActorOf(Props.Create<DestinationReporter>(), "DestinationReporter");

          Console.WriteLine("Awaiting hits");
          Console.ReadLine();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }
  }
}
