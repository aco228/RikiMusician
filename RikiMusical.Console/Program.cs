using BlottoBeats.Library.SongData;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Globalization;
using Melanchall.DryWetMidi.Smf;
using Melanchall.DryWetMidi.Devices;
using WMPLib;
using System.Speech.AudioFormat;

namespace RikiMusical.ConsoleApp
{
  class Program
  {
    public static string ROOT_PATH = @"E:\RikiMusician\";
    public static string DATA_PATH = ROOT_PATH + @"_data\";
    static private MediaPlayer.MediaPlayer player;
    static public List<SongParameters> backlog;
    static public Generator generator;

    static void Main(string[] args)
    {
      Console.Title = "Riki";
      Console.OutputEncoding = Encoding.UTF8;
      player = new MediaPlayer.MediaPlayer();
      backlog = new List<SongParameters>();
      generator = new Generator();
      GenerateJazz();
      
      string text = ComposeText();
      Syntx(text);
      
      
      //Speak(text);

      //new Thread(() => { Start(); }).Start();
      StartPlaying();
      Console.ReadKey();
    }

    private static void Syntx(string text)
    {
      //RikiSyntx.GetSound(text);
      //return;

      using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
      using (MemoryStream streamAudio = new MemoryStream())
      {
        var culture = CultureInfo.GetCultureInfo("rs-RS");
        var voices = synthesizer.GetInstalledVoices();
        //var a = SpeechSynthesizer.AllVoices;

        synthesizer.SelectVoice("Microsoft Matej");

        synthesizer.Volume = 100;  // 0...100
        synthesizer.Rate = -3;     // -10...10
                                   //synthesizer.Speak("Hello World");
                                   //synthesizer.SpeakAsync(text);
        //synthesizer.SetOutputToWaveFile("speak1.wav", new SpeechAudioFormatInfo(96000, AudioBitsPerSample.Sixteen, AudioChannel.Stereo));
        
        //synthesizer.SetOutputToWaveStream(streamAudio);
        synthesizer.SetOutputToWaveFile("speak.wav");
        synthesizer.Speak(text);

        //Normalize.Norm();
      }
    }

    private static string ComposeText()
    {
      return LyricsGenerator.Get();
    }
    

    private static void GenerateJazz()
    {
      Random rnd = new Random();
      //var songLen = generator.generate(new SongParameters(1144069031, 128, "Jazz"));

      //var songLen = generator.generate(new SongParameters(rnd.Next(1044069031, 1344069031), rnd.Next(120, 150), "Twelve-tone"));
      var songLen = generator.generate(new SongParameters(rnd.Next(1044069031, 1344069031), 89, "Twelve-tone"));
      player.Open("temp.mid");
      player.Stop();
      player.Volume = -900;
      //player.Play();
    }

    private static void StartPlaying()
    {
      

      new Thread(() =>
      {
        var player1 = new WMPLib.WindowsMediaPlayer();
        player1.URL = @"temp.mid";
        //player1.settings.volume = 50;
        player1.controls.play();
      }).Start();

      new Thread(() =>
      {
        Thread.Sleep(12000);
        var player2 = new WMPLib.WindowsMediaPlayer();
        player2.URL = @"speak.mp3";
        //player2.settings.volume = 100;
        player2.controls.play();
      }).Start();
      //PlayNASound("temp.mid");
    }

    static bool waiting = false;
    static AutoResetEvent stop = new AutoResetEvent(false);
    public static void PlayMp3FromUrl(string url)
    {
      using (Stream ms = new MemoryStream())
      {
        using (Stream stream = WebRequest.Create(url)
            .GetResponse().GetResponseStream())
        {
          byte[] buffer = new byte[32768];
          int read;
          while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
          {
            ms.Write(buffer, 0, read);
          }
        }

        ms.Position = 0;
        using (WaveStream blockAlignedStream = new BlockAlignReductionStream(WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(ms))))
        {
          using (WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
          {
            waveOut.Init(blockAlignedStream);
            waveOut.PlaybackStopped += (sender, e) =>
            {
              waveOut.Stop();
            };

            waveOut.Play();
            waiting = true;
            stop.WaitOne(10000);
            waiting = false;
          }
        }
      }
    }

    public static void PlayNASound(string url)
    {
      using (Stream ms = new MemoryStream())
      {
        using (Stream stream = File.Open(url, FileMode.Open))
        {
          byte[] buffer = new byte[32768];
          int read;
          while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
          {
            ms.Write(buffer, 0, read);
          }
        }

        ms.Position = 0;
        using (WaveStream blockAlignedStream = new BlockAlignReductionStream(WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(ms))))
        {
          using (WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
          {
            waveOut.Init(blockAlignedStream);
            waveOut.PlaybackStopped += (sender, e) =>
            {
              waveOut.Stop();
            };

            waveOut.Play();
            waiting = true;
            stop.WaitOne(10000);
            waiting = false;
          }
        }
      }
    }
  }
}

