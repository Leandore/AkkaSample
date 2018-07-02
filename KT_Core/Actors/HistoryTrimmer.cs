using Akka.Actor;
using Akka.Routing;
using System;
using System.Collections.Generic;

namespace KT_Core.Actors
{
  public class HistoryTrimmer : ReceiveActor
  {
    public HistoryTrimmer()
    {
      Receive<ExecuteTrim>(h =>
      {
        var totalRows = 3;
        var totalArea = h.TotalColumns * totalRows;
        if ((h.Hits + 1) == (totalArea) && h.PossibleRowNumber == totalRows && h.PossibleColumnNumber == 1)
        {
          var t = new TimeSpan(0, 0, 0, (int)h.Hits);
          Context.System.Scheduler.ScheduleTellOnce(t, Context
            .ActorSelection("akka.tcp://DestinationReporterSystem@localhost:5300/user/DestinationReporter"), 1, Self);
          return;
        }


        var q = new Queue<string>(h.HistoryToTrim);
        if (q.Count > 666667)
          q.Dequeue();

        if (!q.Contains($"{h.PossibleRowNumber}-{h.PossibleColumnNumber}"))
        {
          q.Enqueue($"{h.PossibleRowNumber}-{h.PossibleColumnNumber}");

          var t = new TimeSpan(0, 0, 0, (int)h.Hits);
          Context.System.Scheduler.ScheduleTellOnce(t,

          Context
          .ActorSelection("akka.tcp://KingsRouteSystem@localhost:5290/user/Initiator"), 
          new InitNextMove(q, h.PossibleRowNumber, h.PossibleColumnNumber, h.Hits + 1), Self);
        }

      });
    }

    #region Construct Node

    public static Props Props()
    {
      return Akka.Actor.Props.Create(() => new HistoryTrimmer()).WithRouter(FromConfig.Instance);
    }

    #endregion Construct Node
  }
}