using CapacitorScanner.Core.Model;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner.Messages
{
    public class LoginMessage : AsyncRequestMessage<LoginModel?>
    {
        public LoginModel? LoginModel { get; private set; } = null;
        public bool IsClosing { get; private set; } = false;
        public LoginMessage(LoginModel? loginModel)
        {
            LoginModel = loginModel;
            IsClosing = true;
        }
        public LoginMessage()
        {
            IsClosing = false;
        }
    }
}
