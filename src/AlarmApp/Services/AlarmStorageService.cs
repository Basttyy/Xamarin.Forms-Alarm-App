﻿using System;
using AlarmApp.Models;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using Realms;
using System.Linq;

namespace AlarmApp.Services
{
	public class AlarmStorageService : IAlarmStorageService
	{
		public Realm Realm { get { return Realm.GetInstance();} }


		public AlarmStorageService()
		{
			
		}

		/// <summary>
		/// Gets all alarms
		/// </summary>
		/// <returns>All alarms</returns>
		public List<Alarm> GetAllAlarms()
		{
			return Realm.All<Alarm>().ToList();
		}

		/// <summary>
		/// Gets the alarms for today
		/// </summary>
		/// <returns>Today's alarms</returns>
		public List<Alarm> GetTodaysAlarms()
		{
			var all = Realm.All<Alarm>();
			return all.ToList().Where(x => x.OccursToday == true).ToList();
		}

		/// <summary>
		/// Adds the alarm
		/// </summary>
		/// <param name="alarm">Alarm to add</param>
		public void AddAlarm(Alarm alarm)
		{
			Realm.Write(() =>
			{
				Realm.Add<Alarm>(alarm);
			});
		}

		/// <summary>
		/// Updates the alarm
		/// </summary>
		/// <param name="alarm">Alarm to update</param>
		public void UpdateAlarm(Alarm alarm)
		{
			Realm.Write(() =>
			{
				Realm.Add<Alarm>(alarm, true);
			});
		}

		/// <summary>
		/// Deletes the alarm
		/// </summary>
		/// <param name="alarm">Alarm we want to delete</param>
		public void DeleteAlarm(Alarm alarm)
		{
			Realm.Write(() =>
			{
				Realm.Remove(alarm);
			});
		}

		/// <summary>
		/// Checks if the given alarm exists
		/// </summary>
		/// <returns><c>true</c>, if alarm was found, <c>false</c> otherwise</returns>
		/// <param name="alarm">The Alarm we want to know already exists</param>
		public bool DoesAlarmExist(Alarm alarm)
		{
			var containsAlarm = Realm.All<Alarm>().Contains(alarm);
			if (containsAlarm)
				return true;

			return false;
		}

		/// <summary>
		/// Deletes all the alarms
		/// </summary>
		public void DeleteAllAlarms()
		{
			Realm.Write(() =>
			{
				Realm.RemoveAll<Alarm>();
			});
		}

		/// <summary>
		/// Gets the settings
		/// </summary>
		/// <returns>The settings object</returns>
		public Settings GetSettings()
		{
			Settings settings = new Settings();

			var settingsList =Realm.All<Settings>();
			var settingsAreFound = settingsList?.Count() > 0;

			if (settingsAreFound)
				settings = settingsList.ElementAt(0);
			else
				Realm.Write(() => Realm.Add(settings));

			return settings;
		}
	}
}
