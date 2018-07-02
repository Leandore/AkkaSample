using System;
using System.Collections.Generic;
using System.Text;

namespace KT_Core
{


  public class ExecuteTrim
  {
    public short CurrentRow { get; }
    public uint CurrentColumn { get; }
    public short PossibleRowNumber { get; }
    public uint PossibleColumnNumber { get; }
    public uint Hits { get; }
    public uint TotalColumns { get; }
    public Queue<string> HistoryToTrim { get; }

    public ExecuteTrim(short currentRow, uint currentColumn,
      short possibleRowNumber, uint possibleColumnNumber, Queue<string> historyToTrim, uint hits, uint totalColumns)
    {
      CurrentRow = currentRow;
      CurrentColumn = currentColumn;
      PossibleRowNumber = possibleRowNumber;
      PossibleColumnNumber = possibleColumnNumber;
      HistoryToTrim = historyToTrim;
      Hits = hits;
      TotalColumns = totalColumns;
    }
  }
}
