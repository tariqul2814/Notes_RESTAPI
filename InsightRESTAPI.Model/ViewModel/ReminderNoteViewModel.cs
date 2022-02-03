using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InsightRESTAPI.Model.ViewModel
{
    public class ReminderNoteViewModel
    {
        public long note_id { get; set; }
        public int note_type { get; set; }
        public string text_note { get; set; }
        public DateTime reminder_date { get; set; }
    }
}
