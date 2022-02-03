using System;
using System.Collections.Generic;
using System.Text;
using static InsightRESTAPI.Common.EnumObjects;

namespace InsightRESTAPI.Model.DBModel
{
    public class Notes : BaseModel
    {
        public long ID { get; set; }
        public NotesType note_type { get; set; }
    }
}
