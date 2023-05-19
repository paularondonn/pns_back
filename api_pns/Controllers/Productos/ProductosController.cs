using api_pns.Context;
using api_pns.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System;
using api_pns.Models.Products;
using Microsoft.AspNetCore.Rewrite;
using System.Runtime.InteropServices;

namespace api_pns.Controllers.Productos
{
    [Route("api")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        public Connection conn;
        private readonly IConfiguration _configuration;
        public ReplySucess oReply = new ReplySucess();

        public ProductsController(IConfiguration configuration)
        {
            _configuration = configuration;
            conn = new Connection();
        }

        #region Listar productos
        // GET: api/listProducts
        /// <summary>
        /// Listar productos
        /// </summary>
        /// <remarks>
        /// Método para listar los productos
        /// </remarks>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("listProducts")]
        public async Task<IActionResult> listProducts()
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_listProducts", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    List<ProductsModel> detailProducts = new List<ProductsModel>();

                    while (await sqldr.ReadAsync())
                    {
                        ProductsModel detailProducts2 = new ProductsModel();
                        if (sqldr["id_product"] != DBNull.Value) { detailProducts2.idProduct = Convert.ToInt32(sqldr["id_product"]); } else { detailProducts2.idProduct = 0; }
                        if (sqldr["name"] != DBNull.Value) { detailProducts2.name = sqldr["name"].ToString(); } else { detailProducts2.name = ""; }
                        if (sqldr["supplier_name"] != DBNull.Value) { detailProducts2.supplierName = sqldr["supplier_name"].ToString(); } else { detailProducts2.supplierName = ""; }

                        detailProducts.Add(detailProducts2);
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = r.Flag ? 200 : 400;

                    if (r.Flag == true)
                    {
                        r.Data = detailProducts;
                        r.Message = "Successful products";
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

        #region Consultar detalle productos
        // GET: api/consultProducts/{idProduct}
        /// <summary>
        /// Consultar detalle del producto
        /// </summary>
        /// <remarks>
        /// Método para consultar productos
        /// </remarks>
        /// <param name="idProduct">Identificador del producto a consultar</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpGet]
        [Route("consultProducts/{idProduct}")]
        public async Task<IActionResult> consultProducts([FromRoute] int idProduct)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_consultProduct", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id_product", idProduct));
                    cmd.Parameters.Add("@message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    SqlDataReader sqldr = await cmd.ExecuteReaderAsync();

                    ProductsConsultModel detailProducts = new ProductsConsultModel();

                    while (await sqldr.ReadAsync())
                    {
                        if (sqldr["id_product"] != DBNull.Value) { detailProducts.idProduct = Convert.ToInt32(sqldr["id_product"]); } else { detailProducts.idProduct = 0; }
                        if (sqldr["name"] != DBNull.Value) { detailProducts.name = sqldr["name"].ToString(); } else { detailProducts.name = ""; }
                        if (sqldr["id_suppliers"] != DBNull.Value) { detailProducts.idSuppliers = Convert.ToInt32(sqldr["id_suppliers"]); } else { detailProducts.idSuppliers = 0; }
                    }

                    await sqldr.CloseAsync();

                    r.Message = cmd.Parameters["@message"].Value != null ? cmd.Parameters["@message"].Value.ToString() : "";
                    r.Flag = (bool)cmd.Parameters["@flag"].Value;
                    r.Status = 400;


                    if (r.Flag)
                    {
                        r.Data = detailProducts;
                        r.Message = "Successful products";
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

        #region Crear o actualizar productos
        // POST: api/createUpdateProduct
        /// <summary>
        /// Crear/Actualizar ciudad
        /// </summary>
        /// <remarks>
        /// Método que permite crear/actualizar una ciudad
        /// </remarks>
        /// <param name="product">Codigo y nombre del producto, e identificador del proveedor asociado al producto</param>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el token JWT de acceso</response>
        [HttpPost]
        [Route("createUpdateProduct")]
        public async Task<IActionResult> createUpdateProduct([FromBody] ProductsCreateUpdateModel product)
        {
            using (SqlConnection connection = conn.ConnectBD(_configuration))
            {
                ReplyLogin r = new ReplyLogin();

                try
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("sp_createUpdateProducts", connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id_product", product.idProduct));
                    cmd.Parameters.Add(new SqlParameter("@id_suppliers", product.idSuppliers));
                    cmd.Parameters.Add(new SqlParameter("@name", product.name));
                    cmd.Parameters.Add(new SqlParameter("@price", product.price));
                    cmd.Parameters.Add(new SqlParameter("@amount", product.amount));
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
                    oReply.Message = r.Message;
                    oReply.Data = null;
                    return BadRequest(oReply);
                }
            }
        }
        #endregion
    }
}