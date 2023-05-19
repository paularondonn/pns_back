using api_pns.Context;
using api_pns.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Threading.Tasks;
using api_pns.Models.Countries;

namespace api_pns.Controllers.Paises
{
    [Route("api")]
    [ApiController]
    public class PaisesController : ControllerBase
    {

        public Connection conn;
        private readonly IConfiguration _configuration;
        public ReplySucess oReply = new ReplySucess();

        public PaisesController(IConfiguration configuration)
        {
            _configuration = configuration;
            conn = new Connection();
        }

        #region Listar paises
        // GET: api/listCountries
        /// <summary>
        /// Listar paises
        /// </summary>
        /// <remarks>
        /// Método para consultar paises
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("listCountries")]
        public async Task<IActionResult> listCountries()
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_listCountries", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    List<CountriesModel> detailCountries = new List<CountriesModel>();

                    while (await sqldr.ReadAsync())
                    {
                        CountriesModel detailCountries2 = new CountriesModel();
                        if (sqldr["id_country"] != DBNull.Value) { detailCountries2.idCountry = Convert.ToInt32(sqldr["id_country"]); } else { detailCountries2.idCountry = 0; }
                        if (sqldr["name"] != DBNull.Value) { detailCountries2.name = sqldr["name"].ToString(); } else { detailCountries2.name = ""; }

                        detailCountries.Add(detailCountries2);
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = r.Flag ? 200 : 400;


                    if (r.Flag)
                    {
                        r.Data = detailCountries;
                        r.Message = "Successful countries";
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

        #region Detalle pais
        // GET: api/consultCountries/{idCountry}
        /// <summary>
        /// Detalle pais
        /// </summary>
        /// <remarks>
        /// Método para obtener detalle del pais
        /// </remarks>
        /// <param name="idCountry">Identificador del pais a consultar</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("consultCountries/{idCountry}")]
        public async Task<IActionResult> consultCountries([FromRoute] int idCountry)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_consultCountries", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id_country", idCountry));
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    CountriesModel detailCountries = new CountriesModel();

                    while (await sqldr.ReadAsync())
                    {
                        if (sqldr["id_country"] != DBNull.Value) { detailCountries.idCountry = Convert.ToInt32(sqldr["id_country"]); } else { detailCountries.idCountry = 0; }
                        if (sqldr["name"] != DBNull.Value) { detailCountries.name = sqldr["name"].ToString(); } else { detailCountries.name = ""; }
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = r.Flag ? 200 : 400;


                    if (r.Flag)
                    {
                        r.Data = detailCountries;
                        r.Message = "Successful countries";
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

        #region Crear o actualizar paises
        // POST: api/createUpdateCountry
        /// <summary>
        /// Crear/Actualizar pais
        /// </summary>
        /// <remarks>
        /// Método para crear o actualizar paises.
        /// </remarks>
        /// <param name="country">Codigo y nombre del país</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpPost]
        [Route("createUpdateCountry")]
        public async Task<IActionResult> createUpdateCountry([FromBody] CountriesCreateUpdateModel country)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_createUpdateCountry", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id_country", country.idCountry));
                    cmd.Parameters.Add(new SqlParameter("@name", country.name));
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    await cmd.ExecuteNonQueryAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = r.Flag == true ? 200 : 400;

                    if (r.Flag)
                    {
                        oReply.Ok = r.Flag;
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
