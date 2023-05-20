using api_pns.Context;
using api_pns.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System;
using api_pns.Models.Suppliers;
using Microsoft.AspNetCore.Rewrite;
using System.Runtime.InteropServices;

namespace api_pns.Controllers.Proveedores
{
    [Route("api")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        public Connection conn;
        private readonly IConfiguration _configuration;
        public ReplySucess oReply = new ReplySucess();

        public SuppliersController(IConfiguration configuration)
        {
            _configuration = configuration;
            conn = new Connection();
        }

        #region Listar proveedores
        // GET: api/listSuppliers
        /// <summary>
        /// Listar proveedores
        /// </summary>
        /// <remarks>
        /// Método para listar los proveedores
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("listSuppliers")]
        public async Task<IActionResult> listSuppliers()
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_listSuppliers", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    List<SuppliersModel> detailSuppliers = new List<SuppliersModel>();

                    while (await sqldr.ReadAsync())
                    {
                        SuppliersModel detailSuppliers2 = new SuppliersModel();
                        if (sqldr["id_suppliers"] != DBNull.Value) { detailSuppliers2.idSupplier = Convert.ToInt32(sqldr["id_suppliers"]); } else { detailSuppliers2.idSupplier = 0; }
                        if (sqldr["name"] != DBNull.Value) { detailSuppliers2.name = sqldr["name"].ToString(); } else { detailSuppliers2.name = ""; }
                        if (sqldr["nit"] != DBNull.Value) { detailSuppliers2.nit = sqldr["nit"].ToString(); } else { detailSuppliers2.nit = ""; }
                        if (sqldr["email"] != DBNull.Value) { detailSuppliers2.email = sqldr["email"].ToString(); } else { detailSuppliers2.email = ""; }
                        if (sqldr["telephone"] != DBNull.Value) { detailSuppliers2.telephone = sqldr["telephone"].ToString(); } else { detailSuppliers2.telephone = ""; }

                        detailSuppliers.Add(detailSuppliers2);
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = r.Flag ? 200 : 400;

                    if (r.Flag == true)
                    {
                        r.Data = detailSuppliers;
                        r.Message = "Successful Suppliers";
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

        #region Consultar detalle proveedores
        // GET: api/consultSuppliers/{idSupplier}
        /// <summary>
        /// Consultar detalle del proveedor 
        /// </summary>
        /// <remarks>
        /// Método para consultar proveedor
        /// </remarks>
        /// <param name="idSupplier">Identificador del proveedor a consultar</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("consultSuppliers/{idSupplier}")]
        public async Task<IActionResult> consultSuppliers([FromRoute] int idSupplier)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_consultSuppliers", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id_suppliers", idSupplier));
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    SuppliersConsultModel detailSuppliers = new SuppliersConsultModel();

                    while (await sqldr.ReadAsync())
                    {
                        if (sqldr["id_suppliers"] != DBNull.Value) { detailSuppliers.idSupplier = Convert.ToInt32(sqldr["id_suppliers"]); } else { detailSuppliers.idSupplier = 0; }
                        if (sqldr["nit"] != DBNull.Value) { detailSuppliers.nit = sqldr["nit"].ToString(); } else { detailSuppliers.nit = ""; }
                        if (sqldr["name"] != DBNull.Value) { detailSuppliers.name = sqldr["name"].ToString(); } else { detailSuppliers.name = ""; }
                        if (sqldr["email"] != DBNull.Value) { detailSuppliers.email = sqldr["email"].ToString(); } else { detailSuppliers.email = ""; }
                        if (sqldr["telephone"] != DBNull.Value) { detailSuppliers.telephone = sqldr["telephone"].ToString(); } else { detailSuppliers.telephone = ""; }
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = 400;


                    if (r.Flag)
                    {
                        r.Data = detailSuppliers;
                        r.Message = "Successful suppliers";
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

        #region Crear o actualizar proveedores
        // POST: api/createUpdateSupplier
        /// <summary>
        /// Crear/Actualizar proveedor
        /// </summary>
        /// <remarks>
        /// Método que permite crear/actualizar un proveedor
        /// </remarks>
        /// <param name="supplier">Codigo y nombre del proveedor, e identificador el producto asociado al proveedor</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpPost]
        [Route("createUpdateSupplier")]
        public async Task<IActionResult> createUpdateSupplier([FromBody] SuppliersCreateUpdateModel supplier)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_createUpdateSuppliers", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id_suppliers", supplier.idSupplier));
                    cmd.Parameters.Add(new SqlParameter("@nit", supplier.nit));
                    cmd.Parameters.Add(new SqlParameter("@name", supplier.name));
                    cmd.Parameters.Add(new SqlParameter("@email", supplier.email));
                    cmd.Parameters.Add(new SqlParameter("@telephone", supplier.telephone));
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    await cmd.ExecuteNonQueryAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = r.Flag == true ? 200 : 400;

                    if (r.Flag)
                    {
                        oReply.Ok = r.Flag == true ? true : false;
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