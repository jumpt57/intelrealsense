using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace deplacement_forme.model.scene
{
    class CameraScene
    {
        private PerspectiveCamera PerspectiveCamera;

        public CameraScene()
        {
            PerspectiveCamera = new PerspectiveCamera();
            PerspectiveCamera.Position = new Point3D(1.5, 2, 3);
            PerspectiveCamera.LookDirection = new Vector3D(-1.5, -2, -3);
            PerspectiveCamera.UpDirection = new Vector3D(0, 1, 0);
            PerspectiveCamera.FieldOfView = 60;

            Transform3DGroup Transform3DGroup = new Transform3DGroup();

            RotateTransform3D RotateTransform3D1 = new RotateTransform3D();
            AxisAngleRotation3D AxisAngleRotation3D1 = new AxisAngleRotation3D();
            AxisAngleRotation3D1.Axis = new Vector3D(0, 1, 0);
            AxisAngleRotation3D1.Angle = 0.0;// a voir pour le binding
            RotateTransform3D1.Rotation = AxisAngleRotation3D1;

            RotateTransform3D RotateTransform3D2 = new RotateTransform3D();
            AxisAngleRotation3D AxisAngleRotation3D2 = new AxisAngleRotation3D();
            AxisAngleRotation3D2.Axis = new Vector3D(1, 0, 0);
            AxisAngleRotation3D2.Angle = 0.0;// a voir pour le binding
            RotateTransform3D2.Rotation = AxisAngleRotation3D2;

            Transform3DGroup.Children.Add(RotateTransform3D1);
            Transform3DGroup.Children.Add(RotateTransform3D2);

            PerspectiveCamera.Transform = Transform3DGroup;
        }

        public PerspectiveCamera GetPerspectiveCamera()
        {
            return this.PerspectiveCamera;
        }

        public void SetPerspectiveCamera(PerspectiveCamera PerspectiveCamera)
        {
            this.PerspectiveCamera = PerspectiveCamera;
        }

    }
}
