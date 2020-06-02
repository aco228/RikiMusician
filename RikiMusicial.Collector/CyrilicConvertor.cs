using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RikiMusicial.Collector
{
  
  public class CyrilicConvertor
  {
    public Dictionary<char, string> Values = new Dictionary<char, string>();
    public CyrilicConvertor()
    {
      Values.Add('а', "a");
      Values.Add('б', "b");
      Values.Add('в', "v");
      Values.Add('г', "g");
      Values.Add('д', "d");
      Values.Add('ђ', "đ");
      Values.Add('е', "e");
      Values.Add('ж', "ž");
      Values.Add('з', "z");
      Values.Add('и', "i");
      Values.Add('ј', "j");
      Values.Add('к', "k");
      Values.Add('л', "l");
      Values.Add('љ', "lj");
      Values.Add('м', "m");
      Values.Add('н', "n");
      Values.Add('њ', "nj");
      Values.Add('о', "o");
      Values.Add('п', "p");
      Values.Add('р', "r");
      Values.Add('с', "s");
      Values.Add('т', "t");
      Values.Add('ћ', "ć");
      Values.Add('у', "u");
      Values.Add('ф', "f");
      Values.Add('х', "x");
      Values.Add('ц', "c");
      Values.Add('ч', "č");
      Values.Add('џ', "dž");
      Values.Add('ш', "š");
    }

    public string Transform(string input)
    {
      string result = "";
      foreach (char c in input)
      {
        bool isUpperCase = char.IsUpper(c);
        if (Values.ContainsKey(char.ToLower(c)))
        {
          StringBuilder valueGet = new StringBuilder(Values[char.ToLower(c)]);
          if (isUpperCase)
            valueGet[0] = char.ToUpper(valueGet[0]);
          result += valueGet.ToString();
        }
        else
          result += c;
      }
      return result;
    }
  }
}
