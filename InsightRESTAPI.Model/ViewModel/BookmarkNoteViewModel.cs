using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InsightRESTAPI.Model.ViewModel
{
    public class BookmarkNoteViewModel
    {
        public long note_id { get; set; }
        public int note_type { get; set; }
        [Url]
        public string url { get; set; }
    }
}
