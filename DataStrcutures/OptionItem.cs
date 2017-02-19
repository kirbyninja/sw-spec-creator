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

        public OptionItem(int itemNo, string text)
            : this()
        {
            this.ItemNo = itemNo;
            this.Text = text;
        }

        public OptionItem(Option option, int itemNo, string text)
            : this(itemNo, text)
        {
            this.Option = option;
        }

        public int ItemNo { get; set; }
        public Option Option { get; set; }
        public string Text { get; set; }
    }
}