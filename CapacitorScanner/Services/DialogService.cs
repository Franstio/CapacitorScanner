using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner.Services
{
    public class DialogService
    {
        private Window? GetOwner()
        {
            var lifetime = Avalonia.Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            return lifetime?.Windows.FirstOrDefault(w => w.IsActive) ?? lifetime?.MainWindow;
        }

        public async Task ShowMessageAsync(string title, string message)
        {
            var box = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
            {
                ContentTitle = title,
                ContentMessage = message,
                ButtonDefinitions = ButtonEnum.Ok,
                Icon = Icon.Info,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            });

            var owner = GetOwner();
            if (owner is not null)
                await box.ShowAsPopupAsync(owner);
        }

        public async Task<bool> ShowConfirmAsync(string title, string message)
        {
            var box = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
            {
                ContentTitle = title,
                ContentMessage = message,
                ButtonDefinitions = ButtonEnum.YesNo,
                Icon = Icon.Question,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            });

            var owner = GetOwner()!;
            var result = await box.ShowAsPopupAsync(owner);

            return result == ButtonResult.Yes;
        }
    }
}
