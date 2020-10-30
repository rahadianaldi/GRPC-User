using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace HandsOn_User
{
    public class GreeterService : Greeter.GreeterBase
    {
        static string conn = "Host=mostrans.ccoltfh3q1mc.us-east-2.rds.amazonaws.com;Username=buku_tamu;Password=Iniadalahpassword;Database=buku_tamu;Pooling=false;Timeout=30;CommandTimeout=30";
        NpgsqlConnection conPG = new NpgsqlConnection(conn);
        string strQuery = "";

        public NpgsqlCommand ExecutePGQuery(string Sql)
        {
            Sql.Replace("--", "");
            Sql.Replace("/*", "");
            Sql.Replace("*\\", "");

            NpgsqlCommand command = new NpgsqlCommand(Sql, conPG);

            return command;
        }

        public override async Task getAllCustomer(Empty request,
   Grpc.Core.IServerStreamWriter<Customer> responseStream,
   Grpc.Core.ServerCallContext context)
        {
            List<Customer> responses = new List<Customer>();
            try
            {
                conPG.Open();
                strQuery = "SELECT * FROM handson_user";
                NpgsqlCommand command = ExecutePGQuery(strQuery);
                NpgsqlDataReader dtr = command.ExecuteReader();
                while (dtr.Read())
                {
                    Customer cus = new Customer();
                    cus.Id = Convert.ToString(dtr["id"]);
                    cus.Username = Convert.ToString(dtr["username"]);
                    cus.Password =  Convert.ToString(dtr["password"]);

                    responses.Add(cus);
                }
            }
            catch (Exception)
            {
                throw;
            }
            foreach (var response in responses)
            {
                await responseStream.WriteAsync(response);
            }
        }

        public override Task<Customer> getCustomerById(CustomerId request, ServerCallContext context)
        {
            Customer Res = new Customer();
            try
            {
                conPG.Open();
                strQuery = "SELECT * FROM handson_user where id = " + request.Id;
                NpgsqlCommand command = ExecutePGQuery(strQuery);
                NpgsqlDataReader dtr = command.ExecuteReader();
                while (dtr.Read())
                {
                    Res.Id = Convert.ToString(dtr["id"]);
                    Res.Username = Convert.ToString(dtr["username"]);
                    Res.Password = Convert.ToString(dtr["password"]);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Task.FromResult(Res);
        }

        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}
