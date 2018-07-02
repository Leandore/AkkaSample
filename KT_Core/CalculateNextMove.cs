using System.Collections.Generic;

namespace KT_Core
{
  public class CalculateNextMove
  {
    public short CurrentRow { get; }
    public uint CurrentColumn { get; }
    public short PossibleRowNumber { get; }
    public uint PossibleColumnNumber { get; }
    public uint Hits { get; }
    public Queue<string> PastCoordinates { get; }

    public CalculateNextMove(short currentRow, uint currentColumn,
      short possibleRowNumber, uint possibleColumnNumber, Queue<string> pastCoordinates, uint hits)
    {
      CurrentRow = currentRow;
      CurrentColumn = currentColumn;
      PossibleRowNumber = possibleRowNumber;
      PossibleColumnNumber = possibleColumnNumber;
      PastCoordinates = pastCoordinates;
      Hits = hits;
    }
  }
}
