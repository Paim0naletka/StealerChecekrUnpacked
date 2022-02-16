// Decompiled with JetBrains decompiler
// Type: stealerchecker.Models.ResponseModel
// Assembly: stealerchecker, Version=9.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EACD1CA4-3D73-4169-BD60-7A660205DCFD
// Assembly location: C:\Users\fox89\Desktop\StealerChecker\stealerchecker.exe

using Newtonsoft.Json;

namespace stealerchecker.Models
{
  public struct ResponseModel
  {
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("avatar")]
    public string Avatar { get; set; }

    [JsonProperty("discriminator")]
    public string Discriminator { get; set; }

    [JsonProperty("public_flags")]
    public long PublicFlags { get; set; }

    [JsonProperty("flags")]
    public long Flags { get; set; }

    [JsonProperty("banner")]
    public object Banner { get; set; }

    [JsonProperty("banner_color")]
    public object BannerColor { get; set; }

    [JsonProperty("accent_color")]
    public object AccentColor { get; set; }

    [JsonProperty("bio")]
    public string Bio { get; set; }

    [JsonProperty("locale")]
    public string Locale { get; set; }

    [JsonProperty("nsfw_allowed")]
    public bool NsfwAllowed { get; set; }

    [JsonProperty("mfa_enabled")]
    public bool MfaEnabled { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("verified")]
    public bool Verified { get; set; }

    [JsonProperty("phone")]
    public string Phone { get; set; }
  }
}
