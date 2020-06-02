using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RikiMusical.Lyrics
{
  class Program
  {
    static void Main(string[] args)
    {
      string result = GetLyrics();

      Console.WriteLine();
      Console.WriteLine();
      Console.WriteLine();
      Console.WriteLine();

      Console.OutputEncoding = Encoding.UTF8;
      Console.WriteLine(result);

      Console.ReadKey();

    }

    private static List<string> Words = new List<string>();

    public static string GetLyrics()
    {
      GetWords();
      Random rnd = new Random();
      string result = "";
      int NUMBER_OF_VERSES = 8;
      List<int> indexes = new List<int>();

      for(int i = 0; i < NUMBER_OF_VERSES; i++)
      {
        int index = -1;
        for(; ;)
        {
          if (index != -1 && !indexes.Contains(index))
            break;
          index = rnd.Next(0, Words.Count);
        }

        string start = Words.ElementAt(index).Trim();
        string rymn = GetRymn(start);
        string lastWord = GetLastWord(start);

        for(int j = 0; j < Words.Count; j++)
          if(!indexes.Contains(j) && j != index && GetRymn(Words.ElementAt(j)).Equals(rymn) && lastWord != GetLastWord(Words.ElementAt(j)))
          {
            string lw = GetLastWord(Words.ElementAt(j));
            result += start
              + Environment.NewLine
              + Words.ElementAt(j)
              + Environment.NewLine;
            indexes.Add(j);
            Console.WriteLine("Rym found");
            break;
          }

        Console.WriteLine("Rym not found");
      }

      return result;
    }

    private static string GetRymn(string start)
    {
      string rymn = "";
      for (int j = start.Length - 1; j >= 0; j--)
      {
        if (rymn.Length == 3)
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

    public static List<string> GetWords()
    {
      string[] lines = File.ReadAllLines(@"E:\Aco\BlottoBeats\RikiMusical.Console\dosto.txt", Encoding.UTF8);
      
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
          if (s == '.' || s == '!' || s == '?')
          {
            words.Add(refw);
            refw = "";
          }
        }

        int MAX_WORDS = 6;
        foreach (string w in words)
        {
          bool max_word_flex_outOfRange = false;
          string[] split = w.Split(' ');
          if (split.Length < MAX_WORDS)
            continue;

          string refWord = "";

          int max_word_flex = MAX_WORDS;
          for (int i = 0; i < max_word_flex; i++)
          {
            if (string.IsNullOrEmpty(split[i]))
              continue;

            if (i == max_word_flex - 1 && split[i].Length <= 2)
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

          Words.Add(refWord);
        }

        Console.WriteLine($"{linec}/{lines.Length}");
      }

      return Words;
    }
  }
}
