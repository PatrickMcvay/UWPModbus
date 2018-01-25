using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmiWithNModbus4.Utilities.Extensions.Yokogawa
{
    public static class YokoExtensions
    {
        public static ushort ToRegisterNumber(this int i)
        {
            int isub = 40001;
            return (ushort) (i - isub);
        }
    }
}
