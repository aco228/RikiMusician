using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VersOne.Epub;

namespace RikiMusicial.Collector
{
  class Program
  {
    private static string ROOT = @"C:\Users\ako\Documents\MEGAsync Downloads";
    private static string CONTENT = ROOT + @"\data.txt";
    private static CyrilicConvertor CyrilicCharConvertor = new CyrilicConvertor();
    private static List<string> FinishedBooks = new List<string>();

    static void Main(string[] args)
    {
      Console.Title = "Riki";
      Console.OutputEncoding = Encoding.UTF8;
      DirectoryInfo directory = new DirectoryInfo(ROOT);

      FinishedBooks = File.ReadAllLines(ROOT + @"\finished.txt").ToList();

      //for (int i = 2; i <= 8; i++)
      //  File.WriteAllText(string.Format(@"C:\Users\ako\Documents\MEGAsync Downloads\__p.{0}.txt", i), "");

      GetDataAsync(GetFiles(directory));
      Console.ReadKey();

    }

    public static List<FileInfo> GetFiles(DirectoryInfo di)
    {
      List<FileInfo> files = di.GetFiles().ToList();
      foreach (DirectoryInfo dd in di.GetDirectories())
        files.AddRange(GetFiles(dd));
      return files;
    }

    public static void GetDataAsync(List<FileInfo> files)
    {
      List<string> lines = new List<string>();
      foreach (FileInfo fi in files)
      {
        if (!fi.Extension.ToLower().Equals(".epub"))
          continue;
        if (FinishedBooks.Contains(fi.Name))
          continue;

        try
        {
          EpubBook epubBook = EpubReader.ReadBook(fi.FullName);
          string title = epubBook.Title;
          string author = epubBook.Author;
          List<string> authors = epubBook.AuthorList;

          EpubContent bookContent = epubBook.Content;
          Dictionary<string, EpubTextContentFile> htmlFiles = bookContent.Html;
          foreach (EpubTextContentFile htmlFile in htmlFiles.Values)
          {
            MatchCollection mc = new Regex(">(.*?)<").Matches(htmlFile.Content);
            foreach (Match m in mc)
            {
              if (m.Groups.Count != 2)
                continue;
              string value = m.Groups[1].Value.Replace("&nbsp;", string.Empty).Trim();
              string[] vsplit = value.Split(' ');
              if (string.IsNullOrEmpty(value) || value.Length <= 3
                || lines.Contains(value) || vsplit.Length < 2
                || char.IsLower(value[0]))
                continue;

              if (!value.Contains('.'))
                if (!value.Contains('?'))
                  if (!value.Contains('!'))
                    continue;

              if (!char.IsLetter(value[0]))
                if (value[0] != '-')
                  continue;

              lines.Add(Program.CyrilicCharConvertor.Transform(value));
            }
          }

          string Content = "";
          foreach (string value in lines)
            Content += ' ' + value;
          AnylizeText(Content);
          Console.WriteLine($"[SUCC] - {title}");
        }
        catch (Exception e)
        {
          Console.WriteLine($"[ERR] - {fi.Name}");
        }
        finally
        {
          FinishedBooks.Add(fi.Name);
          File.AppendAllText(ROOT + @"\finished.txt", fi.Name + Environment.NewLine);
        }

      }
    }

    private static void AnylizeText(string input)
    {
      List<string> phazes = new List<string>();
      MatchCollection mc = new Regex(@"\((.+?)\)").Matches(input);
      foreach (Match m in mc)
        input.Replace(m.Value, string.Empty);


      string phaze = "";
      for (int i = 0; i < input.Length; i++)
      {
        if (input[i] == '.' || input[i] == '?' || input[i] == '!')
        {
          if (input[i] == '.' && (i + 3 < input.Length && input.Substring(i, 3).Equals("...")))
          {
            i += 3;
            continue;
          }

          phazes.Add(phaze.Trim() + input[i]);
          i++;
          phaze = "";
          continue;

        }
        phaze += input[i];
      }

      Dictionary<int, List<string>> words = new Dictionary<int, List<string>>();
      foreach (string p in phazes)
      {
        string[] split = p.Split(' ');
        if (split[0].LastOrDefault() == ':')
          continue;

        int wcount = 0;
        foreach (string word in split)
          if (word.Length >= 3)
            wcount++;

        if (wcount < 2 || wcount > 8)
          continue;

        bool containsUnproprieteChars = false;
        foreach (string word in split)
          if (word.Length >= 3
            && word.Contains("-"))
          {
            containsUnproprieteChars = true;
            break;
          }
        if (containsUnproprieteChars)
          continue;

        if (!words.ContainsKey(wcount))
          words.Add(wcount, new List<string>());
        string inputValue = p;
        if (inputValue.StartsWith("\" ") || inputValue.StartsWith(", "))
          inputValue = p.Substring(2);
        words[wcount].Add(inputValue);
      }

      foreach (KeyValuePair<int, List<string>> w in words)
      {
        string content = "";
        foreach (string s in w.Value) content += s + Environment.NewLine;
        File.AppendAllText(string.Format(@"C:\Users\ako\Documents\MEGAsync Downloads\__p.{0}.txt", w.Key), content);
      }
    }


  }
}
