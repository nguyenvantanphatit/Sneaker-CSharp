using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sneaker.Models
{
    public class Email
    {
        public string To { get; set; }
        public string From { get; set; }
        public int Subject { get; set; }
    }
}