using api_pns.Context;
using api_pns.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

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
    }
}
