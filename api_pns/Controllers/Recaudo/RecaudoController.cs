using api_pns.Context;
using api_pns.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System;

namespace api_pns.Controllers.Recaudo
{
    [Route("api")]
    [ApiController]
    public class RecaudoController : Controller
    {
        public Connection conn;
        private readonly IConfiguration _configuration;
        public ReplySucess oReply = new ReplySucess();

        public RecaudoController(IConfiguration configuration)
        {
            _configuration = configuration;
            conn = new Connection();
        }

        #region Listar ordenes para recaudo
        // GET: api/listOrdersPay
        /// <summary>
        /// Listar ordenes en recaudo
        /// </summary>
        /// <remarks>
        /// Método para listar ordenes en recaudo
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("listOrdersPay")]
        public async Task<IActionResult> listOrdersPay()
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_listOrdersPay", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    List<Dictionary<string, object>> details = new List<Dictionary<string, object>>();

                    while (await sqldr.ReadAsync())
                    {
                        Dictionary<string, object> data = new Dictionary<string, object>();
                        if (sqldr["id_take_order"] != DBNull.Value) { data.Add("idTakeOrder", Convert.ToInt32(sqldr["id_take_order"])); } else { data.Add("idTakeOrder", ""); }
                        if (sqldr["id_table"] != DBNull.Value) { data.Add("idTable", Convert.ToInt32(sqldr["id_table"])); } else { data.Add("idTable", ""); }
                        if (sqldr["name"] != DBNull.Value) { data.Add("name", sqldr["name"].ToString()); } else { data.Add("name", ""); }
                        if (sqldr["totalValue"] != DBNull.Value) { data.Add("totalValue", sqldr["totalValue"].ToString()); } else { data.Add("totalValue", ""); }
                        if (sqldr["paid"] != DBNull.Value) { data.Add("paid", Convert.ToBoolean(sqldr["paid"])); } else { data.Add("paid", ""); }

                        details.Add(data);
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = r.Flag ? 200 : 400;


                    if (r.Flag == true)
                    {
                        r.Data = details;
                        r.Status = 200;

                        oReply.Ok = true;
                        oReply.Message = r.Message;
                        oReply.Data = r.Data;

                        return Ok(oReply);
                    }
                    else
                    {
                        oReply.Ok = false;
                        oReply.Message = r.Message;
                        oReply.Data = null;

                        return BadRequest(oReply);
                    }
                }
                catch (Exception ex)
                {
                    oReply.Ok = false;
                    oReply.Message = ex.Message;
                    oReply.Data = null;
                    return BadRequest(oReply);
                }
            }
        }
        #endregion
    }
}
