using deplacement_forme.model.forme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace deplacement_forme.controller
{
    class CubeController
    {
        private bool handClosed;
        private float StartX, StartY, PreviousX, PreviousY;

        public CubeController()
        {
            Init();
        }

        private void Init()
        {           
            handClosed = false;
        }

        public bool GetHandClosed()
        {
            return this.handClosed;
        }

        public void SetHandClosed(bool handClosed)
        {
            this.handClosed = handClosed;
        }

        public void CheckCollision(PXCMPoint3DF32 position, Cube Cube, Grid grid)
        {
            if (handClosed)
            {
                Point point = new Point();
                point.X = Math.Max(Math.Min(0.9F, position.x), 0.1F);
                point.Y = Math.Max(Math.Min(0.9F, position.y), 0.1F);

                if (Cube.PointOn(point, grid))
                {
                    if (StartX != -999999999 && StartY != -999999999)
                    {
                        var CurrentX = position.x - 0.5f;
                        var CurrentY = position.y - 0.5f;

                        if (CurrentX != PreviousX && CurrentY != PreviousY)
                        {
                            PreviousX = CurrentX;
                            PreviousY = CurrentY;

                            var diffx = CurrentX - StartX;
                            var diffy = CurrentY - StartY;

                            Cube.RotationX(diffx * 8);
                            Cube.RotationY(diffy * 8);
                        }
                        
                    }
                }
            }
        }

        public float GetStartX()
        {
            return this.StartX;
        }

        public void SetStartX(float StartX)
        {
            this.StartX = StartX;
        }

        public float GetStartY()
        {
            return this.StartY;
        }

        public void SetStartY(float StartY)
        {
            this.StartY = StartY;
        }

        public float GetPreviousX()
        {
            return this.PreviousX;
        }

        public void SetPreviousX(float PreviousX)
        {
            this.PreviousX = PreviousX;
        }

        public float GetPreviousY()
        {
            return this.PreviousY;
        }

        public void SetPreviousY(float PreviousY)
        {
            this.PreviousY = PreviousY;
        }
    }
}
