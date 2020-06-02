using NAudio.Lame;
using NAudio.Wave;
using SpeechLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RikiMusical.ConsoleApp
{
  public class RikiSyntx
  {
    public static byte[] GetSound(string text)
    {
      const SpeechVoiceSpeakFlags speechFlags = SpeechVoiceSpeakFlags.SVSFlagsAsync;
      var synth = new SpVoice();
      var wave = new SpMemoryStream();
      var voices = synth.GetVoices();
      try
      {
        // synth setup
        synth.Volume = 100;
        synth.Rate = -3;
        foreach (SpObjectToken voice in voices)
          if (voice.GetAttribute("Name") == "Microsoft Matej")
          {
            synth.Voice = voice;
            break;
          }

        wave.Format.Type = SpeechAudioFormatType.SAFT22kHz16BitMono;
        synth.AudioOutputStream = wave;
        synth.Speak(text, speechFlags);
        synth.WaitUntilDone(Timeout.Infinite);

        var waveFormat = new WaveFormat(22050, 16, 1);
        using (var ms = new MemoryStream((byte[])wave.GetData()))
        using (var reader = new RawSourceWaveStream(ms, waveFormat))
        using (var outStream = new MemoryStream())
        using (var writer = new WaveFileWriter(outStream, waveFormat))
        {
          reader.CopyTo(writer);
          //return o.Mp3 ? ConvertToMp3(outStream) : outStream.GetBuffer();

          //var bytes = ConvertToMp3(outStream);
          File.WriteAllBytes("speak.wav", outStream.GetBuffer());

          return null;
        }
      }
      finally
      {
        Marshal.ReleaseComObject(voices);
        Marshal.ReleaseComObject(wave);
        Marshal.ReleaseComObject(synth);
      }
    }

    internal static byte[] ConvertToMp3(Stream wave)
    {
      wave.Position = 0;
      using (var mp3 = new MemoryStream())
      using (var reader = new WaveFileReader(wave))
      using (var writer = new LameMP3FileWriter(mp3, reader.WaveFormat, 128))
      {
        reader.CopyTo(writer);
        mp3.Position = 0;
        return mp3.ToArray();
      }
    }
  }
}
