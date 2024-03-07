using System;
using System.Threading.Tasks;
using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;

namespace ELIXIR.API.Controllers.SETUP_CONTROLLER
{
    public class AccountTitleController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountTitleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route("AddNewAccountTitle")]
        public async Task<IActionResult> AddNewAccountTitle(AccountTitle[] accountTitles)
        {
            try
            {
                await _unitOfWork.AccountTitle.AddNewAccountTitle(accountTitles);
                return Ok("Operation completed");
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }
        }

        [HttpPatch]
        [Route("UpdateAccountTitleStatus")]
        public async Task<IActionResult> UpdateAccountTitleStatus(AccountTitle accountTitle)
        {
            
            try
            {
                await _unitOfWork.AccountTitle.UpdateAccountTitle(accountTitle);
                string status;
                if (accountTitle.IsActive)
                {
                    status = "Activated";
                }
                else
                {
                    status = "Inactivated";
                }

                await _unitOfWork.CompleteAsync();
                return Ok($"Account Title {accountTitle.AccountTitleName} is successfully {status}");
            }
            catch (Exception e)
            {
                return Conflict(new
                {
                    e.Message
                });
            }
        }

        [HttpGet]
        [Route("GetAllAccountTitleAsync")]
        public async Task<IActionResult> GetAllAccountTitlesAsync()
        {
            try
            {
                var accountTitles = await _unitOfWork.AccountTitle.GetAllAccountTitleAsync();
                return Ok(accountTitles);
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }
        }

        [HttpGet]
        [Route("GetAllAccountTitleAsyncPagination/{status}")]
        public async Task<IActionResult> GetAllAccountTitleAsyncPagination([FromRoute] bool status, [FromQuery] UserParams userParams)
        {
            var accountTitles = await _unitOfWork.AccountTitle.GetAllAccountTitleAsyncPagination(status, userParams);
            
            Response.AddPaginationHeader(accountTitles.PageSize, accountTitles.CurrentPage, accountTitles.TotalPages, accountTitles.TotalCount, accountTitles.HasNextPage, accountTitles.HasPreviousPage);

            var accountTitle = new
            {
                accountTitles,
                accountTitles.PageSize,
                accountTitles.CurrentPage,
                accountTitles.TotalCount,
                accountTitles.TotalPages,
                accountTitles.HasPreviousPage,
                accountTitles.HasNextPage
            };

            return Ok(accountTitle);
        }
    }
}