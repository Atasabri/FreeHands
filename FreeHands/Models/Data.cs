using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FreeHands.Models
{
    public class Data
    {
        public int ID { get; set; }
        public string TradeName { get; set; }
        public int OrdersCount { get; set; }
        public double OrdersPrice { get; set; }
        public double OrdersPriceT { get; set; }
        public int OrdersCountV { get; set; }
        public double OrdersPriceV { get; set; }
        public double OrdersPriceTV { get; set; }
    }
}