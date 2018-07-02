using System;
using System.Collections.Generic;
using System.Text;

namespace KT_Core
{
  public class MakeNextMove
  {
    public uint Hits { get; }
    public Queue<string> History { get; }
    public short RowNumber { get; }
    public uint ColumnNumber { get; }

    public MakeNextMove(uint hits, Queue<string> hist, short rowNumber, uint columnNumber)
    {
      Hits = hits;
      History = hist;
      RowNumber = rowNumber;
      ColumnNumber = columnNumber;
    }
  }
}
