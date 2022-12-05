using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaypalAccountManager.Entity
{
    public class HeaderTable
    {
        public string HeaderText { get; set; }
        public string Name { get; set; }
        public int MinimumWidth { get; set; }
        public int FillWeight { get; set; }
    }
}
