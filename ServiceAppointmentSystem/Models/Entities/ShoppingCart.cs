using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ServiceAppointmentSystem.Models.Entities
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }

        public int ItemId { get; set; }

        public string UserId { get; set; }

		[Range(1, 1000, ErrorMessage = "Enter a value from 1 to 1000")]
		public int Count { get; set; }

		[ForeignKey("ItemId")]
        public Item? Item { get; set; }

        [ForeignKey("UserId")]
        public AppUser? AppUser { get; set; }

		[NotMapped]
		public double Price { get; set; }
	}
}
