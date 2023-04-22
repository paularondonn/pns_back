using api_pns.Context;
using api_pns.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using api_pns.Models.Menu;

namespace api_pns.Controllers.Menu
{
    [Route("api")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        public Connection conn;
        private readonly IConfiguration _configuration;
        public ReplySucess oReply = new ReplySucess();

        public MenuController(IConfiguration configuration)
        {
            _configuration = configuration;
            conn = new Connection();
        }

        #region Menu
        // GET: api/Menu/{idRole}
        /// <summary>
        /// Menu
        /// </summary>
        /// <remarks>
        /// Método para consultar menu segun el rol del usuario
        /// </remarks>
        /// <param name="idRole">Rol del usuario</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("menu/{idRole}")]
        public async Task<IActionResult> Menu([FromRoute] int idRole)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_menu", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id_role", idRole));
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    List<MenuModel> detailMenu = new List<MenuModel>();

                    while (await sqldr.ReadAsync())
                    {
                        MenuModel detailMenu2 = new MenuModel();
                        if (sqldr["id_permission"] != DBNull.Value) { detailMenu2.idPermission = Convert.ToInt32(sqldr["id_permission"]); } else { detailMenu2.idPermission = 0; }
                        if (sqldr["id_role"] != DBNull.Value) { detailMenu2.idRole = Convert.ToInt32(sqldr["id_role"]); } else { detailMenu2.idRole = 0; }
                        if (sqldr["id_menu"] != DBNull.Value) { detailMenu2.idMenu = Convert.ToInt32(sqldr["id_menu"]); } else { detailMenu2.idMenu = 0; }
                        if (sqldr["name"] != DBNull.Value) { detailMenu2.name = sqldr["name"].ToString(); } else { detailMenu2.name = ""; }
                        if (sqldr["permission"] != DBNull.Value) { detailMenu2.permission = Convert.ToBoolean(sqldr["permission"]); } else { detailMenu2.permission = false; }

                        detailMenu.Add(detailMenu2);
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = 400;


                    if (r.Flag)
                    {
                        Dictionary<string, object> detailsMenu = new Dictionary<string, object>();
                        detailsMenu.Add("menu", detailMenu);

                        r.Data = detailsMenu;
                        r.Message = "Successful menu";
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
    }
}
