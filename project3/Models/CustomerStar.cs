using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace project3.Models
{
    public class CustomerStar
    {
        public int cus_ID { get; set; }
        public byte[] Img { get; set; }
        public string UserName { get; set; }
        public string Sex { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public double Star { get; set; }
    }
}