using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace ServiceAppointmentSystem.Models.Entities
{
	public class Item 
	{
		public int Id { get; set; }

		public string Name { get; set; }	

		public string Description { get; set; }	

		[Display(Name = "List Price")]
		public double ListPrice { get; set; }

		[Display(Name = "Price for 20 Items")]
		public double Price20 { get; set; }

		[Display(Name = "Price for 50 Items")]
		public double Price50 { get; set; }

        [Display(Name = "Image URL")]
		public string? ImageURL { get; set; }

		public double Price { get; set; }

		[Display(Name = "Service Type")]
		public int ServiceId { get; set; }

        [ForeignKey("ServiceId")]
		[ValidateNever]
		public Service? Service { get; set; }
	}
}
