using InsightRESTAPI.Common;
using InsightRESTAPI.Model.CommonModel;
using InsightRESTAPI.Model.ViewModel;
using InsightRESTAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsightRESTAPI.Controllers
{
    [ApiExplorerSettings(IgnoreApi = false)]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class NoteController : Controller
    {
        private readonly INoteServices _services;
        private readonly string requestTime = Utilities.GetRequestResponseTime();
        public NoteController(INoteServices services)
        {
            _services = services;
        }

        [HttpGet]
        [ProducesResponseType(typeof(SuccessfulLoginResponse), 200)]
        [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
        [Route("")]
        public async Task<IActionResult> GetNotes(int? page, int? limit, string note_type)
        {
            try
            {
                int note_type_id = 0;
                if(note_type.Trim().ToLower() == "regular note")
                {
                    note_type_id = Convert.ToInt32(EnumObjects.NotesType.Regular);
                }
                else if (note_type.Trim().ToLower() == "reminder")
                {
                    note_type_id = Convert.ToInt32(EnumObjects.NotesType.Reminder);
                }
                else if (note_type.Trim().ToLower() == "todo")
                {
                    note_type_id = Convert.ToInt32(EnumObjects.NotesType.Todo);
                }
                else if (note_type.Trim().ToLower() == "bookmark")
                {
                    note_type_id = Convert.ToInt32(EnumObjects.NotesType.Bookmark);
                }
                else
                {
                    return ErrorResponse.BadRequest("Invalid Note Type. Note Type should be regular note / reminder / todo / bookmark");
                }
                var result = await _services.GetNotes(page, limit, User.Identity.Name, note_type_id);

                if(!result.success)
                {
                    return ErrorResponse.BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(SuccessfulLoginResponse), 200)]
        [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
        [Route("{id:int}")]
        public async Task<IActionResult> GetNotesById(int id)
        {
            try
            {
                var result = await _services.GetNotesById(User.Identity.Name, id);

                if (!result.success)
                {
                    return ErrorResponse.BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(CreatedResult), 201)]
        [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
        [Route("")]
        public async Task<IActionResult> CreateNote(NotesViewModel note)
        {
            try
            {
                if (note.note_type_name.Trim().ToLower() == "regular note")
                {
                    note.note_type = Convert.ToInt32(EnumObjects.NotesType.Regular);
                }
                else if (note.note_type_name.Trim().ToLower() == "reminder")
                {
                    note.note_type = Convert.ToInt32(EnumObjects.NotesType.Reminder);
                }
                else if (note.note_type_name.Trim().ToLower() == "todo")
                {
                    note.note_type = Convert.ToInt32(EnumObjects.NotesType.Todo);
                }
                else if (note.note_type_name.Trim().ToLower() == "bookmark")
                {
                    note.note_type = Convert.ToInt32(EnumObjects.NotesType.Bookmark);
                }
                else
                {
                    return ErrorResponse.BadRequest("Invalid Note Type. Note Type should be regular note / reminder / todo / bookmark");
                }

                var result = await _services.CreateNote(User.Identity.Name, note);

                if (!result.success)
                {
                    return ErrorResponse.BadRequest(result);
                }

                var response = new PayloadResponse<NotesViewModel>
                {
                    Message = result.message,
                    Payload = result.data,
                    PayloadType = "Add Note",
                    RequestTime = requestTime,
                    Success = result.success
                };

                return Created($"{response.RequestURL}/{response.Payload.id}", response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(SuccessfulLoginResponse), 200)]
        [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateNote(NotesViewModel note, int id)
        {
            try
            {
                note.id = id;
                if (note.note_type_name.Trim().ToLower() == "regular note")
                {
                    note.note_type = Convert.ToInt32(EnumObjects.NotesType.Regular);
                }
                else if (note.note_type_name.Trim().ToLower() == "reminder")
                {
                    note.note_type = Convert.ToInt32(EnumObjects.NotesType.Reminder);
                }
                else if (note.note_type_name.Trim().ToLower() == "todo")
                {
                    note.note_type = Convert.ToInt32(EnumObjects.NotesType.Todo);
                }
                else if (note.note_type_name.Trim().ToLower() == "bookmark")
                {
                    note.note_type = Convert.ToInt32(EnumObjects.NotesType.Bookmark);
                }
                else
                {
                    return ErrorResponse.BadRequest("Invalid Note Type. Note Type should be regular note / reminder / todo / bookmark");
                }

                var result = await _services.UpdateNote(User.Identity.Name, note);

                if (!result.success)
                {
                    return ErrorResponse.BadRequest(result);
                }

                var response = new PayloadResponse<NotesViewModel>
                {
                    Message = result.message,
                    Payload = result.data,
                    PayloadType = "Update Note",
                    RequestTime = requestTime,
                    Success = result.success
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
