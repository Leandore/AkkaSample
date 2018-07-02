using System;
using Akka.Actor;
using System.Collections.Generic;
using Akka.Configuration;
using KT_Core;
using Akka.Routing;

namespace KT_Hierarchy
{
  public class FinalDestination : ReceiveActor
  {
    private long _totalHits;
    public FinalDestination()
    {
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

      return Akka.Actor.Props.Create(() => new FinalDestination());
    }

    #endregion Construct Node
  }

  public class HistoryManager : ReceiveActor
  {
    private IActorRef _finalDestination;

    public HistoryManager(IActorRef finalDestination)
    {
      _finalDestination = finalDestination;

      Receive<ExecuteTrim>(h =>
      {
        var totalRows = 3;
        var totalArea = h.TotalColumns * totalRows;
        if ((h.Hits + 1) == (totalArea) && h.PossibleRowNumber == totalRows && h.PossibleColumnNumber == 1)
        {
          Context.System.Scheduler.ScheduleTellOnce(new TimeSpan(0, 0, 1), _finalDestination, 1, Self);
          return;
        }

        var q = new Queue<string>(h.HistoryToTrim);
        if (q.Count > 666667)
          q.Dequeue();

        if (!q.Contains($"{h.PossibleRowNumber}-{h.PossibleColumnNumber}"))
        {
          q.Enqueue($"{h.PossibleRowNumber}-{h.PossibleColumnNumber}");

          Console.WriteLine(string.Join(",", h.HistoryToTrim));
          Context.ActorSelection("akka://KingsRouteSystem/user/Initiator").Tell(new InitNextMove(q, h.PossibleRowNumber, h.PossibleColumnNumber, h.Hits + 1));
        }

      });
    }

    #region Construct Node

    public static Props Props(IActorRef finalDestination)
    {
      return Akka.Actor.Props.Create(() => new HistoryManager(finalDestination)).WithRouter(FromConfig.Instance);
    }

    #endregion Construct Node
  }

  public class HistoryValidator : ReceiveActor
  {
    private IActorRef _historyManager;

    public HistoryValidator(IActorRef historyManager, uint totalColumns)
    {
      _historyManager = historyManager;

      Receive<NextMoveAllowed>(h =>
      {
        short totalRows = 3;
        var totalArea = (uint)(totalRows * h.TotalColumns);
        if (IsValidMove(h.CurrentRow, h.CurrentColumn, h.PossibleRow, h.PossibleColumn, totalRows, h.Hits, h.TotalColumns, totalArea))
        {
          _historyManager.Tell(new ExecuteTrim(h.CurrentRow, h.CurrentColumn, h.PossibleRow, h.PossibleColumn, h.PreviousHitsCoordinates, h.Hits, h.TotalColumns));
        }
      });
    }

    #region Construct Node

    public static Props Props(IActorRef historyManager, uint totalColumns)
    {
      return Akka.Actor.Props.Create(() => new HistoryValidator(historyManager, totalColumns)).WithRouter(FromConfig.Instance);
    }

    #endregion Construct Node

    #region Validation

    private bool IsValidMove(short currentRow, uint currentColumn, short rowNumber, uint columnNumber, short totalRows, uint hits, uint totalColumns, uint totalArea)
    {
      var isValid = false;

      #region Check crash boundaries

      isValid = rowNumber >= 1
        && columnNumber >= 1
        && columnNumber <= totalColumns
        && rowNumber <= totalRows;

      #endregion Check crash boundaries

      if (!isValid)
        return false;

      #region Dont go back to start

      if (isValid)
        isValid = !(rowNumber == 1 && columnNumber == 1);

      #endregion Dont go back to start

      if (!isValid)
        return false;

      if (isValid && rowNumber == totalRows && columnNumber == 1)
      {
        isValid = hits == totalArea - 1;
      }

      if (!isValid)
        return false;

      #region Neighbours check
      // todo here , neighbours check must be neighbour of last item in the state list
      if (isValid)
      {
        if (currentRow == rowNumber)
        {
          isValid = (currentColumn == (columnNumber + 1)) || (currentColumn == (columnNumber - 1));
        }
        if (currentRow == (rowNumber - 1) || currentRow == (rowNumber + 1))
        {
          isValid = (currentColumn == (columnNumber)) || (currentColumn == (columnNumber + 1)) || (currentColumn == (columnNumber - 1));
        }
        if (currentColumn == columnNumber)
        {
          isValid = (currentRow == (rowNumber + 1)) || (currentRow == (rowNumber - 1));
        }
        if (currentColumn == (columnNumber - 1) || currentColumn == (columnNumber + 1))
        {
          isValid = (currentRow == (rowNumber)) || (currentRow == (rowNumber + 1)) || (currentRow == (rowNumber - 1));
        }
      }
      #endregion Neighbours check

      // if (!isValid)
      //   return false;

      #region Dont go back in time

      // if (isValid)
      // {
      //   isValid = (hist.Contains($"{rowNumber}-{columnNumber}") == false);
      // }

      #endregion Dont go back in time

      return isValid;
    }

    #endregion Validation
  }

  public class Initiator : ReceiveActor
  {
    private IActorRef _historyValidator;

    private readonly short _totalRows = 3;
    private readonly uint _totalColumns;
    private readonly uint _totalArea;

