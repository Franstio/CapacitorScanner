using CapacitorScanner.Model;
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

namespace CapacitorScanner.Services
{
    public class SQLiteService
    {
        private string _connectionString;
        private ConfigService _config;
        public SQLiteService(ConfigService config)
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task CreateTransaction(ScrapTransaction transaction)
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
                var res = await con.QueryAsync<LoginModel>(query,login);
                return res.FirstOrDefault();
            }
        }
        public async Task<string> GetHostname(string bin)
        {
            using (var con = await GetConn())
            {
                string query = $"Select hostname from binhost where bin=@bin";
                var res = await con.ExecuteReaderAsync(query, new {bin});
                await res.ReadAsync()!;
                return res.GetString(0);
            }
        }
    }
}
