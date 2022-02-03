using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InsightRESTAPI.Model.DBModel
{
    public class RegularNote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Maximum 100 characters")]
        public string text_note { get; set; }

        [Display(Name = "Note Id")]
        [Required(ErrorMessage = "Note Id is required")]
        public long NoteID { get; set; }
        public virtual Notes Notes { get; set; }
    }
}
