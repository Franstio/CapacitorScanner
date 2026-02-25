using CapacitorScanner.Core.Model;
using CapacitorScanner.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CapacitorScanner.ViewModels
{
	public partial class SettingsViewModel : ObservableObject
	{
		[ObservableProperty]
		private ConfigModel config = null!;

		private ConfigService ConfigService = null!;
		[ObservableProperty]
		private bool isLogin = false;
		AppState _state = null!;
		public SettingsViewModel(ConfigService configService,AppState state)
		{
			ConfigService = configService;
			config = ConfigService.Config;
			_state = state;
		}
		[RelayCommand]
		public void onLoad()
		{
			IsLogin = _state.Login is not null;
		}

		[RelayCommand]
		public void SaveConfig()
		{
			ConfigService.Save();
		}
	}
}