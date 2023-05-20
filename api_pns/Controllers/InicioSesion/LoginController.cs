using api_pns.Models.Login;
using api_pns.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Threading.Tasks;
using api_pns.Context;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Rewrite;

namespace api_pns.Controllers.InicioSesion
{

    [ApiController]
    public class LoginController : ControllerBase
    {
        public Connection conn;
        private readonly IConfiguration _configuration;

        public ReplySucess oReply = new ReplySucess();

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
            conn = new Connection();
        }

        #region Metodo para iniciar sesión
        // POST: api/Login
        /// <summary>
        /// Iniciar sesión
        /// </summary>
        /// <remarks>
        /// Método que me permite iniciar sesión e ingresar al sistema, este devuelve el token solicitado por los servicios en su respuesta
        /// </remarks>
        /// <param name="login">Usuario, contraseña y centro de atención asociado al usuario</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpPost]
        [Route("api/Login")]
        public async Task<IActionResult> Login(LoginModel login)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_login", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@name_user", login.name_user));
                    cmd.Parameters.Add(new SqlParameter("@password", login.password));
                    cmd.Parameters.Add(new SqlParameter("@id_headquarters", login.id_headquarters));
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    DetailsUserLoginReplyModel detailsUser = new DetailsUserLoginReplyModel();

                    while (await sqldr.ReadAsync())
                    {
                        if (sqldr["id_user"] != DBNull.Value) { detailsUser.idUser = Convert.ToInt32(sqldr["id_user"]); } else { detailsUser.idUser = 0; }
                        if (sqldr["id_role"] != DBNull.Value) { detailsUser.idRole = Convert.ToInt32(sqldr["id_role"]); } else { detailsUser.idRole = 0; }
                        if (sqldr["names"] != DBNull.Value) { detailsUser.Name = sqldr["names"].ToString(); } else { detailsUser.Name = ""; }
                        if (sqldr["surnames"] != DBNull.Value) { detailsUser.LastName = sqldr["surnames"].ToString(); } else { detailsUser.LastName = ""; }
                        if (sqldr["name_user"] != DBNull.Value) { detailsUser.UserName = sqldr["name_user"].ToString(); } else { detailsUser.UserName = ""; }
                        if (sqldr["id_headquarters"] != DBNull.Value) { detailsUser.idHeadquarters = Convert.ToInt32(sqldr["id_headquarters"]); } else { detailsUser.idHeadquarters = 0; }
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = r.Flag ? 200 : 400;


                    if (r.Flag)
                    {
                        Dictionary<string, object> detailsLogin = new Dictionary<string, object>();
                        detailsLogin.Add("name", detailsUser.Name + " " + detailsUser.LastName);
                        detailsLogin.Add("idUser", detailsUser.idUser);
                        detailsLogin.Add("idRole", detailsUser.idRole);
                        detailsLogin.Add("idHeadquarters", detailsUser.idHeadquarters);
                        detailsLogin.Add("UserName", detailsUser.UserName);

                        r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                        r.Flag = (bool)cmd.Parameters["@flag"].Value;
                        r.Status = 200;
                        r.Data = detailsLogin;

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

        #region Metodo para validar usuario
        // GET: api/validUserName/{name_user}
        /// <summary>
        /// Menu
        /// </summary>
        /// <remarks>
        /// Método para validar nombre de usuario
        /// </remarks>
        /// <param name="name_user">Nombre de usuario</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("api/validUserName/{name_user}")]
        public async Task<IActionResult> validUserName([FromRoute] string name_user)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_validUserName", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@name_user", name_user));
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    List<LoginValidUserNameModel> detailValidUserName = new List<LoginValidUserNameModel>();

                    while (await sqldr.ReadAsync())
                    {
                        LoginValidUserNameModel detailValidUserName2 = new LoginValidUserNameModel();
                        if (sqldr["id_sities"] != DBNull.Value) { detailValidUserName2.idSities = Convert.ToInt32(sqldr["id_sities"]); } else { detailValidUserName2.idSities = 0; }
                        if (sqldr["id_headquarters"] != DBNull.Value) { detailValidUserName2.idHeadquarters = Convert.ToInt32(sqldr["id_headquarters"]); } else { detailValidUserName2.idHeadquarters = 0; }
                        if (sqldr["name"] != DBNull.Value) { detailValidUserName2.name = sqldr["name"].ToString(); } else { detailValidUserName2.name = ""; }

                        detailValidUserName.Add(detailValidUserName2);
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = r.Flag ? 200 : 400;


                    if (r.Flag)
                    {
                        r.Data = detailValidUserName;
                        r.Message = "Successful valid";
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
