using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner.Core.Model.LocalDb
{
    public class LoginModel
    {
        public string username { get; set; } = null!;
        public string password { get; set; } = null!;
        public LoginModel() { }
        public LoginModel(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
