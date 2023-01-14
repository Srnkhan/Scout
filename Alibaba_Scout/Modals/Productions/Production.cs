using Alibaba_Scout.Modals.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alibaba_Scout.Modals.Productions
{
    internal class Production : Base
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string Detail { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category{ get; set; }
        private Production()
        {

        }
        public Production(string url, string name, string detail, Guid categoryId)
        {
            Url = url;
            Name = name;
            Detail = detail;
            CategoryId = categoryId;
        }
    }
}
