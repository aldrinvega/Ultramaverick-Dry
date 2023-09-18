using System.Threading.Tasks;
using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY.AddNewChecklistQuestions;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY;
using System.Security.Claims;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY.Checklist_Operation;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY.Checklist_Questions;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY.Checklist_Types;
using Microsoft.AspNetCore.Authorization;

namespace ELIXIR.API.Controllers.SETUP_CONTROLLER
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChecklistController : BaseApiController
    {
        private readonly IMediator _mediator;

        public ChecklistController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("AddNewChecklistQuestion")]
        public async Task<IActionResult> Add([FromBody] AddNewChecklistQuestionCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity
                && int.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.AddedBy = userId;
                }

                await _mediator.Send(command);
                return Ok();
            }
            catch (Exception e)
            {
                return Conflict(new
                {
                    e.Message
                });
            }
        }
        
        [Authorize]
        [HttpGet("GetAllChecklists")]
        public async Task<IActionResult> Get([FromQuery] GetAllChecklists.GetAllChecklistsQuery query)
        {
            try
            {
                var checklists = await _mediator.Send(query);

                Response.AddPaginationHeader(
                    checklists.CurrentPage,
                    checklists.PageSize,
                    checklists.TotalCount,
                    checklists.TotalPages,
                    checklists.HasPreviousPage,
                    checklists.HasNextPage
                );

                var result = new {
                        checklists,
                        checklists.CurrentPage,
                        checklists.PageSize,
                        checklists.TotalCount,
                        checklists.TotalPages,
                        checklists.HasPreviousPage,
                        checklists.HasNextPage
                };
                
                return Ok(result);
            }
            catch (Exception e)
            {
                return Conflict(new
                {
                    e.Message
                });
            }
        }
        
        [HttpGet("GetAllChecklistsQuestions")]
        public async Task<IActionResult> Get([FromQuery] GetAllChecklistsDescription.GetAllChecklistsDescriptionQuery query)
        {
            try
            {
                var checklistsQuestions = await _mediator.Send(query);

                Response.AddPaginationHeader(
                    checklistsQuestions.CurrentPage,
                    checklistsQuestions.PageSize,
                    checklistsQuestions.TotalCount,
                    checklistsQuestions.TotalPages,
                    checklistsQuestions.HasPreviousPage,
                    checklistsQuestions.HasNextPage
                );

                var result = new {
                    checklistsQuestions,
                    checklistsQuestions.CurrentPage,
                    checklistsQuestions.PageSize,
                    checklistsQuestions.TotalCount,
                    checklistsQuestions.TotalPages,
                    checklistsQuestions.HasPreviousPage,
                    checklistsQuestions.HasNextPage
                };
                
                return Ok(result);
            }
            catch (Exception e)
            {
                return Conflict(new
                {
                    e.Message
                });
            }
        }

        [HttpPatch("UpdateChecklistQuestionStatus")]
        public async Task<IActionResult> UpdateChecklistStatus(
            [FromQuery] UpdateChecklistStatus.UpdateChecklistStatusCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok();
            }
            catch (Exception e)
            {
                return Conflict(new
                {
                    e.Message
                });
            }
        }
        
        [HttpPut("UpdateChecklistQuestion/{id:int}")]
        public async Task<IActionResult> UpdateChecklistDescription(
            [FromBody] UpdateChecklistQuestion.UpdateChecklistDescriptionCommand command, int id)
        {
            try
            {
                command.Id = id;
                await _mediator.Send(command);
                return Ok();
            }
            catch (Exception e)
            {
                return Conflict(new
                {
                    e.Message
                });
            }
        }
        
        [HttpPost("AddNewChecklistType")]
        public async Task<IActionResult> Add([FromBody] AddNewChecklistType.AddNewChecklistTypeCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok();
            }
            catch (Exception e)
            {
                return Conflict(new
                {
                    e.Message
                });
            }
        }

        [HttpPost("AddNewChecklist")]
        public async Task<IActionResult> AddNewChecklist(AddNewChecklist.AddNewChecklistCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok("Done");
            }
            catch (Exception e)
            {
                return Conflict(new
                {
                    e.Message
                });
            }
        }
    }
}