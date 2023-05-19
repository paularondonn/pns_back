using api_pns.Context;
using api_pns.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Security.Policy;
using System;
using System.Threading.Tasks;
using api_pns.Models.Reportes;
using api_pns.Models.Excel;

namespace api_pns.Controllers.Reportes
{
    [Route("api")]
    [ApiController]
    public class ReportesController : Controller
    {
        public Connection conn;
        private readonly IConfiguration _configuration;
        public ReplySucess oReply = new ReplySucess();

        public ReportesController(IConfiguration configuration)
        {
            _configuration = configuration;
            conn = new Connection();
        }

        #region Generar - Reporte excel
        // POST: api/reportExcel
        /// <summary>
        /// Generar - Reporte excel de facturación pendiente
        /// </summary>
        /// <remarks>
        /// Método para obtener reporte de facturación pendiente según ID de contratos.
        /// </remarks>
        /// <param name="request">Define la fecha inicia, final ID´s de contratos</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        /// <response code="400">Bad Request. url no encontrada</response>
        /// <response code="200">OK. Transacción realizada exitosamente</response>
        [HttpPost]
        [Route("reportExcel")]
        public async Task<IActionResult> Report([FromBody] ReportesModel data)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();
                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_reports", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@action", data.action));
                    cmd.Parameters.Add(new SqlParameter("@dateInitial", data.initialDate.ToString("yyyy-MM-dd") + " 00:00:00"));
                    cmd.Parameters.Add(new SqlParameter("@dateFinal", data.finalDate.ToString("yyyy-MM-dd") + " 23:59:59"));
                    cmd.Parameters.Add(new SqlParameter("@idSede", data.idSede));

                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = cmd.ExecuteReader();

                    if (sqldr.HasRows)
                    {
                        List<Dictionary<string, object>> listReport = new List<Dictionary<string, object>>();

                        while (await sqldr.ReadAsync())
                        {
                            if (data.action == 1)
                            {
                                Dictionary<string, object> row = new Dictionary<string, object>
                                {
                                    { "idTakeOrder", Convert.ToInt32(!Convert.IsDBNull(sqldr["id_take_order"]) ? sqldr["id_take_order"] : null) },
                                    { "idProduct", Convert.ToInt32(!Convert.IsDBNull(sqldr["id_product"]) ? sqldr["id_product"] : null) },
                                    { "name", Convert.ToString(!Convert.IsDBNull(sqldr["name"]) ? sqldr["name"] : null) },
                                    { "date", Convert.ToString(!Convert.IsDBNull(sqldr["date"]) ? sqldr["date"] : null) }
                                };

                                listReport.Add(row);
                            }
                            else if (data.action == 2)
                            {
                                Dictionary<string, object> row = new Dictionary<string, object>
                                {
                                    { "idTakeOrder", Convert.ToInt32(!Convert.IsDBNull(sqldr["id_take_order"]) ? sqldr["id_take_order"] : null) },
                                    { "idProduct", Convert.ToInt32(!Convert.IsDBNull(sqldr["id_product"]) ? sqldr["id_product"] : null) },
                                    { "name", Convert.ToString(!Convert.IsDBNull(sqldr["name"]) ? sqldr["name"] : null) },
                                    { "date", Convert.ToString(!Convert.IsDBNull(sqldr["date"]) ? sqldr["date"] : null) },
                                    { "amount", Convert.ToString(!Convert.IsDBNull(sqldr["amount"]) ? sqldr["amount"] : null) }
                                };

                                listReport.Add(row);
                            }
                            else if (data.action == 3)
                            {
                                Dictionary<string, object> row = new Dictionary<string, object>
                                {
                                    { "idTakeOrder", Convert.ToInt32(!Convert.IsDBNull(sqldr["id_take_order"]) ? sqldr["id_take_order"] : null) },
                                    { "idProduct", Convert.ToInt32(!Convert.IsDBNull(sqldr["id_product"]) ? sqldr["id_product"] : null) },
                                    { "name", Convert.ToString(!Convert.IsDBNull(sqldr["name"]) ? sqldr["name"] : null) },
                                    { "date", Convert.ToString(!Convert.IsDBNull(sqldr["date"]) ? sqldr["date"] : null) },
                                    { "amount", Convert.ToString(!Convert.IsDBNull(sqldr["amount"]) ? sqldr["amount"] : null) },
                                    { "totalValue", Convert.ToString(!Convert.IsDBNull(sqldr["totalValue"]) ? sqldr["totalValue"] : null) }
                                };

                                listReport.Add(row);
                            }
                            else if (data.action == 4)
                            {
                                Dictionary<string, object> row = new Dictionary<string, object>
                                {
                                    { "idTakeOrder", Convert.ToInt32(!Convert.IsDBNull(sqldr["id_take_order"]) ? sqldr["id_take_order"] : null) },
                                    { "date", Convert.ToString(!Convert.IsDBNull(sqldr["date"]) ? sqldr["date"] : null) },
                                    { "totalValue", Convert.ToString(!Convert.IsDBNull(sqldr["totalValue"]) ? sqldr["totalValue"] : null) },
                                    { "idProduct", Convert.ToInt32(!Convert.IsDBNull(sqldr["id_product"]) ? sqldr["id_product"] : null) },
                                    { "nameProduct", Convert.ToString(!Convert.IsDBNull(sqldr["name_product"]) ? sqldr["name_product"] : null) },
                                    { "amount", Convert.ToString(!Convert.IsDBNull(sqldr["amount"]) ? sqldr["amount"] : null) },
                                    { "idTable", Convert.ToInt32(!Convert.IsDBNull(sqldr["id_table"]) ? sqldr["id_table"] : null) },
                                    { "nameTable", Convert.ToString(!Convert.IsDBNull(sqldr["name_table"]) ? sqldr["name_table"] : null) },
                                    { "idSede", Convert.ToInt32(!Convert.IsDBNull(sqldr["id_headquarters"]) ? sqldr["id_headquarters"] : null) },
                                    { "nameSede", Convert.ToString(!Convert.IsDBNull(sqldr["name_sede"]) ? sqldr["name_sede"] : null) }
                                };

                                listReport.Add(row);
                            }
                            else
                            {
                                Dictionary<string, object> row = new Dictionary<string, object>
                                {
                                    { "idProduct", Convert.ToInt32(!Convert.IsDBNull(sqldr["id_product"]) ? sqldr["id_product"] : null) },
                                    { "name", Convert.ToString(!Convert.IsDBNull(sqldr["name"]) ? sqldr["name"] : null) },
                                    { "price", Convert.ToString(!Convert.IsDBNull(sqldr["price"]) ? sqldr["price"] : null) },
                                    { "amount", Convert.ToString(!Convert.IsDBNull(sqldr["amount"]) ? sqldr["amount"] : null) }
                                };

                                listReport.Add(row);
                            }
                        }


                        await sqldr.CloseAsync();

                        // Creación de tabla para creación de informe
                        DataTable tableData = new DataTable();

                        if (data.action == 1)
                        {
                            tableData.Columns.Add("NÚMERO DE ORDEN", typeof(int));
                            tableData.Columns.Add("CÓDIGO DE PRODUCTO", typeof(int));
                            tableData.Columns.Add("PRODUCTO", typeof(string));
                            tableData.Columns.Add("FECHA DE LA ORDEN", typeof(string));

                            foreach (var item in listReport)
                            {
                                tableData.Rows.Add(
                                    item["idTakeOrder"],
                                    item["idProduct"],
                                    item["name"],
                                    item["date"]
                                    );
                            }
                        }
                        else if (data.action == 2)
                        {
                            tableData.Columns.Add("NÚMERO DE ORDEN", typeof(int));
                            tableData.Columns.Add("CÓDIGO DE PRODUCTO", typeof(int));
                            tableData.Columns.Add("PRODUCTO", typeof(string));
                            tableData.Columns.Add("FECHA DE LA ORDEN", typeof(string));
                            tableData.Columns.Add("CANTIDAD DE PRODUCTOS", typeof(string));

                            foreach (var item in listReport)
                            {
                                tableData.Rows.Add(
                                    item["idTakeOrder"],
                                    item["idProduct"],
                                    item["name"],
                                    item["date"],
                                    item["amount"]
                                    );
                            }
                        }
                        else if (data.action == 3)
                        {
                            tableData.Columns.Add("NÚMERO DE ORDEN", typeof(int));
                            tableData.Columns.Add("CÓDIGO DE PRODUCTO", typeof(int));
                            tableData.Columns.Add("PRODUCTO", typeof(string));
                            tableData.Columns.Add("FECHA DE LA ORDEN", typeof(string));
                            tableData.Columns.Add("CANTIDAD DE PRODUCTOS", typeof(string));
                            tableData.Columns.Add("VALOR TOTAL", typeof(string));

                            foreach (var item in listReport)
                            {
                                tableData.Rows.Add(
                                    item["idTakeOrder"],
                                    item["idProduct"],
                                    item["name"],
                                    item["date"],
                                    item["amount"],
                                    item["totalValue"]
                                    );
                            }
                        }
                        else if (data.action == 4)
                        {
                            tableData.Columns.Add("NÚMERO DE ORDEN", typeof(int));
                            tableData.Columns.Add("FECHA DE LA ORDEN", typeof(string));
                            tableData.Columns.Add("VALOR TOTAL", typeof(string));
                            tableData.Columns.Add("CÓDIGO DE PRODUCTO", typeof(int));
                            tableData.Columns.Add("PRODUCTO", typeof(string));
                            tableData.Columns.Add("CANTIDAD DE PRODUCTOS", typeof(string));
                            tableData.Columns.Add("CÓDIGO DE MESA", typeof(string));
                            tableData.Columns.Add("MESA", typeof(string));
                            tableData.Columns.Add("CÓDIGO DE SEDE", typeof(string));
                            tableData.Columns.Add("SEDE", typeof(string));

                            foreach (var item in listReport)
                            {
                                tableData.Rows.Add(
                                    item["idTakeOrder"],
                                    item["date"],
                                    item["totalValue"],
                                    item["idProduct"],
                                    item["nameProduct"],
                                    item["amount"],
                                    item["idTable"],
                                    item["nameTable"],
                                    item["idSede"],
                                    item["nameSede"]
                                    );
                            }
                        }
                        else
                        {
                            tableData.Columns.Add("CÓDIGO DE PRODUCTO", typeof(int));
                            tableData.Columns.Add("PRODUCTO", typeof(string));
                            tableData.Columns.Add("PRECIO", typeof(string));
                            tableData.Columns.Add("CANTIDAD DE PRODUCTOS", typeof(string));

                            foreach (var item in listReport)
                            {
                                tableData.Rows.Add(
                                    item["idProduct"],
                                    item["name"],
                                    item["price"],
                                    item["amount"]);
                            }
                        }
                        // Información de filtros
                        // Información de reporte
                        Dictionary<string, object> dataReport = new Dictionary<string, object>
                        {
                            { "reportTitle", "REPORTE" },
                            { "sheetName", "REPORTE" },
                            { "startDate", data.initialDate.ToString("yyyy-MM-dd")},
                            { "endDate", data.finalDate.ToString("yyyy-MM-dd")}
                        };

                        // Generar reporte para devolver en base64
                        ReporteExcel file = new ReporteExcel();
                        var reportBase64 = file.CreateSheet(tableData);

                        r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                        r.Flag = (bool)cmd.Parameters["@flag"].Value;
                        r.Status = r.Flag ? 200 : 400;


                        if (r.Flag == true)
                        {
                            r.Data = reportBase64;

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

                        return Ok(oReply);
                    }
                    else
                    {
                        oReply.Ok = false;
                        oReply.Message = "No hay registros";
                        oReply.Data = null;
                        return BadRequest(oReply);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
        #endregion
    }
}
