using ELIXIR.DATA.CORE.ICONFIGURATION;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELIXIR.API.Controllers.REPORT_CONTROLLER
{

    public class ReportController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportController(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }





    }
}
