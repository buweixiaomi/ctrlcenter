using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManageWeb.Areas.OutApi.Models
{
    public class FeedBackData
    {
        public string CusNo { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int FeedbackType { get; set; }
      
    }
}