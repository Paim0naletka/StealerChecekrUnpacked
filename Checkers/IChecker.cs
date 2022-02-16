// Decompiled with JetBrains decompiler
// Type: stealerchecker.Checkers.IChecker
// Assembly: stealerchecker, Version=9.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EACD1CA4-3D73-4169-BD60-7A660205DCFD
// Assembly location: C:\Users\fox89\Desktop\StealerChecker\stealerchecker.exe

using Leaf.xNet;

namespace stealerchecker.Checkers
{
  public interface IChecker
  {
    string Service { get; }

    void ProcessLine(
      string line,
      string proxy,
      ProxyType type,
      out string result,
      out bool isValid);
  }
}
