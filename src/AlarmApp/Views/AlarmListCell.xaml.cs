﻿using System;
using System.Collections.Generic;
using AlarmApp.Models;
using Xamarin.Forms;

namespace AlarmApp.Views
{
	public partial class AlarmListCell : ViewCell
	{
		public AlarmListCell()
		{
			InitializeComponent();
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			if (BindingContext == null) return;
			var alarm = (Alarm)BindingContext;

			TimeLabel.Text = alarm.Time.ToString(@"hh\:mm");
			var freq = alarm.UserFriendlyFrequency;
			FrequencyLabel.Text = string.IsNullOrWhiteSpace(freq) ? null : $"Every {freq}";
			//DaysOfWeekView.BindingContext = alarm.Days;
		}
	}
}
