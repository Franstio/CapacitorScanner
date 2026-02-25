using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner.Core.Model
{
    public class ScrapTransaction
    {
        public int Id { get; set; } = 0;
        public string TransactionDate { get; set; } = string.Empty;
        public string LoginDate { get; set; } = string.Empty;
        public string Badgeno { get; set; } = string.Empty;
        public string Container { get; set; } = string.Empty;
        public string Bin { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public double WeightResult { get; set; } = 0.0;
        public int DoorStatus { get; set; } = 0;
        public string Activity { get; set; } = string.Empty;
        public string ScrapItemName { get; set; } = string.Empty;
        public string ScrapTypeName { get; set; } = string.Empty;
        public string ScrapGroupName { get; set; } = string.Empty;
        public string LastBadgeno { get; set; } = string.Empty;

        public ScrapTransaction() { }

        public ScrapTransaction(
            int id,
            string transactionDate,
            string loginDate,
            string badgeno,
            string container,
            string bin,
            string status,
            string host,
            double weightResult,
            int doorStatus,
            string activity,
            string scrapItemName,
            string scrapTypeName,
            string scrapGroupName,
            string lastBadgeno)
        {
            Id = id;
            TransactionDate = transactionDate;
            LoginDate = loginDate;
            Badgeno = badgeno;
            Container = container;
            Bin = bin;
            Status = status;
            Host = host;
            WeightResult = weightResult;
            DoorStatus = doorStatus;
            Activity = activity;
            ScrapItemName = scrapItemName;
            ScrapTypeName = scrapTypeName;
            ScrapGroupName = scrapGroupName;
            LastBadgeno = lastBadgeno;
        }
    }

}
