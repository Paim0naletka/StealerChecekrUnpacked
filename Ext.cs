// Decompiled with JetBrains decompiler
// Type: stealerchecker.Ext
// Assembly: stealerchecker, Version=9.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EACD1CA4-3D73-4169-BD60-7A660205DCFD
// Assembly location: C:\Users\fox89\Desktop\StealerChecker\stealerchecker.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading;

namespace stealerchecker
{
  internal static class Ext
  {
    public static void Print(this Program.Menu menu)
    {
      Colorful.Console.Clear();
      Colorful.Console.WriteLine(menu.Name, Color.Pink);
      Colorful.Console.WriteLine();
      foreach (KeyValuePair<string, Action> keyValuePair in menu.menu)
        Colorful.Console.WriteLine(string.Format("{0}. {1}", (object) (menu.menu.IndexOf(keyValuePair) + 1), (object) keyValuePair.Key), Color.LightCyan);
      Colorful.Console.WriteLine();
      Colorful.Console.WriteLine("55. back <--", Color.Cyan);
      int num = 0;
      try
      {
        num = int.Parse(Colorful.Console.ReadLine());
      }
      catch
      {
        menu.Print();
      }
      Colorful.Console.WriteLine();
      if (num == 55)
      {
        Colorful.Console.Clear();
        Program.PrintMainMenu();
      }
      if (num > menu.menu.Count || num < 1)
        menu.Print();
      try
      {
        menu.menu[num - 1].Value();
      }
      catch
      {
        menu.Print();
      }
      Program.PrintMainMenu();
    }

    internal static void WriteLine(this StringBuilder builder, string value) => builder.Append(value).Append(Program.NewLine);

    public static string Input(string text, Color color)
    {
      Colorful.Console.WriteLine(text, color);
      return Colorful.Console.ReadLine();
    }

    public static bool AnyNew(this string a, string b) => CultureInfo.CurrentCulture.CompareInfo.IndexOf(a, b, 0, a.Length, CompareOptions.IgnoreCase) >= 0;

    public static void Wait(this Thread th)
    {
      do
        ;
      while (th.IsAlive);
    }

    public static string Join(this IEnumerable<string> ts) => string.Join(Program.NewLine, ts);

    public static string Join(this IEnumerable<string> ts, string del) => string.Join(del, ts);

    public static string Join(this string[] ts) => string.Join(Program.NewLine, ts);

    public static string Join(this string[] ts, string del) => string.Join(del, ts);
  }
}
