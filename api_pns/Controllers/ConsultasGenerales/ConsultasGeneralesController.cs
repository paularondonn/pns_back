using api_pns.Context;
using api_pns.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System;
using api_pns.Models.Consults;

namespace api_pns.Controllers.ConsultasGenerales
{
    [Route("api")]
    [ApiController]
    public class ConsultasGeneralesController : ControllerBase
    {
        public Connection conn;
        private readonly IConfiguration _configuration;
        public ReplySucess oReply = new ReplySucess();

        public ConsultasGeneralesController(IConfiguration configuration)
        {
            _configuration = configuration;
            conn = new Connection();
        }

        #region Listar sedes
        // GET: api/listHeadquarters
        /// <summary>
        /// Listar sedes
        /// </summary>
        /// <remarks>
        /// Método para listar las sedes
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("listHeadquarters")]
        public async Task<IActionResult> listHeadquarters()
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_listHeadquarters", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    List<ConsultsModel> detail = new List<ConsultsModel>();

                    while (await sqldr.ReadAsync())
                    {
                        ConsultsModel detail2 = new ConsultsModel();
                        if (sqldr["id_headquarters"] != DBNull.Value) { detail2.idHeadquarters = Convert.ToInt32(sqldr["id_headquarters"]); } else { detail2.idHeadquarters = 0; }
                        if (sqldr["country_name"] != DBNull.Value) { detail2.countryName = sqldr["country_name"].ToString(); } else { detail2.countryName = ""; }
                        if (sqldr["city_name"] != DBNull.Value) { detail2.cityName = sqldr["city_name"].ToString(); } else { detail2.cityName = ""; }
                        if (sqldr["name"] != DBNull.Value) { detail2.name = sqldr["name"].ToString(); } else { detail2.name = ""; }

                        detail.Add(detail2);
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = 400;


                    if (r.Flag == true)
                    {
                        r.Data = detail;
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

        #region Listar sedes por ciudad
        // GET: api/listHeadquartersCity/{idCity}
        /// <summary>
        /// Listar sedes
        /// </summary>
        /// <remarks>
        /// Método para listar las sedes
        /// </remarks>
        /// <param name="idCity">Identificador de la ciudad para consultar sedes</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("listHeadquartersCity/{idCity}")]
        public async Task<IActionResult> listHeadquartersCity([FromRoute] int idCity)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_listHeadquartersCity", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id_city", idCity));
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    List<HeadquartersListModel> detail = new List<HeadquartersListModel>();

                    while (await sqldr.ReadAsync())
                    {
                        HeadquartersListModel detail2 = new HeadquartersListModel();
                        if (sqldr["id_headquarters"] != DBNull.Value) { detail2.idHeadquarters = Convert.ToInt32(sqldr["id_headquarters"]); } else { detail2.idHeadquarters = 0; }
                        if (sqldr["name"] != DBNull.Value) { detail2.name = sqldr["name"].ToString(); } else { detail2.name = ""; }

                        detail.Add(detail2);
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = 400;


                    if (r.Flag == true)
                    {
                        r.Data = detail;
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

        #region Listar roles
        // GET: api/listRoles
        /// <summary>
        /// Listar roles
        /// </summary>
        /// <remarks>
        /// Método para listar roles
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("listRoles")]
        public async Task<IActionResult> listRoles()
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_listRoles", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    List<Dictionary<string, object>> details = new List<Dictionary<string, object>>();

                    while (await sqldr.ReadAsync())
                    {
                        Dictionary<string, object> data = new Dictionary<string, object>();
                        if (sqldr["id_role"] != DBNull.Value) { data.Add("idRole", Convert.ToInt32(sqldr["id_role"])); } else { data.Add("idRole", ""); }
                        if (sqldr["name"] != DBNull.Value) { data.Add("name", sqldr["name"].ToString()); } else { data.Add("name", ""); }

                        details.Add(data);
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = 400;


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

        #region Listar tipos de documento
        // GET: api/listDocumentType
        /// <summary>
        /// Listar tipos de documento
        /// </summary>
        /// <remarks>
        /// Método para listar tipos de documento
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("listDocumentType")]
        public async Task<IActionResult> listDocumentType()
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_listDocumentType", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    List<Dictionary<string, object>> details = new List<Dictionary<string, object>>();

                    while (await sqldr.ReadAsync())
                    {
                        Dictionary<string, object> data = new Dictionary<string, object>();
                        if (sqldr["id_document_type"] != DBNull.Value) { data.Add("idDocumentType", Convert.ToInt32(sqldr["id_document_type"])); } else { data.Add("idDocumentType", ""); }
                        if (sqldr["prefix"] != DBNull.Value) { data.Add("prefix", sqldr["prefix"].ToString()); } else { data.Add("prefix", ""); }
                        if (sqldr["name"] != DBNull.Value) { data.Add("name", sqldr["name"].ToString()); } else { data.Add("name", ""); }

                        details.Add(data);
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = 400;


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
