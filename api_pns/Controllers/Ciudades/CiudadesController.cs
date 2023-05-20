using api_pns.Context;
using api_pns.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System;
using api_pns.Models.Cities;
using Microsoft.AspNetCore.Rewrite;

namespace api_pns.Controllers.Ciudades
{
    [Route("api")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        public Connection conn;
        private readonly IConfiguration _configuration;
        public ReplySucess oReply = new ReplySucess();

        public CitiesController(IConfiguration configuration)
        {
            _configuration = configuration;
            conn = new Connection();
        }

        #region Listar ciudades
        // GET: api/listCities
        /// <summary>
        /// Listar ciudades
        /// </summary>
        /// <remarks>
        /// Método para listar las ciudades
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("listCities")]
        public async Task<IActionResult> listCities()
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_listCities", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    List<CitiesModel> detailCities = new List<CitiesModel>();

                    while (await sqldr.ReadAsync())
                    {
                        CitiesModel detailCities2 = new CitiesModel();
                        if (sqldr["id_city"] != DBNull.Value) { detailCities2.idCity = Convert.ToInt32(sqldr["id_city"]); } else { detailCities2.idCity = 0; }
                        if (sqldr["name"] != DBNull.Value) { detailCities2.name = sqldr["name"].ToString(); } else { detailCities2.name = ""; }
                        if (sqldr["country_name"] != DBNull.Value) { detailCities2.countryName = sqldr["country_name"].ToString(); } else { detailCities2.countryName = ""; }

                        detailCities.Add(detailCities2);
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = r.Flag ? 200 : 400;

                    if (r.Flag == true)
                    {
                        r.Data = detailCities;
                        r.Message = "Successful cities";
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

        #region Consultar ciudades segun el pais
        // GET: api/consultCitiesCountry/{idCountry}
        /// <summary>
        /// Consultar ciudades
        /// </summary>
        /// <remarks>
        /// Método para consultar las ciudades segun el pais
        /// </remarks>
        /// <param name="idCountry">Identificador del pais para consultar ciudades</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("consultCitiesCountry/{idCountry}")]
        public async Task<IActionResult> consultCitiesCountry([FromRoute] int idCountry)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_consultCitiesCountry", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id_country", idCountry));
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    List<CitiesConsultModel> detailCities = new List<CitiesConsultModel>();

                    while (await sqldr.ReadAsync())
                    {
                        CitiesConsultModel detailCities2 = new CitiesConsultModel();
                        if (sqldr["id_city"] != DBNull.Value) { detailCities2.idCity = Convert.ToInt32(sqldr["id_city"]); } else { detailCities2.idCity = 0; }
                        if (sqldr["name"] != DBNull.Value) { detailCities2.name = sqldr["name"].ToString(); } else { detailCities2.name = ""; }
                        if (sqldr["id_country"] != DBNull.Value) { detailCities2.idCountry = Convert.ToInt32(sqldr["id_country"]); } else { detailCities2.idCountry = 0; }

                        detailCities.Add(detailCities2);
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = 400;

                    if (r.Flag)
                    {
                        r.Data = detailCities;
                        r.Message = "Successful cities";
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

        #region Consultar detalle ciudades
        // GET: api/consultCities/{idCity}
        /// <summary>
        /// Consultar detalle de la ciudad
        /// </summary>
        /// <remarks>
        /// Método para consultar ciudad
        /// </remarks>
        /// <param name="idCity">Identificador de la ciudad a consultar</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("consultCities/{idCity}")]
        public async Task<IActionResult> consultCities([FromRoute] int idCity)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_consultCity", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id_city", idCity));
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    CitiesConsultModel detailCities = new CitiesConsultModel();

                    while (await sqldr.ReadAsync())
                    {
                        if (sqldr["id_city"] != DBNull.Value) { detailCities.idCity = Convert.ToInt32(sqldr["id_city"]); } else { detailCities.idCity = 0; }
                        if (sqldr["name"] != DBNull.Value) { detailCities.name = sqldr["name"].ToString(); } else { detailCities.name = ""; }
                        if (sqldr["id_country"] != DBNull.Value) { detailCities.idCountry = Convert.ToInt32(sqldr["id_country"]); } else { detailCities.idCountry = 0; }
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = 400;


                    if (r.Flag)
                    {
                        r.Data = detailCities;
                        r.Message = "Successful cities";
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

        #region Crear o actualizar ciudades
        // POST: api/createUpdateCity
        /// <summary>
        /// Crear/Actualizar ciudad
        /// </summary>
        /// <remarks>
        /// Método que permite crear/actualizar una ciudad
        /// </remarks>
        /// <param name="city">Codigo y nombre de la ciudad, e identificador del pais asociado a la ciudad</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpPost]
        [Route("createUpdateCity")]
        public async Task<IActionResult> createUpdateCity([FromBody] CitiesCreateUpdateModel city)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_createUpdateCity", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id_city", city.idCity));
                    cmd.Parameters.Add(new SqlParameter("@id_country", city.idCountry));
                    cmd.Parameters.Add(new SqlParameter("@name", city.name));
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
