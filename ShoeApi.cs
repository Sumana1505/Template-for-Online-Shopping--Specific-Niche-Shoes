using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Net.Http.Formatting;

namespace ShoeServices
{
    public static class ShoeAPI
    {
        private static string connection;
        //public static void Run(TimerInfo myTimer, ILogger log,ExecutionContext context)
        //{
        //    var config = new ConfigurationBuilder()
        //         .SetBasePath(context.FunctionAppDirectory)
        //         .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
        //         .AddEnvironmentVariables()
        //         .Build();
        //    var appSettingValue = config["appSettingKey"];
        //    connection = config.GetConnectionString("SqlConnectionString");

        //}

        [FunctionName("ChangePassword")]
        public static async Task<IActionResult> ChangePwd(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {


            log.LogInformation("HTTP trigger change Password  request.");
 
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string email = data.email;
            string oldPassword = data.OldPassword;
            string newPassword = data.NewPassword;
            string responseResult = "";
            connection = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=ShoeaDB;Data Source=.;";
            using (var con1 = new SqlConnection(connection))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "usp_changePassword";
                    cmd.Parameters.AddWithValue("@email",email);
                    cmd.Parameters.AddWithValue("@OldPassword", oldPassword);
                    cmd.Parameters.AddWithValue("@NewPassword", newPassword);
                    SqlParameter result = new SqlParameter("@result", SqlDbType.VarChar, 100);
                    result.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(result);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con1;
                    con1.Open();
                    cmd.ExecuteNonQuery();
                    responseResult = result.Value.ToString();
                }
                catch (Exception ex)
                {
                    log.LogInformation(ex.Message);

                }
            }
            string responseMessage = string.IsNullOrEmpty(responseResult)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("UpdateWallet")]
        public static async Task<IActionResult> UpdateWallet(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {


            log.LogInformation("HTTP trigger update wallet  request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string email = data.email;
            var amount = data.Amount;
            string responseResult = "";

            connection = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=ShoeaDB;Data Source=.;";
            using (var con1 = new SqlConnection(connection))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "usp_UpdateWallet";
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    SqlParameter result = new SqlParameter("@result", SqlDbType.VarChar, 100);
                    result.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(result);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con1;
                    con1.Open();
                    cmd.ExecuteNonQuery();
                    responseResult = result.Value.ToString();
                }
                catch (Exception ex)
                {
                    log.LogInformation(ex.Message);

                }
            }
            string responseMessage = string.IsNullOrEmpty(responseResult)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("GetAddress")]
        public static async Task<HttpResponseMessage> GerAddress(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {


            log.LogInformation("HTTP trigger update wallet  request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string email = data.email;

            connection = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=ShoeaDB;Data Source=.;";
            using (var con1 = new SqlConnection(connection))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "usp_GetCustomerAddress";
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con1;
                    con1.Open();
                    var result = cmd.ExecuteReader();
                    
                    List<UserAddress> address = new List<UserAddress>();
                    while (result.Read())
                    {
                        UserAddress addr = new UserAddress();
                        addr.Addresstype = result.GetString(0);
                        addr.Address = result.GetString(1);
                        address.Add(addr);
                    }
                    var jsonToReturn = JsonConvert.SerializeObject(address);
                    return new HttpResponseMessage(HttpStatusCode.OK) 
                    { Content  = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
                    };
                    //return new JsonResult(jsonToReturn);
                }
                catch (Exception ex)
                {
                    log.LogInformation(ex.Message);

                }
                return new HttpResponseMessage(HttpStatusCode.OK) {};

            }
        }

    }
}
