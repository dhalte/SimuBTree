using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SimuBTree
{
  static class Helper
  {
    internal static void Assert(bool b)
    {
      if (!b)
      {
        Debugger.Break();
      }
    }

    private static List<HelperTraceListener> listeners = new List<HelperTraceListener>();
    internal static void Trace(string msg)
    {
      foreach (HelperTraceListener listener in listeners)
      {
        listener.Trace(msg);
      }
    }
    internal static void EnregistreListener(HelperTraceListener listener)
    {
      listeners.Add(listener);
    }
    public interface HelperTraceListener
    {
      void Trace(string msg);
    }
    public static string Dump(BTreeClass bTree)
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine($"Order={bTree.Order}, Deep={bTree.Deep}, NbKeys={bTree.NbKeys}");
      Dump(bTree.Root, sb, 0);
      return sb.ToString();
    }

    private static void Dump(BTreeNode node, StringBuilder sb, int profondeur)
    {
      string cartouche = new string(' ', 4 * profondeur);
      if (!node.Leaf)
      {
        Dump(node.Child(0), sb, profondeur + 1);
      }
      for (int i = 0; i < node.NbKeys; i++)
      {
        sb.AppendLine($"{cartouche}{node.Key(i)}");
        if (node.NbChildren > i + 1)
        {
          Dump(node.Child(i + 1), sb, profondeur + 1);
        }
      }
    }
  }

}
