using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InsightRESTAPI.Model.ViewModel
{
    public class NotesViewModel
    {
        public long id { get; set; }
        public int note_type { get; set; }
        public string note_type_name { get; set; }
        [Url]
        public string? url { get; set; }
        public string? text_note { get; set; }
        public DateTime? reminder_date { get; set; }
        public DateTime? due_date { get; set; }
        public bool? is_complete { get; set; }

    }
}
