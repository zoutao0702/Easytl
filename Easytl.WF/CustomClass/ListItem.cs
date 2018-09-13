using System;
using System.Collections.Generic;
using System.Text;

namespace Easytl.WF.CustomClass
{
    /// <summary>
    /// 选择项类，用于ComboBox或者ListBox添加项
    /// </summary>
    [Serializable]
    public class ListItem
    {
        private string text = string.Empty;
        private string value = string.Empty;

        public ListItem()
        { }

        public ListItem(string _text, string _value)
        {
            text = _text;
            value = _value;
        }

        public override string ToString()
        {
            return this.text;
        }

        public string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }
    }
}




