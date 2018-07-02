using System;
using System.Collections.Generic;
using System.Text;

namespace KT_Core
{
  public class ValidateNextMove
  {
    public short TotalRow { get; }
    public uint TotalColumn { get; }
    public short CurrentRow { get; }
    public uint CurrentColumn { get; }
    public short RowNumber { get; }
    public uint ColumnNumber { get; }
    public uint Hits { get; }
    public Queue<string> History { get; }

    public ValidateNextMove(short totalRows, uint totalColumns, short currentRow, uint currentColumn, short rowNumber, uint columnNumber, Queue<string> hist, uint hits)
    {
      TotalRow = totalRows;
      TotalColumn = totalColumns;

      CurrentRow = currentRow;
      CurrentColumn = currentColumn;
      RowNumber = rowNumber;
      ColumnNumber = columnNumber;
      History = hist;
      Hits = hits;
    }
  }
}
