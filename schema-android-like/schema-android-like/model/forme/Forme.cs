using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace schema_android_like.model.forme
{
    abstract class Forme
    {
        abstract public Rectangle Get();

        private int column;
        private int row;
        private bool touched;  

        public Forme()
        {
            this.touched = false;
        }

        public bool PointOn(Point point, Grid containinGrid)
        {
            var p = this.Get().TranslatePoint(new Point(), containinGrid);

            return (p.X <= point.X * containinGrid.ActualWidth) &&
                   ((p.X + this.Get().ActualWidth) >= point.X * containinGrid.ActualWidth) &&
                   ((p.Y + this.Get().ActualHeight) >= point.Y * containinGrid.ActualHeight) &&
                   (p.Y <= point.Y * containinGrid.ActualHeight);
        }

        public string CreateName(int column, int row)
        {
            return "F_R" + row + "C" + column;
        }

        public void WasTouched()
        {
            this.Get().Fill = Brushes.Blue;
            this.touched = true;
        }

        public void Reset()
        {
            this.Get().Fill = Brushes.LightBlue;
            this.touched = false;
        }

        public int GetColumn()
        {
            return this.column;
        }

        public void SetColumn(int column)
        {
            this.column = column;
        }

        public int GetRow()
        {
            return this.row;
        }

        public void SetRow(int row)
        {
            this.row = row;
        }

        public bool IsTouched()
        {
            return this.touched;
        }

        public bool CompareByPosition(Forme forme)
        {
            if (this.GetColumn() == forme.GetColumn() && this.GetRow() == forme.GetRow())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
