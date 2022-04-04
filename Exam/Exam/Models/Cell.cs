using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Exam.Models
{
    public class Cell : Label
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool IsChecker { get; set; }
        public bool IsQueen { get; set; } = false;
        public int Player { get; set; }
        public override string ToString()
        => $"Row {Row} Column {Column}";
    }
}
