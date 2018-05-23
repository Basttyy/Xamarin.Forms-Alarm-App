﻿using System;
using System.Collections.Generic;
using System.Text;
using AlarmApp.Models;
using Xamarin.Forms;

namespace AlarmApp.Views
{
	public partial class DaysOfWeekView : ContentView
	{
		StringBuilder _sb;

		string[] days = {
			"Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"
		};

		public DaysOfWeekView()
		{
			InitializeComponent();
		}


		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			if (BindingContext == null) return;
			var alarm = ((Alarm)BindingContext);

			var daysOfWeek = alarm.Days;
			_sb = new StringBuilder();

			for (int i = 0; i < daysOfWeek.AllDays.Length; i++)
			{
				var hasDay = daysOfWeek.AllDays[i];
				if(hasDay)
				{
					if (i > 0)
					{
						_sb.Append(", ");
					}
					_sb.Append(days[i]);
				}
			}

			DaysLabel.Text = _sb.ToString();
			//MondayLabel.IsVisible = daysOfWeek.Monday;
			//TuesdayLabel.IsVisible = daysOfWeek.Tuesday;
			//WednesdayLabel.IsVisible = daysOfWeek.Wednesday;
			//ThursdayLabel.IsVisible = daysOfWeek.Thursday;
			//FridayLabel.IsVisible = daysOfWeek.Friday;
			//SaturdayLabel.IsVisible = daysOfWeek.Saturday;
			//SundayLabel.IsVisible = daysOfWeek.Sunday;
		}
	}
}
