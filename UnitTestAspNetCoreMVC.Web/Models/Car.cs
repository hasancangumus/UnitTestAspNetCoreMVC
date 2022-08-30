using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UnitTestAspNetCoreMVC.Web.Models
{
    public partial class Car
    {
        public int Id { get; set; }
        [Required]
        public string Model { get; set; }
        public decimal? Price { get; set; }
        public int? ModelYear { get; set; }

        [Required]
        public string Color { get; set; }
    }
}
