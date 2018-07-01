﻿using System;
using AlarmApp.Services;
using FreshMvvm;
using PropertyChanged;
using AlarmApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AlarmApp.Helpers;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using AlarmApp.Popups;
using Rg.Plugins.Popup.Pages;
using System.Linq;
using Rg.Plugins.Popup.Services;

namespace AlarmApp.PageModels
{
	[AddINotifyPropertyChangedInterface]
	public class SettingsTonePageModel : FreshBasePageModel
	{
		IFileLocator _fileLocator = DependencyService.Get<IFileLocator>();
		IPlaySoundService _soundService = DependencyService.Get<IPlaySoundService>();

		Uri _newToneUri;
		AlarmToneNamingPopupPage _namingPopupPage;

		IAlarmStorageService _alarmStorage;
		AlarmTone _selectedTone;

		public Settings Settings { get; set; }

		public AlarmTone SelectedTone
		{
			get { return _selectedTone; }
			set 
			{
				if(value != null)
					SetSelectedTone(value);
			}
		}

		public bool FileNeedsNamed { get; set; }

		public ObservableCollection<AlarmTone> AllAlarmTones { get; set; }

		public ICommand ToneSavedCommand 
		{ 
			get
			{
				return new FreshAwaitCommand(async (obj, tcs) =>
				{
					var selectedTone = _selectedTone;
					OnToneSelected(selectedTone);
					await CoreMethods.PopPageModel(false, true);
					tcs.SetResult(true);
				});
			}
		}

		public ICommand DeleteToneCommand
		{
			get
			{
				return new Command<AlarmTone>((alarmTone) =>
				{
					DeleteAlarmTone(alarmTone);
				});
			}
		}

		public SettingsTonePageModel(IAlarmStorageService alarmStorage)
		{
			_alarmStorage = alarmStorage;
		}

		public override void Init(object initData)
		{
			base.Init(initData);

			Settings = _alarmStorage.GetSettings();
			AllAlarmTones = new ObservableCollection<AlarmTone>(_alarmStorage.GetAllTones());
			
		}


		void PlayTone(AlarmTone tone)
		{
			_soundService.PlayAudio(tone);
		}

		/// <summary>
		/// Handles setting the current tone value
		/// </summary>
		/// <param name="value">Value.</param>
		void SetSelectedTone(AlarmTone value)
		{
			var isSelectedNull = value.Equals(default(AlarmTone)) || value == null;
			if (isSelectedNull) return;

			//if the user selected the 'choose custom tone' option, display file explorer
			var wasSelectCustomToneSelected = value.Equals(Defaults.Tones[0]);
			if(wasSelectCustomToneSelected)
			{
				_fileLocator.OpenFileLocator();
				_fileLocator.FileChosen += ToneFileChosen;
				return;
			}

			//_selectedTone = value;
			PlayTone(value);
			AddConfirmToolbarItem();

			_selectedTone = value;
		}

		void AddConfirmToolbarItem()
		{
			if(CurrentPage.ToolbarItems.Count == 0)
			{
				CurrentPage.ToolbarItems.Add(new ToolbarItem
				{
					Text = "Save",
					Icon = "save",
					Command = ToneSavedCommand
				});
			}
		}

		/// <summary>
		/// Allows the user to set a custom tone
		/// </summary>
		AlarmTone GetAlarmToneNotSetTone()
		{
			//temporary until final solution has been implemented
			return new AlarmTone("In future, you will be able to select custom tones", null);
		}

		/// <summary>
		/// Saves the tone to settings when the tone has been selected
		/// </summary>
		/// <param name="alarmTone">The selected alarm tone</param>
		void OnToneSelected(AlarmTone alarmTone)
		{
			_alarmStorage.Realm.Write(() =>
			{
				_alarmStorage.GetSettings().AlarmTone = alarmTone;
			});
		}

		/// <summary>
		/// When the user selects an audio file from the file system
		/// </summary>
		/// <param name="uri">URI of the chosen audio file</param>
		async void ToneFileChosen(Uri uri)
		{
			System.Diagnostics.Debug.WriteLine("pcl: " + uri.LocalPath);
			_newToneUri = uri;
			_namingPopupPage = new AlarmToneNamingPopupPage();
			_namingPopupPage.ToneNameSet += OnNewToneNameSet;
			FileNeedsNamed = true;
		}

		/// <summary>
		/// Action to be done when a newly added tone has its name set
		/// </summary>
		/// <param name="toneName">The name to be given to the alarm tone</param>
		void OnNewToneNameSet(string toneName)
		{
			var newTone = new AlarmTone
			{
				Name = toneName,
				Path = _newToneUri.LocalPath,
				IsCustomTone = true
			};

			AllAlarmTones.Add(newTone);
			_alarmStorage.AddTone(newTone);

			_namingPopupPage.ToneNameSet -= OnNewToneNameSet;
			_fileLocator.FileChosen -= ToneFileChosen;
			_selectedTone = AllAlarmTones.Last();
			SetSelectedTone(newTone);
			FileNeedsNamed = false;
		}

		/// <summary>
		/// Deletes the given alarm tone
		/// </summary>
		/// <param name="alarmTone">Alarm tone to delete</param>
		void DeleteAlarmTone(AlarmTone alarmTone)
		{
			AllAlarmTones.Remove(alarmTone);
			_alarmStorage.Realm.Write(() =>
			{
				_alarmStorage.Realm.Remove(alarmTone);
			});

			_soundService.StopAudio();
		}

		protected async override void ViewIsAppearing(object sender, EventArgs e)
		{
			base.ViewIsAppearing(sender, e);

			if(FileNeedsNamed)
			{
				await PopupNavigation.Instance.PushAsync(_namingPopupPage);
			}
		}

		protected override void ViewIsDisappearing(object sender, EventArgs e)
		{
			_soundService.StopAudio();
			base.ViewIsDisappearing(sender, e);
		}
	}
}
