using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace deplacement_forme.model.forme
{
    class Cube
    {
        private Viewport3D Viewport3D;

        private double angleX = 0, angleY = 0;
        private AxisAngleRotation3D rotationX, rotationY;
        private DoubleAnimation animation;
        private Transform3DGroup tansform3DGroup;
        private RotateTransform3D rotateTranasform3DX, rotateTranasform3DY;

        public Cube()
        {
            Viewport3D = new Viewport3D();
            Viewport3D.Margin = new Thickness(4, 4, 4, 4);

            ModelVisual3D ModelVisual3D = new ModelVisual3D();
            Viewport3D.Children.Add(ModelVisual3D);

            Model3DGroup Model3DGroup = new Model3DGroup();
            ModelVisual3D.Content = Model3DGroup;

            AmbientLight AmbientLight = new AmbientLight();
            AmbientLight.Color = Colors.Gray;
            Model3DGroup.Children.Add(AmbientLight);

            DirectionalLight DirectionalLight1 = new DirectionalLight();
            DirectionalLight1.Color = Colors.Gray;
            DirectionalLight1.Direction = new Vector3D(1, -2, -3);

            DirectionalLight DirectionalLight2 = new DirectionalLight();
            DirectionalLight2.Color = Colors.Gray;
            DirectionalLight2.Direction = new Vector3D(-1, 2, 3);

            Model3DGroup.Children.Add(DirectionalLight1);
            Model3DGroup.Children.Add(DirectionalLight2);

            GeometryModel3D GeometryModel3D = new GeometryModel3D();
            Model3DGroup.Children.Add(GeometryModel3D);

            MeshGeometry3D MeshGeometry3D = new MeshGeometry3D();
            MeshGeometry3D.Positions = new Point3DCollection(CreatePositions());
            MeshGeometry3D.TriangleIndices = new Int32Collection(CreateIndices());

            GeometryModel3D.Geometry = MeshGeometry3D;

            GeometryModel3D.Material = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(128, 255, 0 , 0)));

            rotationX = new AxisAngleRotation3D
            {
                Axis = new Vector3D(0, 1, 0),
                Angle = 1
            };

            rotationY = new AxisAngleRotation3D
            {
                Axis = new Vector3D(2, 0, -1),
                Angle = 1
            };

            animation = new DoubleAnimation();

            rotateTranasform3DX = new RotateTransform3D(rotationX);
            rotateTranasform3DY = new RotateTransform3D(rotationY);

            tansform3DGroup = new Transform3DGroup();
            tansform3DGroup.Children.Add(rotateTranasform3DX);
            tansform3DGroup.Children.Add(rotateTranasform3DY);

            GeometryModel3D.Transform = tansform3DGroup;
        }

        private Int32Collection CreateIndices()
        {
            Int32Collection points = new Int32Collection();
            points.Add(0);
            points.Add(1);
            points.Add(2);
            points.Add(2);
            points.Add(3);
            points.Add(0);

            points.Add(4);
            points.Add(5);
            points.Add(6);
            points.Add(6);
            points.Add(7);
            points.Add(4);

            points.Add(8);
            points.Add(9);
            points.Add(10);
            points.Add(10);
            points.Add(11);
            points.Add(8);

            points.Add(12);
            points.Add(13);
            points.Add(14);
            points.Add(14);
            points.Add(15);
            points.Add(12);

            points.Add(16);
            points.Add(17);
            points.Add(18);
            points.Add(18);
            points.Add(19);
            points.Add(16);

            points.Add(20);
            points.Add(21);
            points.Add(22);
            points.Add(22);
            points.Add(23);
            points.Add(20);

            return points;
        }

        private Point3DCollection CreatePositions()
        {
            Point3DCollection points = new Point3DCollection();
            points.Add(new Point3D(-1, -1, -1));
            points.Add(new Point3D(1, -1, -1));
            points.Add(new Point3D(1, -1, 1));
            points.Add(new Point3D(-1, -1, 1));

            points.Add(new Point3D(-1, -1, 1));
            points.Add(new Point3D(1, -1, 1));
            points.Add(new Point3D(1, 1, 1));
            points.Add(new Point3D(-1, 1, 1));

            points.Add(new Point3D(1, -1, 1));
            points.Add(new Point3D(1, -1, -1));
            points.Add(new Point3D(1, 1, -1));
            points.Add(new Point3D(1, 1, 1));

            points.Add(new Point3D(1, 1, 1));
            points.Add(new Point3D(1, 1, -1));
            points.Add(new Point3D(-1, 1, -1));
            points.Add(new Point3D(-1, 1, 1));

            points.Add(new Point3D(-1, -1, 1));
            points.Add(new Point3D(-1, 1, 1));
            points.Add(new Point3D(-1, 1, -1));
            points.Add(new Point3D(-1, -1, -1));

            points.Add(new Point3D(-1, -1, -1));
            points.Add(new Point3D(-1, 1, -1));
            points.Add(new Point3D(1, 1, -1));
            points.Add(new Point3D(1, -1, -1));

            return points;
        }

        public Viewport3D GetViewport3D()
        {
            return this.Viewport3D;
        }

        public void SetViewport3D(Viewport3D Viewport3D)
        {
            this.Viewport3D = Viewport3D;
        }

        public void RotationGauche()
        {
            animation.From = angleX;
            animation.To = angleX -= 0.5;
            animation.Duration = TimeSpan.FromSeconds(0.2);
            rotationX.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
        }

        public void RotationDroite()
        {
            animation.From = angleX;
            animation.To = angleX += 0.5;
            animation.Duration = TimeSpan.FromSeconds(0.2);
            rotationX.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
        }

        public void RotationHaut()
        {
            animation.From = angleY;
            animation.To = angleY -= 0.5;
            animation.Duration = TimeSpan.FromSeconds(0.2);
            rotationY.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
        }

        public void RotationBas()
        {
            animation.From = angleY;
            animation.To = angleY += 0.5;
            animation.Duration = TimeSpan.FromSeconds(0.2);
            rotationY.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
        }

        public void RotationX(double newAngleX)
        {
            animation.From = angleX;
            angleX = angleX + newAngleX;
            animation.To = angleX;
            animation.Duration = TimeSpan.FromSeconds(0.2);
            rotationX.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
        }

        public void RotationY(double newAngleY)
        {           
            animation.From = angleY;
            angleY = angleY + newAngleY;
            animation.To = angleY;
            animation.Duration = TimeSpan.FromSeconds(0.2);
            rotationY.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
        }

        public bool PointOn(Point point, Grid containinGrid)
        {

            var p = this.Viewport3D.TranslatePoint(new Point(), containinGrid);

            return (p.X <= point.X * containinGrid.ActualWidth) &&
                   ((p.X + this.Viewport3D.ActualWidth) >= point.X * containinGrid.ActualWidth) &&
                   ((p.Y + this.Viewport3D.ActualHeight) >= point.Y * containinGrid.ActualHeight) &&
                   (p.Y <= point.Y * containinGrid.ActualHeight);
        }
        
    }
}
