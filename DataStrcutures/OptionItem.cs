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

        public override bool Equals(object obj)
        {
            if (obj is OptionItem)
            {
                var o = obj as OptionItem;
                return this.GetType().GetProperties().Where(p => p.PropertyType.IsValueType).All(p => p.GetValue(this).Equals(p.GetValue(o)))
                    && ((this.Option == null && o.Option == null) || this.Option.OptionNo == o.Option.OptionNo);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            if (Option == null)
                return ItemNo.GetHashCode();
            else
                return Option.OptionNo.GetHashCode() ^ ItemNo.GetHashCode();
        }

        public override string ToString()
        {
            return Text;
        }
    }
}