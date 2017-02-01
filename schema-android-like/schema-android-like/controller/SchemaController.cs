using schema_android_like.model.forme;
using System.Collections.Generic;
using System;
using System.Windows;
using System.Windows.Controls;
using schema_android_like.view.popup;

namespace schema_android_like.controller
{
    /// <summary>
    /// 
    /// </summary>
    class SchemaController
    {
        private List<Forme> formes;
        private List<Forme> solution;
        private List<Forme> current;

        private bool handClosed;      

        public SchemaController()
        {
            Init();           
        } 

        private void Init()
        {
            formes = new List<Forme>();
            solution = new List<Forme>();
            current = new List<Forme>();

            handClosed = false;
        }

        public Forme CreateFormeRectangle(int column, int row)
        {
            var forme = new FormeRectangle(column, row); 
            formes.Add(forme);
            return forme;
        }

        public Forme CreateFormeRonde(int column, int row)
        {
            var forme = new FormeRonde(column, row);
            formes.Add(forme);
            return forme;
        }

        public bool GetHandClosed()
        {
            return this.handClosed;
        }

        public void SetHandClosed(bool handClosed)
        {
            this.handClosed = handClosed;
        }

        public void CheckCollision(PXCMPoint3DF32 position, Grid grid)
        {
            if (handClosed)
            {
                Point point = new Point();
                point.X = Math.Max(Math.Min(0.9F, position.x), 0.1F);
                point.Y = Math.Max(Math.Min(0.9F, position.y), 0.1F);

                foreach (Forme forme in formes)
                {
                    if (!forme.IsTouched() && forme.PointOn(point, grid))
                    {
                        forme.WasTouched();
                        current.Add(forme);
                        CompareSolutionAndCurrent();
                        break;
                    }
                }                
            }
        }

        public void ResetFormes()
        {
            foreach (Forme forme in formes)
            {
                forme.Reset();                
            }
            current.Clear();
        }

        public void CreateSchemaToFind()
        {
            foreach (Forme forme in formes)
            {
                if (forme.GetRow() == 0 && forme.GetColumn() == 0)
                {
                    solution.Add(forme);
                }
                else if (forme.GetRow() == 0 && forme.GetColumn() == 1)
                {
                    solution.Add(forme);
                }
                else if (forme.GetRow() == 0 && forme.GetColumn() == 2)
                {
                    solution.Add(forme);
                }
                else if (forme.GetRow() == 1 && forme.GetColumn() == 1)
                {
                    solution.Add(forme);
                }
                else if (forme.GetRow() == 2 && forme.GetColumn() == 0)
                {
                    solution.Add(forme);
                }
                else if (forme.GetRow() == 2 && forme.GetColumn() == 1)
                {
                    solution.Add(forme);
                }
                else if (forme.GetRow() == 2 && forme.GetColumn() == 2)
                {
                    solution.Add(forme);
                }
            }
        }


        private void CompareSolutionAndCurrent()
        {
            bool theSame = true;
            for (int i = 0; i < current.Count; i++)
            {
                if (!solution[i].CompareByPosition(current[i]))
                {
                    theSame = false;                   
                    break;
                }               
            }

            if (solution.Count == current.Count && theSame)
            {
                new TimedPopUp("Information", "Dévérouillage !", new TimeSpan(0, 0, 2)).Show();
                ResetFormes();
            }
        }

    }
}
