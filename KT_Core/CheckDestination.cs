using System;
using System.Collections.Generic;
using System.Text;

namespace KT_Core
{
  public class CheckDestination
  {
    public uint Hits { get; }
    public Queue<string> History { get; }
    public short RowNumber { get; }
    public uint ColumnNumber { get; }

    public CheckDestination(uint hits, Queue<string> hist,
      short rowNumber, uint columnNumber)
    {
      Hits = hits;
      History = hist;
      RowNumber = rowNumber;
      ColumnNumber = columnNumber;
    }
  }
}
