using ImageSharePassword.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageSharePassword.Web.Models
{
    public class ImageViewModel
    {
        public List<int> ImageIds { get; set; }
        public Image Image { get; set; }
    }
}
