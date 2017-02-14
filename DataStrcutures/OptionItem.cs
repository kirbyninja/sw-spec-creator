using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecCreator.DataStrcutures
{
    public class OptionItem
    {
        public OptionItem()
        { }

        public OptionItem(int optionNo, int itemNo, string text)
            : this()
        {
            this.OptionNo = optionNo;
            this.ItemNo = itemNo;
            this.Text = text;
        }

        public int ItemNo { get; set; }
        public int OptionNo { get; set; }
        public string Text { get; set; }
    }
}