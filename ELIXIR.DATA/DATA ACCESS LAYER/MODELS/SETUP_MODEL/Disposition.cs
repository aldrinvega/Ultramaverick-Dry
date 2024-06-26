﻿using System;
using Microsoft.VisualBasic;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class Disposition : BaseEntity
    { 
        public string DispositionName { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public string ModifiedBy { get; set; }
        public string Reason { get; set; }
    }
}