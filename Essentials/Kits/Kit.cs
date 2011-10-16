using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Essentials.Kits
{
    public struct Kit
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Int32> ItemList { get; set; }
    }
}
