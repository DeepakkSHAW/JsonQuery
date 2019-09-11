using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JsonQuery
{
    public class TimeSheets
    {
        [Required]
        public DateTime? FortniteEndDate { get; set; }
        [Required]
        public int? TimeSheetEntryId { get; set; }
        [Required]
        public DateTime? TimeSheetEntryDate { get; set; }
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        [Required]
        [StringLength(50)]
        public string EmpCode { get; set; }

        [Required]
        public int? DepartmentCode { get; set; }

        public DateTime? SyncDate { get; set; }
        // public List<Departments> Department { get; set; }
        [Required]
        [StringLength(50)]
        public string ActivityCode { get; set; } //Chris21 >> Account
        [Required]
        public byte? Type { get; set; }
        [Required]
        public float? Hours { get; set; }
        [Required]
        public float? Rate { get; set; }
        [Required]
        public float? MinRate { get; set; }
    }
}
