using Microsoft.AspNetCore.Mvc;
using api_pns.Context;
using api_pns.Models;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Threading.Tasks;
using api_pns.Models.Users;

namespace api_pns.Controllers.Usuarios
{
    [Route("api")]
    [ApiController]
    public class UsuariosController : Controller
    {
        public Connection conn; 
        private readonly IConfiguration _configuration;
        public ReplySucess oReply = new ReplySucess();

        public UsuariosController(IConfiguration configuration)
        {
            _configuration = configuration;
            conn = new Connection();
        }

        #region Listar usuarios
        // GET: api/listUsers
        /// <summary>
        /// Listar usuarios
        /// </summary>
        /// <remarks>
        /// Método para consultar usuarios
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("listUsers")]
        public async Task<IActionResult> listUsers()
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_listUsers", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    List<Dictionary<string, object>> details = new List<Dictionary<string, object>>();

                    while (await sqldr.ReadAsync())
                    {
                        Dictionary<string, object> data = new Dictionary<string, object>();
                        if (sqldr["id_user"] != DBNull.Value) { data.Add("idUser", Convert.ToInt32(sqldr["id_user"])); } else { data.Add("idUser", ""); }
                        if (sqldr["type_document"] != DBNull.Value) { data.Add("typeDocument", sqldr["type_document"].ToString()); } else { data.Add("typeDocument", ""); }
                        if (sqldr["prefix"] != DBNull.Value) { data.Add("prefix", sqldr["prefix"].ToString()); } else { data.Add("prefix", ""); }
                        if (sqldr["document_number"] != DBNull.Value) { data.Add("documentNumber", sqldr["document_number"].ToString()); } else { data.Add("documentNumber", ""); }
                        if (sqldr["names"] != DBNull.Value) { data.Add("names", sqldr["names"].ToString()); } else { data.Add("names", ""); }
                        if (sqldr["surnames"] != DBNull.Value) { data.Add("surnames", sqldr["surnames"].ToString()); } else { data.Add("surnames", ""); }
                        if (sqldr["name_user"] != DBNull.Value) { data.Add("nameUser", sqldr["name_user"].ToString()); } else { data.Add("nameUser", ""); }
                        if (sqldr["birth_date"] != DBNull.Value) { data.Add("birthDate", Convert.ToDateTime(sqldr["birth_date"])); } else { data.Add("birthDate", ""); }
                        if (sqldr["name_conuntry"] != DBNull.Value) { data.Add("nameConuntry", sqldr["name_conuntry"].ToString()); } else { data.Add("nameConuntry", ""); }
                        if (sqldr["name_city"] != DBNull.Value) { data.Add("nameCity", sqldr["name_city"].ToString()); } else { data.Add("nameCity", ""); }
                        if (sqldr["name_role"] != DBNull.Value) { data.Add("nameRole", sqldr["name_role"].ToString()); } else { data.Add("nameRole", ""); }

                        details.Add(data);
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = r.Flag ? 200 : 400;


                    if (r.Flag)
                    {
                        r.Data = details;
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

        #region Detalle de usuarios
        // GET: api/consultUser/{idUser}
        /// <summary>
        /// Detalle usuario
        /// </summary>
        /// <remarks>
        /// Método para obtener detalle del usuario
        /// </remarks>
        /// <param name="idUser">Identificador del usuario</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("consultUser/{idUser}")]
        public async Task<IActionResult> consultUser([FromRoute] int idUser)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_consultUser", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id_user", idUser));
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    Dictionary<string, object> data = new Dictionary<string, object>();

                    while (await sqldr.ReadAsync())
                    {
                        if (sqldr["id_user"] != DBNull.Value) { data.Add("idUser", Convert.ToInt32(sqldr["id_user"])); } else { data.Add("idUser", ""); }
                        if (sqldr["id_document_type"] != DBNull.Value) { data.Add("idDocumentType", Convert.ToInt32(sqldr["id_document_type"])); } else { data.Add("idDocumentType", ""); }
                        if (sqldr["document_number"] != DBNull.Value) { data.Add("documentNumber", sqldr["document_number"].ToString()); } else { data.Add("documentNumber", ""); }
                        if (sqldr["names"] != DBNull.Value) { data.Add("names", sqldr["names"].ToString()); } else { data.Add("names", ""); }
                        if (sqldr["surnames"] != DBNull.Value) { data.Add("surnames", sqldr["surnames"].ToString()); } else { data.Add("surnames", ""); }
                        if (sqldr["name_user"] != DBNull.Value) { data.Add("nameUser", sqldr["name_user"].ToString()); } else { data.Add("nameUser", ""); }
                        if (sqldr["birth_date"] != DBNull.Value) { data.Add("birthDate", Convert.ToDateTime(sqldr["birth_date"])); } else { data.Add("birthDate", ""); }
                        if (sqldr["id_country"] != DBNull.Value) { data.Add("idCountry", Convert.ToInt32(sqldr["id_country"])); } else { data.Add("idCountry", ""); }
                        if (sqldr["id_city"] != DBNull.Value) { data.Add("idCity", Convert.ToInt32(sqldr["id_city"])); } else { data.Add("idCity", ""); }
                        if (sqldr["id_role"] != DBNull.Value) { data.Add("idRole", Convert.ToInt32(sqldr["id_role"])); } else { data.Add("idRole", ""); }
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = r.Flag ? 200 : 400;


                    if (r.Flag)
                    {
                        r.Data = data;

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

        #region Crear o actualizar usuario
        // POST: api/createUpdateUsers
        /// <summary>
        /// Crear/Actualizar usuario
        /// </summary>
        /// <remarks>
        /// Método para crear o actualizar usuario.
        /// </remarks>
        /// <param name="data">Data a validar en db</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpPost]
        [Route("createUpdateUsers")]
        public async Task<IActionResult> createUpdateCountry([FromBody] UsersModel data)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_createUpdateUsers", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id_user", data.idUser));
                    cmd.Parameters.Add(new SqlParameter("@document_number", data.documentNumber));
                    cmd.Parameters.Add(new SqlParameter("@names", data.names));
                    cmd.Parameters.Add(new SqlParameter("@surnames", data.surnames));
                    cmd.Parameters.Add(new SqlParameter("@name_user", data.nameUser));
                    cmd.Parameters.Add(new SqlParameter("@password", data.password));
                    cmd.Parameters.Add(new SqlParameter("@birth_date", data.birthDate));
                    cmd.Parameters.Add(new SqlParameter("@id_document_type", data.idDocumentType));
                    cmd.Parameters.Add(new SqlParameter("@id_country", data.idCountry));
                    cmd.Parameters.Add(new SqlParameter("@id_city", data.idCity));
                    cmd.Parameters.Add(new SqlParameter("@id_role", data.idRole));

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

        #region Actualizar contraseña
        // POST: api/updatePassword
        /// <summary>
        /// Actualizar contraseña de usuario
        /// </summary>
        /// <remarks>
        /// Método para actualizar contraseña de usuario.
        /// </remarks>
        /// <param name="data">Data a validar en db</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpPost]
        [Route("updatePassword")]
        public async Task<IActionResult> updatePassword([FromBody] UserPasswordModel data)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_updatePassword", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id_user", data.idUser));
                    cmd.Parameters.Add(new SqlParameter("@password", data.password));

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
