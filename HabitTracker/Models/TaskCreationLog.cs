using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTracker.Models
{   
    public enum RepeatPattern
    {
        Einmalig,
        Täglich,
        Wöchentlich,
        Monatlich,
    }

    public class TaskCreationLog
    {
        public int Id { get; set; }
        public int TaskTemplateId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public RepeatPattern RepeatPattern { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
