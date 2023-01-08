using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alibaba_Scout.Modals.Categories
{
    internal class Category : Base
    {
        public string Name { get; private set; }
        public string Url { get; private set; }
        public Category(string name, string url)
        {
            Name = name;
            Url = url;
        }
        private Category()
        {

        }
    }
}
