using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InsightRESTAPI.Model.ViewModel
{
    public class TaskNoteViewModel
    {
        public long note_id { get; set; }
        public int note_type { get; set; }
        public string text_note { get; set; }
        public DateTime due_date { get; set; }
        public bool is_complete { get; set; }
    }
}
