﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraries.Models.dbModels
{
    public class agvTask
    {
        public int Id { get; set; }
        [Required]
        public int ModelId { get; set; }
        public virtual agvModel_Info Model { get; set; }
        [Required]
        [StringLength(maximumLength: 250, MinimumLength = 1)]
        public string Table { get; set; }
        [Required]
        [StringLength(maximumLength: 250, MinimumLength = 1)]
        public string Current_Station { get; set; }
        [Required]
        [StringLength(maximumLength: 250, MinimumLength = 1)]
        public string Dest_Station { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        [Required]
        [StringLength(maximumLength: 250, MinimumLength = 1)]
        public string Result { get; set; }
        public string ErrorMessage { get; set; }

    }
}
