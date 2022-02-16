// Decompiled with JetBrains decompiler
// Type: stealerchecker.Models.CheckResult
// Assembly: stealerchecker, Version=9.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EACD1CA4-3D73-4169-BD60-7A660205DCFD
// Assembly location: C:\Users\fox89\Desktop\StealerChecker\stealerchecker.exe

namespace stealerchecker.Models
{
  public struct CheckResult
  {
    public bool IsValid;
    public bool Payment;
    public string Token;

    public CheckResult(bool isValid, string token, bool isPayment = false)
    {
      this.IsValid = isValid;
      this.Payment = isPayment;
      this.Token = token;
    }
  }
}
