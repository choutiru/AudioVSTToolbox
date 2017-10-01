﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

using NAudio;
using NAudio.Wave;

using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;

using CommonUtils;
using CommonUtils.Audio;
using CommonUtils.VST;

using DiffPlex;
using DiffPlex.Model;

namespace SynthAnalysisStudio
{
	static class Program
	{
		static string _version = "1.1.1";
		
		static void StartGUI() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		static void StartAudioOutput(string pluginPath, string waveFilePath) {
			try
			{
				var hostCmdStub = new HostCommandStub();
				VstPluginContext ctx = VstPluginContext.Create(pluginPath, hostCmdStub);
				
				// add custom data to the context
				ctx.Set("PluginPath", pluginPath);
				ctx.Set("HostCmdStub", hostCmdStub);
				
				// actually open the plugin itself
				ctx.PluginCommandStub.Open();
				
				var audioOut = new AudioOutput(
					new List<IVstPluginCommandStub>() {ctx.PluginCommandStub},
					waveFilePath);
				Thread.Sleep(100);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}

		static void StartVstHost(string pluginPath, string waveInputFilePath, string fxpFilePath, string waveOutputFilePath, bool doPlay) {

			VstHost host = VstHost.Instance;
			IVstHostCommandStub hostCmdStub = new HostCommandStub();
			host.OpenPlugin(pluginPath, hostCmdStub);
			host.InputWave = waveInputFilePath;
			// with iblock=1...Nblocks and blocksize = Fs * tblock. Fs = 44100 and
			// tblock = 0.15 makes blocksize = 6615.
			int sampleRate = 44100;
			int blockSize = (int) (sampleRate * 0.15f); //6615;
			int channels = 2;
			host.Init(blockSize, sampleRate, channels);
			System.Diagnostics.Debug.WriteLine(host.getPluginInfo());
			host.LoadFXP(fxpFilePath);

			if (doPlay) {
				var playback = new VstPlaybackNAudio(host);
				playback.Play();
				
				Console.WriteLine("Started Audio Playback");
				
				// make sure to play while the stream is playing
				while (playback.PlaybackDevice.PlaybackState == PlaybackState.Playing)
				{
					Thread.Sleep(100);
				}
				
				Console.WriteLine("Ending Audio Playback");
				playback.Stop();
				Console.WriteLine("Stopped Audio Playback");
				playback.Dispose();
			}
			
			if (waveOutputFilePath != "") {
				var fileWriter = new VstFileWriter(host);
				fileWriter.CreateWaveFile(waveOutputFilePath);
			}
		}
		
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			
			/*
			#region Convert Waves SSLChannel to UADSSLChannel
			TextWriter tw3 = new StreamWriter(@"uadsslchannel-output.txt");
			List<WavesSSLChannel> presetList = WavesPreset.ReadXps<WavesSSLChannel>(@"C:\Program Files (x86)\Waves\Plug-Ins\SSLChannel.bundle\Contents\Resources\XPst\1000");
			foreach(var wavesSSLChannel in presetList) {
				WavesSSLChannelToUADSSLChannelAdapter sslChannelAdapter = new WavesSSLChannelToUADSSLChannelAdapter(wavesSSLChannel);
				UADSSLChannel uadSSLChannel = sslChannelAdapter.DoConvert();
				uadSSLChannel.Write(uadSSLChannel.PresetName + ".fxp");
				tw3.WriteLine(uadSSLChannel);
				tw3.WriteLine();
				tw3.WriteLine("-------------------------------------------------------");
			}
			TextWriter tw1 = new StreamWriter(@"sslchannel-output.txt");

			//List<WavesSSLChannel> presetList2 = WavesPreset.ReadXps<WavesSSLChannel>(@"C:\Users\Public\Waves Audio\Plug-In Settings\SSLChannel Settings.xps");
			List<WavesSSLChannel> presetList2 = WavesPreset.ReadXps<WavesSSLChannel>(@"C:\Users\perivar.nerseth\Downloads\SSLChannel Settings.xps");
			foreach(var wavesSSLChannel2 in presetList2) {
				WavesSSLChannelToUADSSLChannelAdapter sslChannelAdapter2 = new WavesSSLChannelToUADSSLChannelAdapter(wavesSSLChannel2);
				UADSSLChannel uadSSLChannel2 = sslChannelAdapter2.DoConvert();
				uadSSLChannel2.Write(uadSSLChannel2.PresetName + ".fxp");
				tw1.WriteLine(wavesSSLChannel2);
				tw1.WriteLine();
				tw1.WriteLine("-------------------------------------------------------");
				
				tw3.WriteLine(uadSSLChannel2);
				tw3.WriteLine();
				tw3.WriteLine("-------------------------------------------------------");
			}
			tw1.Close();
			tw3.Close();
			#endregion
			return;
			 */
			
			/*
			#region Waves SSLChannel preset conversion
			WavesSSLChannel ssl = new WavesSSLChannel();
			TextWriter tw1 = new StreamWriter(@"sslchannel-output.txt");
			ssl.ReadXps(@"C:\Program Files (x86)\Waves\Plug-Ins\SSLChannel.bundle\Contents\Resources\XPst\1000", tw1);
			ssl.ReadXps(@"C:\Users\Public\Waves Audio\Plug-In Settings\SSLChannel Settings.xps", tw1);
			tw1.Close();
			#endregion

			#region Waves SSLChannel preset conversion
			WavesSSLChannel ssl = new WavesSSLChannel();
			TextWriter tw1 = new StreamWriter(@"sslchannel-output.txt");
			ssl.ReadXps(@"C:\Program Files (x86)\Waves\Plug-Ins\SSLChannel.bundle\Contents\Resources\XPst\1000", tw1);
			ssl.ReadXps(@"C:\Users\Public\Waves Audio\Plug-In Settings\SSLChannel Settings.xps", tw1);
			tw1.Close();
			#endregion

			#region Waves SSLComp preset conversion
			WavesSSLComp sslcomp = new WavesSSLComp();
			TextWriter tw2 = new StreamWriter(@"sslcomp-output.txt");
			sslcomp.ReadXps(@"C:\Program Files (x86)\Waves\Plug-Ins\SSLComp.bundle\Contents\Resources\XPst\1000", tw2);
			sslcomp.ReadXps(@"C:\Users\Public\Waves Audio\Plug-In Settings\SSLComp Settings.xps", tw2);
			tw2.Close();
			#endregion
			 */
			
			/*
			PresetConverter.Zebra2Preset.LFOSync lfoSync = PresetConverter.Zebra2Preset.LFOSync.SYNC_1_4;
			double ms = 0;
			int rate = 0;
			//for (int rate = 0; rate <= 200; rate += 1) {
			//	ms = PresetConverter.Zebra2Preset.LFOSyncAndValueToMilliseconds(lfoSync, rate);
			//	Console.Out.WriteLine("{0} SyncValue:{1} {2:0.000} ms", lfoSync, rate, ms);
			//}

			lfoSync = PresetConverter.Zebra2Preset.LFOSync.SYNC_1_4_trip;
			rate = 100;
			ms = PresetConverter.Zebra2Preset.LFOSyncAndValueToMilliseconds(lfoSync, rate);
			Console.Out.WriteLine("{0} SyncValue:{1} {2:0.000} ms", lfoSync, rate, ms);

			lfoSync = PresetConverter.Zebra2Preset.LFOSync.SYNC_1_4;
			rate = 114;
			ms = PresetConverter.Zebra2Preset.LFOSyncAndValueToMilliseconds(lfoSync, rate);
			Console.Out.WriteLine("{0} SyncValue:{1} {2:0.000} ms", lfoSync, rate, ms);

			lfoSync = PresetConverter.Zebra2Preset.LFOSync.SYNC_1_4_dot;
			rate = 100;
			ms = PresetConverter.Zebra2Preset.LFOSyncAndValueToMilliseconds(lfoSync, rate);
			Console.Out.WriteLine("{0} SyncValue:{1} {2:0.000} ms", lfoSync, rate, ms);

			lfoSync = PresetConverter.Zebra2Preset.LFOSync.SYNC_1_4;
			rate = 87;
			ms = PresetConverter.Zebra2Preset.LFOSyncAndValueToMilliseconds(lfoSync, rate);
			Console.Out.WriteLine("{0} SyncValue:{1} {2:0.000} ms", lfoSync, rate, ms);
			
			Console.ReadKey();
			return;
			
			// TEST THE ZEBRA LFO to MS conversion
			PresetConverter.Zebra2Preset.LFOSync lfoSync = PresetConverter.Zebra2Preset.LFOSync.SYNC_1_4;
			double ms = 0;
			for (int rate = 50; rate <= 200; rate += 50) {
				ms = PresetConverter.Zebra2Preset.LFOSyncAndValueToMilliseconds(lfoSync, rate);
				Console.Out.WriteLine("{0} SyncValue:{1} {2:0.000} ms", lfoSync, rate, ms);
			}

			lfoSync = PresetConverter.Zebra2Preset.LFOSync.SYNC_1_4_trip;
			for (int rate = 50; rate <= 200; rate += 50) {
				ms = PresetConverter.Zebra2Preset.LFOSyncAndValueToMilliseconds(lfoSync, rate);
				Console.Out.WriteLine("{0} SyncValue:{1} {2:0.000} ms", lfoSync, rate, ms);
			}

			lfoSync = PresetConverter.Zebra2Preset.LFOSync.SYNC_1_4_dot;
			for (int rate = 50; rate <= 200; rate += 50) {
				ms = PresetConverter.Zebra2Preset.LFOSyncAndValueToMilliseconds(lfoSync, rate);
				Console.Out.WriteLine("{0} SyncValue:{1} {2:0.000} ms", lfoSync, rate, ms);
			}

			Console.ReadKey();
			return;
			
			// TEST THE LFO ZEBRA 2 LFO CONVERSION
			PresetConverter.Zebra2Preset.LFOSync lfoSync = PresetConverter.Zebra2Preset.LFOSync.SYNC_0_1s;
			for (int i = 0; i <= 200; i += 2) {
				double ms = PresetConverter.Zebra2Preset.LFOSyncAndValueToMilliseconds(lfoSync, i);

				// test reverse conversion
				PresetConverter.Zebra2Preset.LFOSync lfoSync2 = PresetConverter.Zebra2Preset.LFOSync.SYNC_0_1s;
				int lfoValue = 0;
				PresetConverter.Zebra2Preset.MillisecondsToLFOSyncAndValue((float) ms, out lfoSync2, out lfoValue);
				double ms2 = PresetConverter.Zebra2Preset.LFOSyncAndValueToMilliseconds(lfoSync2, lfoValue);
				
				Console.Out.WriteLine("{0} SyncValue:{1} {2:0.000} ms (Reverse: {3} {4} {5:0.000} ms)", lfoSync, i, ms, lfoSync2, lfoValue, ms2 );
			}

			lfoSync = PresetConverter.Zebra2Preset.LFOSync.SYNC_1s;
			for (int i = 0; i <= 200; i += 2) {
				double ms = PresetConverter.Zebra2Preset.LFOSyncAndValueToMilliseconds(lfoSync, i);

				// test reverse conversion
				PresetConverter.Zebra2Preset.LFOSync lfoSync2 = PresetConverter.Zebra2Preset.LFOSync.SYNC_0_1s;
				int lfoValue = 0;
				PresetConverter.Zebra2Preset.MillisecondsToLFOSyncAndValue((float) ms, out lfoSync2, out lfoValue);
				double ms2 = PresetConverter.Zebra2Preset.LFOSyncAndValueToMilliseconds(lfoSync2, lfoValue);
				
				Console.Out.WriteLine("{0} SyncValue:{1} {2:0.000} ms (Reverse: {3} {4} {5:0.000} ms)", lfoSync, i, ms, lfoSync2, lfoValue, ms2 );
			}
			
			lfoSync = PresetConverter.Zebra2Preset.LFOSync.SYNC_10s;
			for (int i = 0; i <= 200; i += 2) {
				double ms = PresetConverter.Zebra2Preset.LFOSyncAndValueToMilliseconds(lfoSync, i);

				// test reverse conversion
				PresetConverter.Zebra2Preset.LFOSync lfoSync2 = PresetConverter.Zebra2Preset.LFOSync.SYNC_0_1s;
				int lfoValue = 0;
				PresetConverter.Zebra2Preset.MillisecondsToLFOSyncAndValue((float) ms, out lfoSync2, out lfoValue);
				double ms2 = PresetConverter.Zebra2Preset.LFOSyncAndValueToMilliseconds(lfoSync2, lfoValue);
				
				Console.Out.WriteLine("{0} SyncValue:{1} {2:0.000} ms (Reverse: {3} {4} {5:0.000} ms)", lfoSync, i, ms, lfoSync2, lfoValue, ms2 );
			}
			
			Console.ReadKey();
			return;
			
			// TEST THE CALCULATE LFO DELAY METHODS
			int sampleRate = 44100;
			string[] filePaths = Directory.GetFiles("../Release", "*.wav");
			foreach (string wavFilePath in filePaths) {
				if (wavFilePath.Contains("LFO")) {
					DirectoryInfo fileDir = new DirectoryInfo(wavFilePath);
					Console.Out.WriteLine(fileDir.Name);
					float[] wavData = AudioUtils.ReadMonoFromFile(wavFilePath, sampleRate, 0, 0);

					//float[] wavDataAbs = MathUtils.Abs(wavData);
					
					double ms = 0;
					double Hz = 0;
					if (AudioUtils.CalculateLFODelay(wavData, sampleRate, out ms, out Hz, 0.05f, 50)) {
						Console.Out.WriteLine("Sampled:\t{0:0.000} ms\t{1:0.000} Hz", ms, Hz);

						// get enum name
						string enumName = "";
						var regex = new Regex(@"LFO[-_](\d+_\d+\w?)[-_]");
						var match = regex.Match(fileDir.Name);
						if (match.Success) {
							enumName = "LFO_" + match.Groups[1].Value;
						}
						if(Enum.IsDefined(typeof(AudioUtils.LFOTIMING), enumName)) {
							AudioUtils.LFOTIMING timing = StringUtils.StringToEnum<AudioUtils.LFOTIMING>(enumName);
							Console.Out.WriteLine("Calculated:\t{0:0.000} ms\t{1:0.000} Hz", AudioUtils.LFOOrDelayToMilliseconds(timing), AudioUtils.LFOOrDelayToFrequency(timing));
						}
					}
					
					// store as a png
					//int resolution = 0;
					//System.Drawing.Bitmap png = CommonUtils.FFT.AudioAnalyzer.DrawWaveform(wavData, new System.Drawing.Size(1000, 600), resolution, 1, 0, 44100);
					//string fileName = String.Format("{0}.png", fileDir.Name);
					//png.Save(fileName);
					
					// store as wav
					//string wavFileName = String.Format("{0}.wav", fileDir.Name);
					//AudioUtils.CreateWaveFile(wavData, wavFileName, new WaveFormat(sampleRate, 1));
				}
			}

			Console.ReadKey();
			return;


			// TEST THE CALCULATE LFO TIMING METHODS
			AudioUtils.OutputLFOTimings();
			Console.ReadKey();
			return;
			
			
			// TEST ??
			string[] filePaths = Directory.GetFiles("../..", "*.wav");
			foreach (string file in filePaths) {
				// perivar-filter-2022hz.fxp
				string hertz = "";
				var regex = new Regex(@"perivar-filter-(\d+)hz");
				var match = regex.Match(file);
				if (match.Success) {
					hertz = match.Groups[1].Value;
				}
				int hertzValue = 0;
				int.TryParse(hertz, out hertzValue);
			}
			
			string wavFilePath = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\SynthAnalysisStudio\bin\Release\audio-attack-0,03s-201201191521061845.wav";
			float[] wavData = AudioUtils.ReadMonoFromFile(wavFilePath, 44100, 0, 0);

			// store as a png
			System.Drawing.Bitmap png = CommonUtils.FFT.AudioAnalyzer.DrawWaveform(wavData, new System.Drawing.Size(1000, 600), 10000, 1, 0, 44100);
			string fileName = String.Format("audio-{0}.png", 1);
			png.Save(fileName);

			return;

			string wavFilePath = @"C:\Users\perivar.nerseth\Music\Sine-500hz-60sec.wav";
			//string wavFilePath = @"C:\Users\perivar.nerseth\Music\Per Ivar Samples\Rihanna - Who's That Chick (Prod. By David Guetta) (Synth and Bass).wav";
			//string wavFilePath = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\SynthAnalysisStudio\3.wav";

			float[] wavData = AudioUtils.ReadMonoFromFile(wavFilePath, 44100, 0, 0);

			//float[] wavDataAbs = MathUtils.Abs(wavData);
			
			//System.Drawing.Bitmap png = CommonUtils.FFT.AudioAnalyzer.DrawWaveform(wavDataAbs, new System.Drawing.Size(1000, 600), 1, 1, 0, 44100);
			System.Drawing.Bitmap png = CommonUtils.FFT.AudioAnalyzer.DrawWaveform(wavData, new System.Drawing.Size(1000, 600), 5, 1, 0, 44100);
			string fileName = String.Format("wave-{0}.png", 1);
			png.Save(fileName);
				
			Console.ReadKey();
			return;

						
			using (WaveFileWriter writer = new WaveFileWriter("wav1.wav", new WaveFormat(44100, 1)))
			{
				writer.WriteSamples(wavData, 0, wavData.Length);
			}

			using (WaveFileWriter writer = new WaveFileWriter("wav1-cropped.wav", new WaveFormat(44100, 1)))
			{
				writer.WriteSamples(wavDataCropped, 0, wavDataCropped.Length);
			}
			
			
			TimeSpan t = AudioUtils.GetWaveFileTotalTime(wavFilePath);
			var stream=new MemoryStream(File.ReadAllBytes(wavFilePath));
			byte[] bArray = AudioUtils.ResampleWav(stream, new WaveFormat(8000, 8, 1));
			return;
			
			float min = 0.0f;
			float max = 0.0f;
			MathUtils.ComputeMinAndMax(wavData, out min, out max);
			System.Diagnostics.Debug.WriteLine(String.Format("Wav data value range: Min: {0}. Max: {1}.", min, max));
			float[] wavDataFixed = MathUtils.ConvertRangeAndMainainRatio(wavData, min, max, -32768, 32767);
			
			//WaveFileReader reader = new WaveFileReader(wavFilePath);
			//byte[] audioData = AudioUtils.ReadFully(reader);
			//byte[] audioData = AudioUtils.ResampleWav(reader, new WaveFormat(8000, 8, 1));
			//byte[] audioData = AudioUtils.ResampleWav(wavFilePath, new WaveFormat(8000, 16, 1));
			//float[] floatAudioData = BinaryFile.FloatArrayFromByteArray(audioData);
			//Export.exportCSV("sine-sample-data.csv", floatAudioData);
			
			System.Drawing.Bitmap png = CommonUtils.FFT.AudioAnalyzer.DrawWaveform(wavData, new System.Drawing.Size(1000, 600), 10000);
			string fileName = String.Format("wave-sine-{0}.png", 1);
			png.Save(fileName);

			//System.Drawing.Bitmap png2 = CommonUtils.FFT.AudioAnalyzer.DrawWaveform2(wavData, new System.Drawing.Size(1000, 600), 1);
			//string fileName2 = String.Format("wave-sine-{0}.png", 2);
			//png2.Save(fileName2);

			//System.Drawing.Bitmap png3 = CommonUtils.FFT.AudioAnalyzer.DrawWaveform3(wavDataFixed, new System.Drawing.Size(1000, 600), 1);
			//string fileName3 = String.Format("wave-sine-{0}.png", 3);
			//png3.Save(fileName3);

			return;
			
			/*
			float frequency = 5000;
			float amplitude = 0.5f; // let's not hurt our ears
			double sampleRate = 44100;
			int fftWindowsSize = 4096; // 8192;
			int fftOverlap = 1;
			int numSeconds = 1;
			float[] buffer = MathUtils.GetSineWave(frequency, amplitude, (float) sampleRate, 0, (int) sampleRate*numSeconds);
			float[] spectrumData = CommonUtils.FFT.AudioAnalyzer.CreateSpectrumAnalysisLomont(buffer, sampleRate, fftWindowsSize, fftOverlap);
			System.Drawing.Bitmap png = CommonUtils.FFT.AudioAnalyzer.PrepareAndDrawSpectrumAnalysis(spectrumData, sampleRate, fftWindowsSize, fftOverlap,
			                                                                                         new System.Drawing.Size(1000, 600),
			                                                                                         0, 20000);
			string fileName = String.Format("{0:00.0000}dB.png", MathUtils.ConvertFloatToDB(amplitude));
			png.Save(fileName);
			return;
			 */
			
			string pluginPath = "";
			string waveInputFilePath = "";
			string waveOutputFilePath = "";
			string fxpFilePath = "";
			bool doPlay = false;
			bool useGui = false;

			// Command line parsing
			var CommandLine = new Arguments(args);
			if(CommandLine["plugin"] != null) {
				pluginPath = CommandLine["plugin"];
			}
			if(CommandLine["wavein"] != null) {
				waveInputFilePath = CommandLine["wavein"];
			}
			if(CommandLine["waveout"] != null) {
				waveOutputFilePath = CommandLine["waveout"];
			}
			if(CommandLine["fxp"] != null) {
				fxpFilePath = CommandLine["fxp"];
			}
			if(CommandLine["play"] != null) {
				doPlay = true;
			}
			if(CommandLine["gui"] != null) {
				useGui = true;
			}
			
			if ((!useGui && pluginPath == "" && waveInputFilePath == "") || (!useGui && waveOutputFilePath == "" && !doPlay)) {
				PrintUsage();
				return;
			}
			
			if (useGui) {
				StartGUI();
			} else {
				//StartAudioOutput(pluginPath);
				StartVstHost(pluginPath, waveInputFilePath, fxpFilePath, waveOutputFilePath, doPlay );
			}
		}
		
		public static void PrintUsage() {
			Console.WriteLine("Process VST Plugin. Version {0}.", _version);
			Console.WriteLine("Copyright (C) 2009-2015 Per Ivar Nerseth.");
			Console.WriteLine();
			Console.WriteLine("Usage: SynthAnalysisStudio.exe <Arguments>");
			Console.WriteLine();
			Console.WriteLine("Mandatory Arguments:");
			Console.WriteLine("\t-plugin=<path to the vst plugin to use (.dll)>");
			Console.WriteLine("\t-wavein=<path to the wave file to use (.wav)>");
			Console.WriteLine();
			Console.WriteLine("Optional Arguments:");
			Console.WriteLine("\t-fxp=<path to the vst preset file to use (.fxp or .fxb)>");
			Console.WriteLine("\t-play=<should we play the wave file, or only process it?>");
			Console.WriteLine("\t-gui=<Use GUI instead>");
			Console.WriteLine("\t-waveout=<path to the wave file to create (.wav)>");
		}
		
	}
}
