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
                    activity,
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
                    @Activity,
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
                string query = $"Select bin,weight,binweight,lastfrombinname,lastbadgeno,maxweight,wastetype,hostname,status from binhost where bin=@bin";
                var res = await con.QueryFirstOrDefaultAsync<BinLocalModel>(query, new { bin });
                return res;
            }
        }
        public async Task<IEnumerable<BinLocalModel>> GetBin()
        {
            using (var con = await GetConn())
            {
                string query = $"Select bin,weight,binweight,lastfrombinname,lastbadgeno,maxweight,wastetype,hostname,status from binhost";
                var res = await con.QueryAsync<BinLocalModel>(query);
                return res;
            }
        }
        public async Task UpdateBin(BinLocalModel bin)
        {
            using (var con = await GetConn())
            {
                string query = $"Update binhost set weight=@weight,lastfrombinname=@lastfrombinname,lastbadgeno=@lastbadgeno,binweight=@binweight,wastetype=@wastetype,hostname=@hostname,status=@status where bin=@bin";
                await con.ExecuteAsync(query, bin);
            }

        }
        public async Task InsertBinHost(BinLocalModel bin)
        {
            using (var con = await GetConn())
            {
                string query = $"Insert into binhost(bin,weight,binweight,maxweight,hostname,status,wastetype,lastfrombinname,lastbadgeno) values(@bin,@weight,@binweight,@maxweight,@hostname,@status,@wastetype,@lastfrombinname,@lastbadgeno)";
                await con.ExecuteAsync(query, bin);
            }
        }
        public async Task<IEnumerable<ScrapTransactionModel>> GetFailedScrapTransaction()
        {
            using (var con = await GetConn())
            {
                string query = $"Select id,transaction_date as TransactionDate,login_date as LoginDate,badgeno as BadgeNo,container as Container,bin as Bin,status as Status,host as Host,weightresult as WeightResult,activity as Activity,lastbadgeno as LastBadgeNo from scraptransaction" +
                    $" where status = 'FAILED'";

                return await con.QueryAsync<ScrapTransactionModel>(query);
            }
        }
        public async Task UpdateStatus(string status, int id)
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
                await con.ExecuteAsync(query, new { status, bin });
            }
        }
        public async Task DeleteBin(string bin)
        {
            using (var con = await GetConn())
            {
                string query = $"delete from binhost where bin=@bin";
                await con.ExecuteAsync(query, new { bin });
            }
        }
        public async Task<IEnumerable<StationInfoLocalModel>> GetStationInfoLocal(string? property = null)
        {
            using (var con = await GetConn())
            {
                string query = $"Select id,property,datavalue from station where property like @property";
                return await con.QueryAsync<StationInfoLocalModel>(query, new { property = $"%{property}%" });
            }
        }
        public async Task AddOrUpdateStationInfoLocal(StationInfoLocalModel stationInfoLocal)
        {
            var stationData = await GetStationInfoLocal(stationInfoLocal.property);
            using (var con = await GetConn())
            {
                string query = "";
                if (stationData.Any())
                {
                    query = $"Update station set datavalue=@datavalue where property=@property";
                }
                else
                {
                    query = $"Insert into station(property,datavalue) values(@property,@datavalue)";
                }
                await con.ExecuteAsync(query, stationInfoLocal);
            }
        }
        public async Task InsertEmployee(EmployeeLocalModel employeeLocalModel)
        {
            var check = await GetEmployee(employeeLocalModel.badgeno);
            if (check.Any())
                return;
            using (var con = await GetConn())
            {
                await con.ExecuteAsync($"Insert into employee(employeename,badgeno,registerdate) values(@employeename,@badgeno,@registerdate)", employeeLocalModel);
            }
        }
        public async Task<IEnumerable<EmployeeLocalModel>> GetEmployee(string? badgeNo = null)
        {
            using (var con = await GetConn())
            {
                string query = $"Select id,employeename,badgeno,registerdate from employee where badgeno=@badgeNo";
                return await con.QueryAsync<EmployeeLocalModel>(query, new { badgeNo });
            }
        }
    }
}