    public Initiator(IActorRef historyValidator, uint totalColumns)
    {
      _historyValidator = historyValidator;
      _totalColumns = totalColumns;
      _totalArea = (uint)(_totalRows * _totalColumns);

      Receive<InitNextMove>(h =>
      {
        var t = new TimeSpan(0, 0, 1);

        short possibleRowMatch = 0;
        uint possibleColumnMatch = 0;

        if (h.CurrentRow > 1 && h.CurrentColumn > 1)
        {
          // send message up left
          possibleRowMatch = (short)(h.CurrentRow - 1);
          possibleColumnMatch = h.CurrentColumn - 1;

          Context.System.Scheduler.ScheduleTellOnce(t, _historyValidator,
            new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns), Self);
        }

        if (h.CurrentRow > 1)
        {
          // send message up
          possibleRowMatch = (short)(h.CurrentRow - 1);
          possibleColumnMatch = h.CurrentColumn;

          Context.System.Scheduler.ScheduleTellOnce(t, _historyValidator,
            new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns), Self);

          // send message up right
          possibleRowMatch = (short)(h.CurrentRow - 1);
          possibleColumnMatch = h.CurrentColumn + 1;

          Context.System.Scheduler.ScheduleTellOnce(t, _historyValidator,
            new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns), Self);

        }

        if (h.CurrentColumn > 1)
        {
          // send message left
          possibleRowMatch = h.CurrentRow;
          possibleColumnMatch = h.CurrentColumn - 1;

          Context.System.Scheduler.ScheduleTellOnce(t, _historyValidator,
            new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns), Self);
        }

        if (h.CurrentColumn < totalColumns)
        {
          // send message right
          possibleRowMatch = h.CurrentRow;
          possibleColumnMatch = h.CurrentColumn + 1;

          Context.System.Scheduler.ScheduleTellOnce(t, _historyValidator,
            new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns), Self);
        }

        if (h.CurrentRow < _totalRows)
        {
          if (h.CurrentColumn > 1)
          {
            // send message bottom left
            possibleRowMatch = (short)(h.CurrentRow + 1);
            possibleColumnMatch = h.CurrentColumn - 1;

            Context.System.Scheduler.ScheduleTellOnce(t, _historyValidator,
              new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns), Self);
          }

          // send bottom
          possibleRowMatch = (short)(h.CurrentRow + 1);
          possibleColumnMatch = h.CurrentColumn;

          Context.System.Scheduler.ScheduleTellOnce(t, _historyValidator,
            new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns), Self);

          if (h.CurrentColumn < _totalColumns)
          {
            // send message bottom right
            possibleRowMatch = (short)(h.CurrentRow + 1);
            possibleColumnMatch = h.CurrentColumn + 1;

            Context.System.Scheduler.ScheduleTellOnce(t, _historyValidator,
              new NextMoveAllowed(h.History, h.CurrentRow, h.CurrentColumn, possibleRowMatch, possibleColumnMatch, h.Hits, _totalColumns), Self);

          }
        }

      });
    }

    private void Handle(string msg)
    {
      Console.WriteLine(msg);
    }

    #region Construct Node

    public static Props Props(IActorRef historyValidator, uint totalColumns)
    {
      return Akka.Actor.Props.Create(() => new Initiator(historyValidator, totalColumns)).WithRouter(FromConfig.Instance);
    }

    #endregion Construct Node
  }

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

      #region Config

      var config = ConfigurationFactory.ParseString(@"
      akka
      {
        actor
        {

          deployment {
              /Initiator {
                  router = round-robin-pool
                  resizer {
                      enabled = on
                      lower-bound = 100
                      upper-bound = 5000
                  }
              }

              /HistoryValidator {
                  router = round-robin-pool
                  resizer {
                      enabled = on
                      lower-bound = 100
                      upper-bound = 5000
                  }
              }

              /HistoryManager {
                  router = round-robin-pool
                  resizer {
                      enabled = on
                      lower-bound = 100
                      upper-bound = 5000
                  }
              }

          }
        }
      }
");

      #endregion Config


      var myActorSystem = ActorSystem.Create("KingsRouteSystem", config);

      var finalDestination = myActorSystem.ActorOf(FinalDestination.Props(), $"FinalDestination");
      var historyManager = myActorSystem.ActorOf(HistoryManager.Props(finalDestination), $"HistoryManager");
      var historyValidator = myActorSystem.ActorOf(HistoryValidator.Props(historyManager, columnTotals), $"HistoryValidator");
      var initiator = myActorSystem.ActorOf(Initiator.Props(historyValidator, columnTotals), $"Initiator");

      Console.WriteLine("Initiate which version?");

      var input = Console.ReadLine();

      var q = new Queue<string>();
      switch (input)
      {
        case "1":
          {
            Console.Title = "Running V1";
            q = new Queue<string>();
            q.Enqueue("1-1");
            q.Enqueue("1-2");
            initiator.Tell(new InitNextMove(q, 1, 2, 2));
          }
          break;

        case "2":
          {
            Console.Title = "Running V2";
            q = new Queue<string>();
            q.Enqueue("1-1");
            q.Enqueue("2-1");
            initiator.Tell(new InitNextMove(q, 2, 1, 2));
          }
          break;

        case "3":
          {
            Console.Title = "Running V3";
            q = new Queue<string>();
            q.Enqueue("1-1");
            q.Enqueue("2-2");
            initiator.Tell(new InitNextMove(q, 2, 2, 2));
          }
          break;
      }

    


      Console.ReadLine();
    }
  }
}
