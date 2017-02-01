using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace schema_android_like.model.forme
{
    class FormeRonde : Forme
    {
        private Rectangle rectangle;
        public FormeRonde(int column, int row)
        {
            this.rectangle = new Rectangle();
            this.rectangle.Name = CreateName(column, row);
            this.rectangle.Fill = Brushes.LightBlue;
            this.rectangle.Opacity = 0.50;
            this.rectangle.Height = 100;
            this.rectangle.Width = 100;
            this.rectangle.Stroke = Brushes.Black;
            this.rectangle.RadiusX = 50;
            this.rectangle.RadiusY = 50;
            this.SetColumn(column);
            this.SetRow(row);   
        }

        override public Rectangle Get()
        {
            return this.rectangle;
        }
                
    }
}
