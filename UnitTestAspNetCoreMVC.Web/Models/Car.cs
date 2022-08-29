using System;
using System.Collections.Generic;

namespace UnitTestAspNetCoreMVC.Web.Models
{
    public partial class Car
    {
        public int Id { get; set; }
        public string? Model { get; set; }
        public decimal? Price { get; set; }
        public int? ModelYear { get; set; }
        public string? Color { get; set; }
    }
}
