﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceAppointmentSystem.Models.Entities
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ItemId { get; set; }

        public int Count { get; set; }

        public double Price { get; set; }

        [ForeignKey("OrderId")]
        public OrderHeader? Order { get; set; }

        [ForeignKey("ItemId")]
        public Item? Item { get; set; }
    }
}
