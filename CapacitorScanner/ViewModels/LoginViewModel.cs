using CapacitorScanner.Core.Model.LocalDb;
using CapacitorScanner.Core.Services;
using CapacitorScanner.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private LoginModel user = new LoginModel();

        private BinLocalDbService _service;
        private AppState _state;
        public LoginViewModel(BinLocalDbService dbservice,AppState state)
        {
            _service = dbservice;
            _state = state;
        }
        string HashPassword(string password)
        {
            byte[] key = Encoding.UTF8.GetBytes("asjdlnkcnalnaehneuvnq1uf9q91fvbcibnckncknkzxn=13fkanp33922acnae");
            using (var hm = new HMACSHA512(key))
            {
                byte[] enc = Encoding.UTF8.GetBytes(password);
                byte[] buffer = hm.ComputeHash(enc);
                return Convert.ToBase64String(buffer);
            }
        }

        [RelayCommand]
        public void Load()
        {
            User = new LoginModel();
        }
        [RelayCommand]
        public async Task Login()
        {
            User.password = HashPassword(User.password);
            var res = await _service.Login(User);
            _state.Login = res;
            try
            {
                await WeakReferenceMessenger.Default.Send(new LoginMessage(_state.Login));
            }
            catch  { }
        }
    }
}