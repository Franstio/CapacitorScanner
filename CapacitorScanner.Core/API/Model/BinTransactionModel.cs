using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner.Core.API.Model
{
    public class BinTransactionModel
    {
        public int Stage { get; set; } = 0;
        public string Instruction { get; set; } = string.Empty;
        public TypeTransaction? TransactionType { get; set; } = null;
        public bool IsRunning { get; set; } = false;
        public bool BottomLockTriggerred { get; set; } = false;
        public DateTime? LastRead { get; set; } 
        public enum TypeTransaction
        {
            Idle=0,
            Dispose = 1,
            Collection = 2
        }

    }
}
