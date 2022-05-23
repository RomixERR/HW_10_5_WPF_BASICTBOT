using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HW_10_5_WPF_BASICTBOT
{
    public class UserMessage
    {
        public string UserName   { get; set; }
        public string Message    { get; set; }
        public long ChatId       { get; set; }
        public DateTime dateTime { get; set; }
        public Brush brush       { get; set; }
    }
}
