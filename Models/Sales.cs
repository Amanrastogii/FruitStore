using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStore.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public int FruitId { get; set; }
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; }
        [ForeignKey("FruitId")]
        public Fruit Fruit { get; set; } = null!;
    }
}
