﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace NAudio_Visualizing
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			
			NAudioEngine soundEngine = NAudioEngine.Instance;
			soundEngine.PropertyChanged += NAudioEngine_PropertyChanged;
			
			customWaveViewer1.RegisterSoundPlayer(soundEngine);
			//waveDisplayUserControl.RegisterSoundPlayer(soundEngine);
		}
		
		#region NAudio Engine Events
		private void NAudioEngine_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			NAudioEngine engine = NAudioEngine.Instance;
			switch (e.PropertyName)
			{
				case "FileTag":
					if (engine.FileTag != null)
					{
						TagLib.Tag tag = engine.FileTag.Tag;
					}
					else
					{
						//albumArtPanel.AlbumArtImage = null;
					}
					break;
				case "ChannelPosition":
					//clockDisplay.Time = TimeSpan.FromSeconds(engine.ChannelPosition);
					txtTime.Text = TimeSpan.FromSeconds(engine.ChannelPosition).ToString();
					break;
				default:
					// Do Nothing
					break;
			}

		}
		#endregion
		
		void BtnBrowseClick(object sender, EventArgs e)
		{
			openFileDialog.Filter = "(*.mp3)|*.mp3";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				NAudioEngine.Instance.OpenFile(openFileDialog.FileName);
				txtFilePath.Text = openFileDialog.FileName;
			}
		}
		
		void BtnPlayClick(object sender, EventArgs e)
		{
			if (NAudioEngine.Instance.CanPlay)
				NAudioEngine.Instance.Play();
		}
		
		void BtnPauseClick(object sender, EventArgs e)
		{
			if (NAudioEngine.Instance.CanPause)
				NAudioEngine.Instance.Pause();
		}
		
		void BtnStopClick(object sender, EventArgs e)
		{
			if (NAudioEngine.Instance.CanStop)
				NAudioEngine.Instance.Stop();
		}
		
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			NAudioEngine.Instance.Dispose();
		}
	}
}
