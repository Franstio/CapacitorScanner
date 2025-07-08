﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitorScanner.Model
{
    public class BinModel
    {
        public string Name { get; set; } = string.Empty;
        public string WasteType { get; set; } = string.Empty;
        public decimal Weight { get; set; } = 0;
        public decimal MaxWeight { get; set; } = 0;
        public decimal Percentage { get => (Weight / MaxWeight) * 100; }
        public BinModel() { }
        public BinModel(string _name,string _wasteType, decimal _weight,decimal _maxweight)
        {
            Name = _name;
            WasteType = _wasteType;
            Weight = _weight;
            MaxWeight = _maxweight;
        }
    }
}
