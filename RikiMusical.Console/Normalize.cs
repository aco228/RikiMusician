using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RikiMusical.ConsoleApp
{
  public class Normalize
  {

    public static void Norm()
    {
      var inPath = Program.ROOT_PATH + @"RikiMusical.Console\bin\Debug\speak1.wav";
      var outPath = Program.ROOT_PATH + @"RikiMusical.Console\bin\Debug\speak.wav";
      float max = 0;

      using (var reader = new AudioFileReader(inPath))
      {
        // find the max peak
        float[] buffer = new float[reader.WaveFormat.SampleRate];
        int read;
        do
        {
          read = reader.Read(buffer, 0, buffer.Length);
          for (int n = 0; n < read; n++)
          {
            var abs = Math.Abs(buffer[n]);
            if (abs > max) max = abs;
          }
        } while (read > 0);
        Console.WriteLine($"Max sample value: {max}");

        if (max == 0 || max > 1.0f)
          throw new InvalidOperationException("File cannot be normalized");

        // rewind and amplify
        reader.Position = 0;
        reader.Volume = 1.0f / max;

        // write out to a new WAV file
        WaveFileWriter.CreateWaveFile16(outPath, reader);
      }
    }
  }
}
