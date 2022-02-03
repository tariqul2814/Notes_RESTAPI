using InsightRESTAPI.Common;
using InsightRESTAPI.Common.Configuration;
using InsightRESTAPI.Model;
using InsightRESTAPI.Model.CommonModel;
using InsightRESTAPI.Model.Data;
using InsightRESTAPI.Model.DBModel;
using InsightRESTAPI.Model.ViewModel;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightRESTAPI.Services
{
    public class NoteServices : INoteServices
    {
        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        private readonly ConnectionStringConfig _connectionStringConfig;
        private readonly string requestTime = Utilities.GetRequestResponseTime();
        public NoteServices(ConnectionStringConfig connectionStringConfig)
        {
            _connectionStringConfig = connectionStringConfig;
            optionsBuilder.UseSqlServer(_connectionStringConfig.DefaultConnection);
        }

        public async Task<ServiceResponse<dynamic>> GetNotes(int? page, int? limit, string userIdentity, int noteType)
        {
            try
            {
                var spName = EnumObjects.InsightREST.InsightREST_GetNotes.ToString();
                var commandParameterList = new List<SqlCommandParameter>();
                commandParameterList.Add(SqlCommandParameter.AddParameter("@page", page.ToDBNullIfNothing()));
                commandParameterList.Add(SqlCommandParameter.AddParameter("@limit", limit.ToDBNullIfNothing()));
                commandParameterList.Add(SqlCommandParameter.AddParameter("@user_id", (Convert.ToInt32(userIdentity)).ToDBNullIfNothing()));
                commandParameterList.Add(SqlCommandParameter.AddParameter("@note_type", (Convert.ToInt32(noteType)).ToDBNullIfNothing()));

                foreach (var item in commandParameterList.Cast<SqlCommandParameter>().ToList())
                {
                    if (item.ParameterValue == DBNull.Value || item.ParameterValue == "" || item.ParameterValue == null)
                    {
                        commandParameterList.Remove(item);
                    }
                }

                if(Convert.ToInt32(EnumObjects.NotesType.Regular) == noteType)
                {
                    var result_dt = await DataHelper.ExecuteStoredProcedure<RegularNoteViewModel>(spName, commandParameterList, _connectionStringConfig.DefaultConnection);

                    var result = JsonConvert.DeserializeObject<List<RegularNoteViewModel>>(JsonConvert.SerializeObject(result_dt));

                    if (result.Count() > 0)
                    {
                        return new ServiceResponse<dynamic>
                        {
                            data = result,
                            message = new List<string>() { "note list retrieved successfully." },
                            success = true
                        };
                    }
                    else
                    {
                        return new ServiceResponse<dynamic>
                        {
                            data = null,
                            message = new List<string>() { "No note found." },
                            success = false
                        };
                    }
                }
                else if (Convert.ToInt32(EnumObjects.NotesType.Reminder) == noteType)
                {
                    var result_dt = await DataHelper.ExecuteStoredProcedure<ReminderNoteViewModel>(spName, commandParameterList, _connectionStringConfig.DefaultConnection);

                    var result = JsonConvert.DeserializeObject<List<ReminderNoteViewModel>>(JsonConvert.SerializeObject(result_dt));

                    if (result.Count() > 0)
                    {
                        return new ServiceResponse<dynamic>
                        {
                            data = result,
                            message = new List<string>() { "note list retrieved successfully." },
                            success = true
                        };
                    }
                    else
                    {
                        return new ServiceResponse<dynamic>
                        {
                            data = null,
                            message = new List<string>() { "No note found." },
                            success = false
                        };
                    }
                }
                else if (Convert.ToInt32(EnumObjects.NotesType.Todo) == noteType)
                {
                    var result_dt = await DataHelper.ExecuteStoredProcedure<TaskNoteViewModel>(spName, commandParameterList, _connectionStringConfig.DefaultConnection);

                    var result = JsonConvert.DeserializeObject<List<TaskNoteViewModel>>(JsonConvert.SerializeObject(result_dt));

                    if (result.Count() > 0)
                    {
                        return new ServiceResponse<dynamic>
                        {
                            data = result,
                            message = new List<string>() { "note list retrieved successfully." },
                            success = true
                        };
                    }
                    else
                    {
                        return new ServiceResponse<dynamic>
                        {
                            data = null,
                            message = new List<string>() { "No note found." },
                            success = false
                        };
                    }
                }
                else if (Convert.ToInt32(EnumObjects.NotesType.Bookmark) == noteType)
                {
                    var result_dt = await DataHelper.ExecuteStoredProcedure<BookmarkNoteViewModel>(spName, commandParameterList, _connectionStringConfig.DefaultConnection);

                    var result = JsonConvert.DeserializeObject<List<BookmarkNoteViewModel>>(JsonConvert.SerializeObject(result_dt));

                    if (result.Count() > 0)
                    {
                        return new ServiceResponse<dynamic>
                        {
                            data = result,
                            message = new List<string>() { "note list retrieved successfully." },
                            success = true
                        };
                    }
                    else
                    {
                        return new ServiceResponse<dynamic>
                        {
                            data = null,
                            message = new List<string>() { "No note found." },
                            success = false
                        };
                    }
                }
                else
                {
                    return new ServiceResponse<dynamic>
                    {
                        data = null,
                        message = new List<string>() { "Invalid Notes Type." },
                        success = false
                    };
                }
            }
            catch (Exception er)
            {
                return new ServiceResponse<dynamic>
                {
                    data = null,
                    message = new List<string>() { er.Message },
                    success = false
                };
            }
        }

        public async Task<ServiceResponse<dynamic>> GetNotesById(string userIdentity, int noteId)
        {
            try
            {
                using (var context = new ApplicationDbContext(optionsBuilder.Options))
                {
                    var note_result = await context.Notes.Where(x => x.ID == noteId && !x.IsRemoved && x.CreatedBy == Convert.ToInt32(userIdentity)).FirstOrDefaultAsync();

                    if (note_result == null)
                    {
                        return new ServiceResponse<dynamic>
                        {
                            data = null,
                            message = new List<string>() { "No Notes Found." },
                            success = false
                        };
                    }
                    else if (Convert.ToInt32(note_result.note_type) == 1)
                    {
                        var result = await context.RegularNotes.Where(x => x.NoteID == note_result.ID).FirstOrDefaultAsync();

                        if (result != null)
                        {
                            return new ServiceResponse<dynamic>
                            {
                                data = new RegularNoteViewModel
                                {
                                    note_id = note_result.ID,
                                    note_type = Convert.ToInt32(note_result.note_type),
                                    text_note = result.text_note
                                },
                                message = new List<string>() { "Regular Notes Found." },
                                success = true
                            };
                        }

                    }
                    else if (Convert.ToInt32(note_result.note_type) == 2)
                    {
                        var result = await context.ReminderNotes.Where(x => x.NoteID == note_result.ID).FirstOrDefaultAsync();

                        if (result != null)
                        {
                            return new ServiceResponse<dynamic>
                            {
                                data = new ReminderNoteViewModel
                                {
                                    note_id = note_result.ID,
                                    note_type = Convert.ToInt32(note_result.note_type),
                                    text_note = result.text_note,
                                    reminder_date = result.reminder_date
                                },
                                message = new List<string>() { "Reminder Notes Found." },
                                success = true
                            };
                        }
                    }

                    else if (Convert.ToInt32(note_result.note_type) == 3)
                    {
                        var result = await context.TaskNotes.Where(x => x.NoteID == note_result.ID).FirstOrDefaultAsync();

                        if (result != null)
                        {
                            return new ServiceResponse<dynamic>
                            {
                                data = new TaskNoteViewModel
                                {
                                    note_id = note_result.ID,
                                    note_type = Convert.ToInt32(note_result.note_type),
                                    text_note = result.text_note,
                                    is_complete = result.is_complete,
                                    due_date = result.due_date
                                },
                                message = new List<string>() { "Task Notes Found." },
                                success = true
                            };
                        }
                    }
                    else if (Convert.ToInt32(note_result.note_type) == 4)
                    {
                        var result = await context.BookmarkNotes.Where(x => x.NoteID == note_result.ID).FirstOrDefaultAsync();

                        if (result != null)
                        {
                            return new ServiceResponse<dynamic>
                            {
                                data = new BookmarkNoteViewModel
                                {
                                    note_id = note_result.ID,
                                    note_type = Convert.ToInt32(note_result.note_type),
                                    url = result.url
                                },
                                message = new List<string>() { "Bookmark Notes Found." },
                                success = true
                            };
                        }
                    }

                    return new ServiceResponse<dynamic>
                    {
                        data = null,
                        message = new List<string>() { "No Notes Found." },
                        success = false
                    };
                }
            }
            catch(Exception er)
            {
                return new ServiceResponse<dynamic>
                {
                    data = null,
                    message = new List<string>() { er.Message },
                    success = false
                };
            }
        }

        public async Task<ServiceResponse<NotesViewModel>> CreateNote(string userIdentity, NotesViewModel data)
        {
            try
            {
                using (var context = new ApplicationDbContext(optionsBuilder.Options))
                {
                    if (data == null)
                    {
                        return ServiceResponse<NotesViewModel>.Error("Invalid Note");
                    }
                    else if (Convert.ToInt32(data.note_type) == 1)
                    {
                        if (data.text_note == null)
                        {
                            return ServiceResponse<NotesViewModel>.Error("text_note can't be null");
                        }

                        var new_note = new Notes
                        {
                            note_type = EnumObjects.NotesType.Regular,
                            CreatedBy = Convert.ToInt32(userIdentity),
                            CreatedDate = Utilities.GetDate()
                        };

                        await context.Notes.AddAsync(new_note);
                        await context.SaveChangesAsync();

                        long note_id = new_note.ID;

                        var regular_note = new RegularNote
                        {
                            NoteID = note_id,
                            text_note = data.text_note
                        };

                        await context.RegularNotes.AddAsync(regular_note);
                        await context.SaveChangesAsync();

                        data.id = note_id;

                        return ServiceResponse<NotesViewModel>.AddedSuccessfully(data);
                    }
                    else if (Convert.ToInt32(data.note_type) == 2)
                    {
                        if (data.text_note == null || data.reminder_date == null)
                        {
                            return ServiceResponse<NotesViewModel>.Error("text_note or reminder_date can't be null");
                        }

                        var new_note = new Notes
                        {
                            note_type = EnumObjects.NotesType.Reminder,
                            CreatedBy = Convert.ToInt32(userIdentity),
                            CreatedDate = Utilities.GetDate()
                        };

                        await context.Notes.AddAsync(new_note);
                        await context.SaveChangesAsync();

                        long note_id = new_note.ID;

                        var reminder_note = new ReminderNote
                        {
                            NoteID = note_id,
                            text_note = data.text_note,
                            reminder_date = data.reminder_date.Value
                        };

                        await context.ReminderNotes.AddAsync(reminder_note);
                        await context.SaveChangesAsync();

                        data.id = note_id;

                        return ServiceResponse<NotesViewModel>.AddedSuccessfully(data);
                    }
                    else if (Convert.ToInt32(data.note_type) == 3)
                    {
                        if (data.text_note == null || data.is_complete == null || data.due_date == null)
                        {
                            return ServiceResponse<NotesViewModel>.Error("text_note or is_complete or due_date can't be null");
                        }

                        var new_note = new Notes
                        {
                            note_type = EnumObjects.NotesType.Todo,
                            CreatedBy = Convert.ToInt32(userIdentity),
                            CreatedDate = Utilities.GetDate()
                        };

                        await context.Notes.AddAsync(new_note);
                        await context.SaveChangesAsync();

                        long note_id = new_note.ID;

                        var task_note = new TaskNote
                        {
                            NoteID = note_id,
                            text_note = data.text_note,
                            is_complete = data.is_complete.Value,
                            due_date = data.due_date.Value
                        };

                        await context.TaskNotes.AddAsync(task_note);
                        await context.SaveChangesAsync();

                        data.id = note_id;

                        return ServiceResponse<NotesViewModel>.AddedSuccessfully(data);
                    }
                    else if (Convert.ToInt32(data.note_type) == 4)
                    {
                        if (data.url == null)
                        {
                            return ServiceResponse<NotesViewModel>.Error("url can't be null");
                        }

                        var new_note = new Notes
                        {
                            note_type = EnumObjects.NotesType.Bookmark,
                            CreatedBy = Convert.ToInt32(userIdentity),
                            CreatedDate = Utilities.GetDate()
                        };

                        await context.Notes.AddAsync(new_note);
                        await context.SaveChangesAsync();

                        long note_id = new_note.ID;

                        var bookmark_note = new BookmarkNote
                        {
                            NoteID = note_id,
                            url = data.url
                        };

                        await context.BookmarkNotes.AddAsync(bookmark_note);
                        await context.SaveChangesAsync();

                        data.id = note_id;

                        return ServiceResponse<NotesViewModel>.AddedSuccessfully(data);
                    }

                    return ServiceResponse<NotesViewModel>.Error("Note Type Error");
                }
            }
            catch (Exception er)
            {
                return new ServiceResponse<NotesViewModel>
                {
                    data = null,
                    message = new List<string>() { er.Message },
                    success = false
                };
            }
        }

        public async Task<ServiceResponse<NotesViewModel>> UpdateNote(string userIdentity, NotesViewModel data)
        {
            try
            {
                if (data == null)
                {
                    return ServiceResponse<NotesViewModel>.Error("Invalid Note");
                }

                if (data.note_type == 3)
                {
                    using (var context = new ApplicationDbContext(optionsBuilder.Options))
                    {
                        var note = await context.Notes.Where(x => x.ID == data.id && x.CreatedBy == Convert.ToInt32(userIdentity)).FirstOrDefaultAsync();

                        if (note == null)
                        {
                            return ServiceResponse<NotesViewModel>.Error("No Task Found.");
                        }

                        var task_note = await context.TaskNotes.Where(x => x.NoteID == note.ID).FirstOrDefaultAsync();

                        if (task_note == null)
                        {
                            return ServiceResponse<NotesViewModel>.Error("No Task Found.");
                        }

                        if (data.is_complete == null)
                        {
                            return ServiceResponse<NotesViewModel>.Error("is_complete can't be null");
                        }

                        note.UpdatedBy = note.CreatedBy;
                        note.UpdatedDate = Utilities.GetDate();

                        task_note.is_complete = data.is_complete.Value;

                        context.Notes.Update(note);
                        context.TaskNotes.Update(task_note);

                        await context.SaveChangesAsync();

                        return ServiceResponse<NotesViewModel>.UpdatedSuccessfully(data);
                    }
                }
                return ServiceResponse<NotesViewModel>.Error("Only can Update Task.");
            }
            catch(Exception er)
            {
                return ServiceResponse<NotesViewModel>.Error(er.Message);
            }
        }
    }

    public interface INoteServices
    {
        Task<ServiceResponse<dynamic>> GetNotes(int? page, int? limit, string userIdentity, int noteType);

        Task<ServiceResponse<dynamic>> GetNotesById(string userIdentity, int noteId);

        Task<ServiceResponse<NotesViewModel>> CreateNote(string userIdentity, NotesViewModel data);

        Task<ServiceResponse<NotesViewModel>> UpdateNote(string userIdentity, NotesViewModel data);
    }
}
