using Akka.Actor;
using Akka.Routing;
using Akka.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KT_Core.Actors
{
  public class Node : ReceiveActor
  {
    private readonly short _totalRows = 3;
    private readonly uint _totalColumns;
    private readonly uint _totalArea;

    public Node(uint totalColumns)
    {

      _totalColumns = totalColumns;
      _totalArea = (uint)(_totalRows * _totalColumns);

    //  Receive<CalculateNextMove>(h =>
    //  {
    //    var incrementedHits = h.Hits + 1;
    //    if (incrementedHits < _totalArea)
    //    {
    //      var q = new Queue<string>(h.PastCoordinates);
    //      if (q.Count > 666667)
    //        q.Dequeue();
    //
    //      q.Enqueue($"{h.PossibleRowNumber}-{h.PossibleColumnNumber}");
    //
    //      Console.WriteLine(string.Join(", ", q));
    //
    //      Context.Parent.Tell(new InitNextMove(q, h.PossibleRowNumber, h.PossibleColumnNumber, incrementedHits));
    //    }
    //  });

      Receive<InitNextMove>(h =>
      {
        var t = new TimeSpan(0,0,5);
        short possibleRowMatch = 0;
        uint possibleColumnMatch = 0;

        var instructionCollection = new List<Instruction>();

        if (h.CurrentRow > 1 && h.CurrentColumn > 1)
        {
          // send message up left
          possibleRowMatch = (short)(h.CurrentRow - 1);
          possibleColumnMatch = h.CurrentColumn - 1;

          //instructionCollection.Add(new Instruction(possibleRowMatch, possibleColumnMatch));

          //Context.Parent.Tell(new NextMoveAllowed(h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.History, h.Hits));
          Context.System.Scheduler.ScheduleTellOnce(t, Context.ActorSelection("akka://Validator@localhost:5395/user/Executor"), new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns), Self);


         // Context.ActorSelection("akka.tcp://Validator@localhost:5395/user/Executor").Tell(new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns));

        }

        if (h.CurrentRow > 1)
        {
          // send message up
          possibleRowMatch = (short)(h.CurrentRow - 1);
          possibleColumnMatch = h.CurrentColumn;
          //instructionCollection.Add(new Instruction(possibleRowMatch, possibleColumnMatch));

          //Context.ActorSelection("akka.tcp://Validator@localhost:5495/user/Executor").Tell(new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns));

          Context.System.Scheduler.ScheduleTellOnce(t, Context.ActorSelection("akka.tcp://Validator@localhost:5495/user/Executor"), new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns), Self);

          // send message up right
          possibleRowMatch = (short)(h.CurrentRow - 1);
          possibleColumnMatch = h.CurrentColumn + 1;
          // instructionCollection.Add(new Instruction(possibleRowMatch, possibleColumnMatch));

          Context.System.Scheduler.ScheduleTellOnce(t, Context.ActorSelection("akka.tcp://Validator@localhost:5395/user/Executor"), new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns), Self);

         // Context.ActorSelection("akka.tcp://Validator@localhost:5395/user/Executor").Tell(new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns));
        }

        if (h.CurrentColumn > 1)
        {
          // send message left
          possibleRowMatch = h.CurrentRow;
          possibleColumnMatch = h.CurrentColumn - 1;
          // instructionCollection.Add(new Instruction(possibleRowMatch, possibleColumnMatch));

          Context.System.Scheduler.ScheduleTellOnce(t, Context.ActorSelection("akka.tcp://Validator@localhost:5395/user/Executor"), new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns), Self);

         // Context.ActorSelection("akka.tcp://Validator@localhost:5495/user/Executor").Tell(new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns));

        }

        if (h.CurrentColumn < totalColumns)
        {
          // send message right
          possibleRowMatch = h.CurrentRow;
          possibleColumnMatch = h.CurrentColumn + 1;

          // instructionCollection.Add(new Instruction(possibleRowMatch, possibleColumnMatch));
          //Context.ActorSelection("akka.tcp://Validator@localhost:5395/user/Executor").Tell(new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns));

          Context.System.Scheduler.ScheduleTellOnce(t, Context.ActorSelection("akka.tcp://Validator@localhost:5495/user/Executor"), new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns), Self);

        }

        if (h.CurrentRow < _totalRows)
        {
          if (h.CurrentColumn > 1)
          {
            // send message bottom left
            possibleRowMatch = (short)(h.CurrentRow + 1);
            possibleColumnMatch = h.CurrentColumn - 1;
            //instructionCollection.Add(new Instruction(possibleRowMatch, possibleColumnMatch));
            //Context.ActorSelection("akka.tcp://Validator@localhost:5495/user/Executor").Tell(new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns));
            Context.System.Scheduler.ScheduleTellOnce(t, Context.ActorSelection("akka.tcp://Validator@localhost:5395/user/Executor"), new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns), Self);
          }

          // send bottom
          possibleRowMatch = (short)(h.CurrentRow + 1);
          possibleColumnMatch = h.CurrentColumn;
          //instructionCollection.Add(new Instruction(possibleRowMatch, possibleColumnMatch));
          //  Context.ActorSelection("akka.tcp://Validator@localhost:5395/user/Executor").Tell(new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns));
          Context.System.Scheduler.ScheduleTellOnce(t, Context.ActorSelection("akka.tcp://Validator@localhost:5495/user/Executor"), new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns), Self);

          if (h.CurrentColumn < _totalColumns)
          {
            // send message bottom right
            possibleRowMatch = (short)(h.CurrentRow + 1);
            possibleColumnMatch = h.CurrentColumn + 1;
            //instructionCollection.Add(new Instruction(possibleRowMatch, possibleColumnMatch));
          //  Context.ActorSelection("akka.tcp://Validator@localhost:5495/user/Executor").Tell(new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns));

            Context.System.Scheduler.ScheduleTellOnce(t, Context.ActorSelection("akka.tcp://Validator@localhost:5495/user/Executor"), new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns), Self);

          }

          //Console.WriteLine(string.Join(", ", instructionCollection.Select(e => string.Format($"{e.PossibleRow }-{e.PossibleColumn }"))));
          //Console.WriteLine(string.Join(", ", h.History));

          //Context.ActorSelection("akka.tcp://Validator@localhost:5295/user/Executor")
          //.Tell(new NextMoveAllowed(h.History, instructionCollection, h.CurrentRow, h.CurrentColumn,h.Hits, _totalColumns));
        }

      });
    }

    #region Construct Node

    public static Props Props(uint totalColumns)
    {
      return Akka.Actor.Props.Create(() => new Node(totalColumns)).WithRouter(FromConfig.Instance);
    }

    #endregion Construct Node
  }
}
