using System;
using System.Collections.Generic;
using System.Text;

namespace Easytl.WF.CustomClass
{
    /// <summary>
    /// ѡ�����࣬����ComboBox����ListBox�����
    /// </summary>
    [Serializable]
    public class ListItem
    {
        public ListItem()
        { }

        public ListItem(string text, string value)
        {
            Text = text;
            Value = value;
        }

        public override string ToString()
        {
            return this.Text;
        }

        public string Text { get; set; }

        public string Value { get; set; }

        public object Tag { get; set; }
    }
}




