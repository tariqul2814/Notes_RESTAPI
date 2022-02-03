using System;
using System.Collections.Generic;
using System.Text;

namespace InsightRESTAPI.Common
{
    public class EnumObjects
    {
        public enum Role
        {
            User
        }

        public enum InsightREST
        {
            InsightREST_GetNotes
        }

        public enum NotesType
        {
            Regular = 1,
            Reminder = 2,
            Todo = 3,
            Bookmark = 4
        }
    }
}
