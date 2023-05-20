using api_pns.Models.Tables;
using api_pns.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System;
using api_pns.Context;
using Microsoft.Extensions.Configuration;
using api_pns.Models.Ordenes;

namespace api_pns.Controllers.Ordenes
{
    [Route("api")]
    [ApiController]
    public class OrdenesController : Controller
    {
        public Connection conn;
        private readonly IConfiguration _configuration;
        public ReplySucess oReply = new ReplySucess();

        public OrdenesController(IConfiguration configuration)
        {
            _configuration = configuration;
            conn = new Connection();
        }

        #region Listar ordenes
        // GET: api/listOrder
        /// <summary>
        /// Listar ordenes
        /// </summary>
        /// <remarks>
        /// Método para listar ordenes
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("listOrder")]
        public async Task<IActionResult> listOrder()
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_listOrders", connection);

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

        #region Consultar ordenes
        // GET: api/detailOrder/{idTakeOrder}
        /// <summary>
        /// Consultar detalle de orden
        /// </summary>
        /// <remarks>
        /// Método para consultar detalle de una orden
        /// </remarks>
        /// <param name="idCountry">Identificador del pais para consultar ciudades</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("detailOrder/{idTakeOrder}")]
        public async Task<IActionResult> detailOrder([FromRoute] int idTakeOrder)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_detailOrderPay", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id_take_order", idTakeOrder));
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    List<Dictionary<string, object>> details = new List<Dictionary<string, object>>();

                    while (await sqldr.ReadAsync())
                    {
                        Dictionary<string, object> data = new Dictionary<string, object>();
                        if (sqldr["id_order_product"] != DBNull.Value) { data.Add("idOrderProduct", Convert.ToInt32(sqldr["id_order_product"])); } else { data.Add("idOrderProduct", ""); }
                        if (sqldr["id_take_order"] != DBNull.Value) { data.Add("idTakeOrder", Convert.ToInt32(sqldr["id_take_order"])); } else { data.Add("idTakeOrder", ""); }
                        if (sqldr["id_product"] != DBNull.Value) { data.Add("idProduct", Convert.ToInt32(sqldr["id_product"])); } else { data.Add("idProduct", ""); }
                        if (sqldr["name_product"] != DBNull.Value) { data.Add("nameProduct", sqldr["name_product"].ToString()); } else { data.Add("nameProduct", ""); }
                        if (sqldr["name_supplier"] != DBNull.Value) { data.Add("nameSupplier", sqldr["name_supplier"].ToString()); } else { data.Add("nameSupplier", ""); }
                        if (sqldr["amount"] != DBNull.Value) { data.Add("amount", Convert.ToInt32(sqldr["amount"])); } else { data.Add("amount", ""); }
                        if (sqldr["price"] != DBNull.Value) { data.Add("price", Convert.ToInt32(sqldr["price"])); } else { data.Add("price", ""); }
                        if (sqldr["id_table"] != DBNull.Value) { data.Add("idTable", Convert.ToInt32(sqldr["id_table"])); } else { data.Add("idTable", ""); }
                        if (sqldr["name_table"] != DBNull.Value) { data.Add("nameTable", sqldr["name_table"].ToString()); } else { data.Add("nameTable", ""); }
                        if (sqldr["totalValue"] != DBNull.Value) { data.Add("totalValue", sqldr["totalValue"].ToString()); } else { data.Add("totalValue", ""); }
                        if (sqldr["paid"] != DBNull.Value) { data.Add("paid", Convert.ToBoolean(sqldr["paid"])); } else { data.Add("paid", false); }
                        
                        details.Add(data);
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = r.Flag ? 200 : 400;


                    if (r.Flag)
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

        #region Crear o actualizar ordenes
        // POST: api/createUpdateOrder
        /// <summary>
        /// Crear/Actualizar orden
        /// </summary>
        /// <remarks>
        /// Método que me permite crear/actualizar las ordenes registradas en el sistema
        /// </remarks>
        /// <param name="data">Data a validar en db</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpPost]
        [Route("createUpdateOrder")]
        public async Task<IActionResult> createUpdateOrder([FromBody] OrdenesModel data)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_createUpdateOrder", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@action", data.action));
                    cmd.Parameters.Add(new SqlParameter("@id_take_order", data.idTakeOrder));
                    cmd.Parameters.Add(new SqlParameter("@date", data.date));
                    cmd.Parameters.Add(new SqlParameter("@totalValue", data.totalValue));
                    cmd.Parameters.Add(new SqlParameter("@id_table", data.idTable));
                    cmd.Parameters.Add(new SqlParameter("@id_user", data.idUser));
                    cmd.Parameters.Add(new SqlParameter("@paid", data.paid));

                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    Dictionary<string, object> detail = new Dictionary<string, object>();

                    while (await sqldr.ReadAsync())
                    {
                        if (sqldr["id_order"] != DBNull.Value) { detail.Add("idOrder", Convert.ToInt32(sqldr["id_order"])); } else { detail.Add("idOrder", ""); }
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = r.Flag == true ? 200 : 400;

                    if (r.Flag)
                    {
                        oReply.Ok = true;
                        oReply.Message = r.Message;
                        oReply.Data = detail.Count > 0 ? detail : null;

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

        #region Crear o actualizar detalle
        // POST: api/createUpdateOrderProduct
        /// <summary>
        /// Crear/Actualizar detalle
        /// </summary>
        /// <remarks>
        /// Método que me permite crear/actualizar detalle registrado en el sistema
        /// </remarks>
        /// <param name="data">Data a validar en db</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpPost]
        [Route("createUpdateOrderProduct")]
        public async Task<IActionResult> createUpdateOrderProduct([FromBody] OrdenesProductosModel data)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_createUpdateOrderProduct", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id_order_product", data.idOrderProduct));
                    cmd.Parameters.Add(new SqlParameter("@id_take_order", data.idTakeOrder));
                    cmd.Parameters.Add(new SqlParameter("@id_product", data.idProduct));
                    cmd.Parameters.Add(new SqlParameter("@amount", data.amount));

                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    await cmd.ExecuteNonQueryAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = r.Flag == true ? 200 : 400;

                    if (r.Flag)
                    {
                        oReply.Ok = true;
                        oReply.Message = r.Message;
                        oReply.Data = null;

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
