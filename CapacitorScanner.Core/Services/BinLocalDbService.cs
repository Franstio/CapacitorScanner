using CapacitorScanner.Core.Model;
using CapacitorScanner.Core.Model.LocalDb;
using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner.Core.Services
{
    public class BinLocalDbService
    {
        private string _connectionString;
        private ConfigService _config;
        public BinLocalDbService(ConfigService config)
        {
            _config = config;
            var builder = new SqliteConnectionStringBuilder();
            builder.DataSource = config.Config.dbpath;
            _connectionString = builder.ToString();
        }

        private async Task<SqliteConnection> GetConn()
        {
            var conn = new SqliteConnection(_connectionString);
            await conn.OpenAsync();
            return conn;
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

        public async Task Initialization()
        {

            try
            {
                if (!File.Exists(_config.Config.dbpath))
                {
                    var fs = File.Create(_config.Config.dbpath);
                    fs.Close();
                    string file = await File.ReadAllTextAsync("scanner-init.sql");
                    using (var con = await GetConn())
                    {
                        await con.ExecuteAsync(file);
                        string query = "insert into login(username,password) values(@username,@password)";
                        await con.ExecuteAsync(query, new { username = "admin", password = HashPassword("123") });
                    }
                }
            }
            catch (Exception ex)
            {
                File.Delete(_config.Config.dbpath);
                Console.WriteLine(ex.Message);
            }
        }
        public async Task CreateTransaction(ScrapTransactionModel transaction)
        {
            using (var con = await GetConn())
            {
                await con.ExecuteAsync(@"INSERT INTO ScrapTransaction (
                    transaction_date,
                    login_date,
                    badgeno,
                    container,
                    bin,
                    status,
                    host,
                    weightresult,
                    doorstatus,
                    activity,
                    scrapitem_name,
                    scraptype_name,
                    scrapgroup_name,
                    lastbadgeno
                ) VALUES (
                    @TransactionDate,
                    @LoginDate,
                    @Badgeno,
                    @Container,
                    @Bin,
                    @Status,
                    @Host,
                    @WeightResult,
                    @DoorStatus,
                    @Activity,
                    @ScrapItemName,
                    @ScrapTypeName,
                    @ScrapGroupName,
                    @LastBadgeno
                );", transaction);
            }
        }

        public async Task<LoginModel?> Login(LoginModel login)
        {
            using (var con = await GetConn())
            {
                string query = $"Select id,username,password from login where username=@username and password=@password";
                var res = await con.QueryAsync<LoginModel>(query, login);
                return res.FirstOrDefault();
            }
        }
        public async Task<string> GetHostname(string bin)
        {
            using (var con = await GetConn())
            {
                string query = $"Select hostname from binhost where bin=@bin";
                var res = await con.ExecuteScalarAsync<string>(query, new { bin });
                return res!;
            }
        }
        public async Task<BinLocalModel?> GetBin(string bin)
        {
            using (var con = await GetConn())
            {
                string query = $"Select bin,weight,binweight,maxweight,wastetype,hostname,status from binhost where bin=@bin";
                var res = await con.QueryFirstOrDefaultAsync<BinLocalModel>(query, new { bin });
                return res;
            }
        }
        public async Task<IEnumerable<BinLocalModel>> GetBin()
        {
            using (var con = await GetConn())
            {
                string query = $"Select bin,weight,binweight,maxweight,wastetype,hostname,status from binhost";
                var res = await con.QueryAsync<BinLocalModel>(query);
                return res;
            }
        }
        public async Task UpdateBin(BinLocalModel bin)
        {
            using (var con = await GetConn())
            {
                string query = $"Update binhost set weight=@weight,binweight=@binweight,wastetype=@wastetype,hostname=@hostname,status=@status where bin=@bin";
                await con.ExecuteAsync(query, bin);
            }

        }
        public async Task InsertBinHost(BinLocalModel bin)
        {
            using (var con = await GetConn())
            {
                string query = $"Insert into binhost(bin,weight,binweight,maxweight,hostname,status,wastetype) values(@bin,@weight,@binweight,@maxweight,@hostname,@status,@wastetype)";
                await con.ExecuteAsync(query, bin);
            }
        }
        public async Task<IEnumerable<ScrapTransactionModel>> GetFailedScrapTransaction()
        {
            using (var con = await GetConn())
            {
                string query = $"Select id,transaction_date as TransactionDate,login_date as LoginDate,badgeno as BadgeNo,container as Container,bin as Bin,status as Status,host as Host,weighresult as WeightResult,activity as Activity,lastbadgeno as LastBadgeNo from scraptransaction" +
                    $" where status != 'FAILED'";

                return await con.QueryAsync<ScrapTransactionModel>(query);
            }
        }
        public async Task UpdateStatus(string status,int id)
        {
            using (var con = await GetConn())
            {
                string query = $"Update scraptransaction set status=@status where id=@id";
                await con.ExecuteAsync(query, new { status, id });  
            }
        }
        public async Task UpdateStatusBin(string status, string bin)
        {
            using (var con = await GetConn())
            {
                string query = $"Update binhost set status=@status where bin=@bin";
                await con.ExecuteAsync(query, new { status, bin});
            }
        }
    }
}
