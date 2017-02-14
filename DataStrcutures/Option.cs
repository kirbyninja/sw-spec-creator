using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecCreator.DataStrcutures
{
    public class Option
    {
        private readonly List<OptionItem> optionItems = new List<OptionItem>();

        public Option()
        { }

        public Option(int optionNo, string text)
            : this()
        {
            this.OptionNo = optionNo;
            this.Text = text;
        }

        public Option(int optionNo, string text, IEnumerable<OptionItem> items)
            : this(optionNo, text)
        {
            AddItems(items);
        }

        public IEnumerable<OptionItem> Items { get { return optionItems.AsReadOnly(); } }
        public int OptionNo { get; set; }
        public string Text { get; set; }

        public void AddItem(OptionItem item)
        {
            item.Option = this;
            optionItems.Add(item);
        }

        public void AddItem(int itemNo, string text)
        {
            AddItem(new OptionItem(this, itemNo, text));
        }

        public void AddItems(IEnumerable<OptionItem> items)
        {
            foreach (var item in items)
                AddItem(item);
        }

        public override string ToString()
        {
            return Text;
        }
    }
}