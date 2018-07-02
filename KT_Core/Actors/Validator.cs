using Akka.Actor;
using Akka.Routing;
using System;

namespace KT_Core.Actors
{
  public class Validator : ReceiveActor
  {
    public Validator(uint totalColumns)
    {
      Receive<NextMoveAllowed>(h =>
      {
        short totalRows = 3;
        var totalArea = (uint)(totalRows * h.TotalColumns);
        if (IsValidMove(h.CurrentRow, h.CurrentColumn, h.PossibleRow, h.PossibleColumn, totalRows, h.Hits, h.TotalColumns, totalArea))
        {
          var t = new TimeSpan(0, 0, 5);
          Context.System.Scheduler.ScheduleTellOnce(t, Context
          .ActorSelection("akka.tcp://HistoryTrimmerSystem@localhost:5296/user/Trimmer"), new ExecuteTrim(h.CurrentRow, h.CurrentColumn, h.PossibleRow, h.PossibleColumn, h.PreviousHitsCoordinates, h.Hits, h.TotalColumns), Self );
        }
      });
    }

    #region Construct Node

    public static Props Props(uint totalColumns)
    {
      return Akka.Actor.Props.Create(() => new Validator(totalColumns)).WithRouter(FromConfig.Instance);
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
}