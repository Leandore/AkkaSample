using System;
using System.Collections.Generic;
using System.Text;

namespace KT_Core
{
  public class InitNextMove
  {
    public Queue<string> History { get; }
    public short CurrentRow { get; }
    public uint CurrentColumn { get; }
    public uint Hits { get; }
    public InitNextMove(Queue<string> history, short currentRow, uint currentColumn, uint hits)
    {
      History = history;
      CurrentRow = currentRow;
      CurrentColumn = currentColumn;
      Hits = hits;
    }
  }
}
