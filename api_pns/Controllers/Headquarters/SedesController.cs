using api_pns.Context;
using api_pns.Models.Tables;
using api_pns.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System;
using api_pns.Models.Headquarters;

namespace api_pns.Controllers.Headquarters
{
    [Route("api")]
    [ApiController]
    public class SedesController : ControllerBase
    {
        public Connection conn;
        private readonly IConfiguration _configuration;
        public ReplySucess oReply = new ReplySucess();

        public SedesController(IConfiguration configuration)
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

                    List<HeadquartersModel> detail = new List<HeadquartersModel>();

                    while (await sqldr.ReadAsync())
                    {
                        HeadquartersModel detail2 = new HeadquartersModel();
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
                    oReply.Message = r.Message;
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
                    oReply.Message = r.Message;
                    oReply.Data = null;
                    return BadRequest(oReply);
                }
            }
        }
        #endregion
    }
}
