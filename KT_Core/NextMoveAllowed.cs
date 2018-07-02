using System;
using System.Collections.Generic;

namespace KT_Core
{
  public class NextMoveAllowed
{
    public short CurrentRow { get; }
    public uint CurrentColumn { get; }
    public uint TotalColumns { get; }

    public uint Hits { get; }
    public Queue<string> PreviousHitsCoordinates { get; }
    public short PossibleRow { get; }
    public uint PossibleColumn { get; }
    //public List<Instruction> Instructions { get; }

    //  public NextMoveAllowed(Queue<string> previousHitsCoordinates, List<Instruction> instructions, short currentRow, uint currentColumn, uint hits, uint totalColumns)
    public NextMoveAllowed(Queue<string> previousHitsCoordinates,  short currentRow, uint currentColumn, short possibleRow, uint possibleColumn, uint hits, uint totalColumns)
    {
      //Instructions = instructions;
      CurrentRow = currentRow;
      CurrentColumn = currentColumn;

      PreviousHitsCoordinates = previousHitsCoordinates;
      Hits = hits;
      TotalColumns = totalColumns;
      PossibleRow = possibleRow;
      PossibleColumn = possibleColumn;
    }
  }

  public class Instruction
  {
    public short PossibleRow { get;  }
    public uint PossibleColumn { get; }

    public Instruction(short possibleRow, uint possibleColumn)
    {
      PossibleRow = possibleRow;
      PossibleColumn = possibleColumn;
    }
  }
}
