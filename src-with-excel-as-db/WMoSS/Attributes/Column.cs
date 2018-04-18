using System;
using System.Collections.Generic;
using System.Text;

namespace WMoSS.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class Column : Attribute
    {
        public int ColumnIndex { get; set; }


        public Column(int column)
        {
            ColumnIndex = column;
        }
    }
}
