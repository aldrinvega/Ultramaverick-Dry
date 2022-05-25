using ELIXIR.API.ERRORS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("errors/{code}")]
    public class ErrorController : BaseApiController
    {
        public IActionResult Error (int code)
        {
            return new ObjectResult(new ApiResponse(code));
        
        }
    }
}
