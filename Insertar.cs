using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Data.SqlClient;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


namespace FunctionInsertarDB
{
    public static class Insertar
    {
        [FunctionName("Insertar")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,ILogger log)
        {
            string request = await new StreamReader(req.Body).ReadToEndAsync();            
            List<Pago> ids = new List<Pago>();
            ids = JsonConvert.DeserializeObject<List<Pago>>(request);
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            foreach (var registro in ids) 
            {
                var x = DateTime.Parse(registro.Fecha.ToString()); //DateTime.Parse(registro.Fecha.ToString());
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    var text = "INSERT INTO [dbo].[PAGOS] VALUES(" + registro.IdPago + "," + "'" + x + "'" + "," + "'" + registro.Nombre + "'" + "," +"'" + registro.TipoIdentificacion
                                + "'"+ "," + "'" + registro.NumeroIdentificacion + "'" + "," + registro.Monto + ")";

                    using (SqlCommand cmd = new SqlCommand(text, conn))
                    {
                        // Execute the command and log the # rows affected.
                        var rows = await cmd.ExecuteNonQueryAsync();
                        log.LogInformation($"{rows} rows were updated");
                    }
                }

            }           


            return new OkObjectResult(ids);
        }

    }

    public class Pago
    {
        public int IdPago { get; set; }
        public DateTime Fecha { get; set; }
        public string Nombre { get; set; }
        public string TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public decimal Monto { get; set; }

    }  

}
