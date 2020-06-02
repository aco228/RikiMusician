using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RikiMusical.ConsoleApp
{
  public class LyricsGenerator
  {

    public static string Get()
    {
      string result = GetLyrics();

      

      return result;
    }
    

    public static string GetLyrics()
    {
      Console.WriteLine("Generating..");

      List<string> ParafWords6 = GetWords(5);
      List<string> ParafWords4 = GetWords(4);
      List<string> ParafWords3 = GetWords(3);
      List<string> RefWords = GetWords(2);

      List<string> verses6 = GetVerses(ParafWords6, 3);
      List<string> verses4 = GetVerses(ParafWords4, 3);
      List<string> verses3 = GetVerses(ParafWords3, 4);
      List<string> refren = GetVerses(RefWords, 2);

      string result2 = ""
        + verses4[0] + Environment.NewLine
        + verses6[0] + Environment.NewLine
        
        + Environment.NewLine
        + verses3[0] + Environment.NewLine
        + verses3[1] + Environment.NewLine
        + Environment.NewLine
        
        + verses4[1] + Environment.NewLine
        + verses6[1] + Environment.NewLine

        + Environment.NewLine
        + verses3[2] + Environment.NewLine
        + refren[0] + Environment.NewLine;


      string title = verses3[0].Split('\n')[0].Trim();

      Console.WriteLine();
      Console.WriteLine();
      Console.WriteLine();

      Console.WriteLine();
      Console.WriteLine();
      Console.WriteLine();

      Console.WriteLine();
      Console.WriteLine();
      Console.WriteLine();

      Console.WriteLine(title);
      Console.WriteLine();

      Console.WriteLine(result2);
      Console.WriteLine();
      Console.WriteLine();

      File.WriteAllText(Program.DATA_PATH + @"poems\" + title.Replace("?", string.Empty) + ".txt", result2);
      return result2;
    }

    private static List<string> GetVerses(List<string> words, int number_of_verses)
    {
      Random rnd = new Random();
      List<string> lines = new List<string>();
      List<int> indexes = new List<int>();

      for (int i = 0; i <= number_of_verses; i++)
      {
        int index = -1;
        for (; ; )
        {
          if (index != -1 && !indexes.Contains(index))
            break;
          index = rnd.Next(0, words.Count);
        }

        string start = words.ElementAt(index).Trim();
        string lastWord = GetLastWord(start);
        string rymn = GetRymn(lastWord);
        bool found = false;

        for (int j = 0; j < words.Count; j++)
          if (!indexes.Contains(j) && j != index && GetRymn(GetLastWord(words.ElementAt(j))).Equals(rymn) && lastWord != GetLastWord(words.ElementAt(j)))
          {
            string lw = GetLastWord(words.ElementAt(j));
            lines.Add(start + Environment.NewLine + words.ElementAt(j));
            indexes.Add(j);
            Console.WriteLine("Rym found");
            found = true;
            break;
          }

        if (!found)
          i--;

        Console.WriteLine("Rym not found");
      }

      return lines;
    }

    private static string GetRymn(string start)
    {
      string rymn = "";
      int rymn_matrix = 4;

      if (start.Length <= 3)
        rymn_matrix = start.Length;
      if (start.Length > 3 && start.Length < 5)
        rymn_matrix = 3;
      if (start.Length >= 5 && start.Length <= 7)
        rymn_matrix = 4;
      if (start.Length >= 7)
        rymn_matrix = 5;

      for (int j = start.Length - 1; j >= 0; j--)
      {
        if (rymn.Length == rymn_matrix)
          break;
        if (char.IsLetter(start[j]))
          rymn = rymn + start[j];
      }
      return rymn;
    }

    private static string GetLastWord(string start)
    {
      string spliter = start.Trim().Split(' ').LastOrDefault().Trim().ToLower();
      string result = "";
      foreach (char c in spliter)
        if (char.IsLetter(c))
          result += c;
      return result;
    }

    public static List<string> GetWords(int max_words)
    {
      List<string> Words = new List<string>();
      if(File.Exists(Program.DATA_PATH + @"cache_words_" + max_words + ".txt"))
      {
        Words = File.ReadAllLines(Program.DATA_PATH + @"cache_words_" + max_words + ".txt", Encoding.UTF8).ToList();
        return Words;
      }

      string[] lines = File.ReadAllLines( Program.DATA_PATH + @"data.txt", Encoding.UTF8);

      for (int linec = 0; linec < lines.Length; linec++)
      {
        string line = lines[linec];
        if (string.IsNullOrEmpty(line))
          continue;

        List<string> words = new List<string>();
        string refw = string.Empty;
        foreach (char s in line)
        {
          refw += s;
          if (s == '.' || s == '!' || s == '?' || s == ',' || s == ';' || s == ':')
          {
            words.Add(refw);
            refw = "";
          }
        }

        int MAX_WORDS = max_words;
        foreach (string w in words)
        {
          bool max_word_flex_outOfRange = false;
          string[] split = w.Split(' ');
          if (split.Length <= MAX_WORDS)
            continue;

          string refWord = "";

          int max_word_flex = MAX_WORDS;
          for (int i = 0; i <= max_word_flex; i++)
          {
            if (string.IsNullOrEmpty(split[i]))
              continue;

            if (i >= max_word_flex - 1 && split[i].Length <= 3)
            {
              max_word_flex++;
              if (split.Length <= max_word_flex)
              {
                max_word_flex_outOfRange = true;
                break;
              }
              continue;
            }

            refWord += split[i] + " ";
          }

          if (max_word_flex_outOfRange)
            break;

          refWord = refWord.Trim();
          if(!Words.Contains(refWord))
            Words.Add(refWord);
        }

        Console.WriteLine($"{linec}/{lines.Length}");
      }

      string file = "";
      foreach (string word in Words)
        file += word.Trim() + Environment.NewLine;
      File.WriteAllText( Program.DATA_PATH + @"cache_words_" + max_words + ".txt", file);
      
      return Words;
    }
    



  }
}
