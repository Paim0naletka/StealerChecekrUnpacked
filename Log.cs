// Decompiled with JetBrains decompiler
// Type: stealerchecker.Log
// Assembly: stealerchecker, Version=9.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EACD1CA4-3D73-4169-BD60-7A660205DCFD
// Assembly location: C:\Users\fox89\Desktop\StealerChecker\stealerchecker.exe

using System.IO;

namespace stealerchecker
{
  public struct Log
  {
    public string FullPath;
    public string Name;

    public Log(string fullPath)
    {
      this.FullPath = fullPath;
      this.Name = Path.GetFileName(fullPath);
    }
  }
}
