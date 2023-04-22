using api_pns.Context;
using api_pns.Models;
using api_pns.Models.Cities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System;
using api_pns.Models.Tables;

namespace api_pns.Controllers.Mesas
{
    [Route("api")]
    [ApiController]
    public class MesasController : Controller
    {
        public Connection conn;
        private readonly IConfiguration _configuration;
        public ReplySucess oReply = new ReplySucess();

        public MesasController(IConfiguration configuration)
        {
            _configuration = configuration;
            conn = new Connection();
        }

        #region Listar mesas
        // GET: api/listTables
        /// <summary>
        /// Listar mesas
        /// </summary>
        /// <remarks>
        /// Método para listar las mesas
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("listTables")]
        public async Task<IActionResult> listTables()
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_listTables", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    List<TablesModel> detailCities = new List<TablesModel>();

                    while (await sqldr.ReadAsync())
                    {
                        TablesModel detailCities2 = new TablesModel();
                        if (sqldr["id_table"] != DBNull.Value) { detailCities2.idTable = Convert.ToInt32(sqldr["id_table"]); } else { detailCities2.idTable = 0; }
                        if (sqldr["country_name"] != DBNull.Value) { detailCities2.nameCountry = sqldr["country_name"].ToString(); } else { detailCities2.nameCountry = ""; }
                        if (sqldr["city_name"] != DBNull.Value) { detailCities2.nameCity = sqldr["city_name"].ToString(); } else { detailCities2.nameCity = ""; }
                        if (sqldr["headquarter_name"] != DBNull.Value) { detailCities2.nameHeadquarters = sqldr["headquarter_name"].ToString(); } else { detailCities2.nameHeadquarters = ""; }
                        if (sqldr["name"] != DBNull.Value) { detailCities2.name = sqldr["name"].ToString(); } else { detailCities2.name = ""; }

                        detailCities.Add(detailCities2);
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = 400;


                    if (r.Flag == true)
                    {
                        r.Data = detailCities;
                        r.Status = 200;

                        oReply.Ok = true;
                        oReply.Message = r.Message;
                        oReply.Data = r.Data;

                        return Ok(oReply);
                    }


                    oReply.Ok = false;
                    oReply.Message = r.Message;
                    oReply.Data = null;

                    return Ok(oReply);
                }
                catch (Exception ex)
                {
                    oReply.Ok = false;
                    oReply.Message = r.Message;
                    oReply.Data = null;
                    return BadRequest(oReply);
                }
            }
        }
        #endregion

        #region Consultar mesas segun la sede
        // GET: api/listTablesHeadquarters/{idHeadquarters}
        /// <summary>
        /// Consultar ciudades
        /// </summary>
        /// <remarks>
        /// Método para consultar las ciudades segun el pais
        /// </remarks>
        /// <param name="idHeadquarters">Identificador de la sede para consultar mesas</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("listTablesHeadquarters/{idHeadquarters}")]
        public async Task<IActionResult> consultCitiesCountry([FromRoute] int idHeadquarters)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_listTablesHeadquarters", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id_headquarters", idHeadquarters));
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    List<TablesConsultModel> detail = new List<TablesConsultModel>();

                    while (await sqldr.ReadAsync())
                    {
                        TablesConsultModel detail2 = new TablesConsultModel();
                        if (sqldr["id_table"] != DBNull.Value) { detail2.idTable = Convert.ToInt32(sqldr["id_table"]); } else { detail2.idTable = 0; }
                        if (sqldr["name"] != DBNull.Value) { detail2.name = sqldr["name"].ToString(); } else { detail2.name = ""; }

                        detail.Add(detail2);
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = 400;


                    if (r.Flag)
                    {
                        r.Data = detail;
                        r.Status = 200;

                        oReply.Ok = true;
                        oReply.Message = r.Message;
                        oReply.Data = r.Data;

                        return Ok(oReply);
                    }


                    oReply.Ok = false;
                    oReply.Message = r.Message;
                    oReply.Data = null;

                    return Ok(oReply);
                }
                catch (Exception ex)
                {
                    oReply.Ok = false;
                    oReply.Message = r.Message;
                    oReply.Data = null;
                    return BadRequest(oReply);
                }
            }
        }
        #endregion

        #region Consultar detalle mesa
        // GET: api/consultTable/{idTable}
        /// <summary>
        /// Consultar detalle de la ciudad
        /// </summary>
        /// <remarks>
        /// Método para consultar ciudad
        /// </remarks>
        /// <param name="idCity">Identificador de la ciudad a consultar</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("consultTable/{idTable}")]
        public async Task<IActionResult> consultTable([FromRoute] int idTable)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_detailTables", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id_table", idTable));
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    TablesConsultModel detail = new TablesConsultModel();

                    while (await sqldr.ReadAsync())
                    {
                        if (sqldr["id_country"] != DBNull.Value) { detail.idCountry = Convert.ToInt32(sqldr["id_country"]); } else { detail.idCountry = 0; }
                        if (sqldr["id_city"] != DBNull.Value) { detail.idCity = Convert.ToInt32(sqldr["id_city"]); } else { detail.idCity = 0; }
                        if (sqldr["id_headquarters"] != DBNull.Value) { detail.idHeadquarters = Convert.ToInt32(sqldr["id_headquarters"]); } else { detail.idHeadquarters = 0; }
                        if (sqldr["name"] != DBNull.Value) { detail.name = sqldr["name"].ToString(); } else { detail.name = ""; }
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = 400;


                    if (r.Flag)
                    {
                        r.Data = detail;
                        r.Status = 200;

                        oReply.Ok = true;
                        oReply.Message = r.Message;
                        oReply.Data = r.Data;

                        return Ok(oReply);
                    }


                    oReply.Ok = false;
                    oReply.Message = r.Message;
                    oReply.Data = null;

                    return Ok(oReply);
                }
                catch (Exception ex)
                {
                    oReply.Ok = false;
                    oReply.Message = r.Message;
                    oReply.Data = null;
                    return BadRequest(oReply);
                }
            }
        }
        #endregion

        #region Crear o actualizar ciudades
        // POST: api/createUpdateTable
        /// <summary>
        /// Crear/Actualizar mesa
        /// </summary>
        /// <remarks>
        /// Método que me permite crear/actualizar las mesas registradas en el sistema
        /// </remarks>
        /// <param name="data">Sede y nombre de la mesa</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpPost]
        [Route("createUpdateTable")]
        public async Task<IActionResult> createUpdateTable([FromBody] TablesCreateUpdateModel data)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_createUpdateTables", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id_table", data.idTable));
                    cmd.Parameters.Add(new SqlParameter("@id_headquarters", data.idHeadquarters));
                    cmd.Parameters.Add(new SqlParameter("@name", data.name));
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    await cmd.ExecuteNonQueryAsync();


                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = r.Flag == true ? 200 : 400;

                    oReply.Ok = r.Flag == true ? true : false;
                    oReply.Message = r.Message;
                    oReply.Data = null;

                    return Ok(oReply);
                }
                catch (Exception ex)
                {
                    oReply.Ok = false;
                    oReply.Message = r.Message;
                    oReply.Data = null;
                    return BadRequest(oReply);
                }
            }
        }
        #endregion
    }
}
