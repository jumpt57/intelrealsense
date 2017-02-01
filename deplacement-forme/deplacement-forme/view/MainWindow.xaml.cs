using deplacement_forme.controller;
using deplacement_forme.model;
using deplacement_forme.model.forme;
using deplacement_forme.model.scene;
using RealSenseNavigator;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TouchlessControllerConfiguration = PXCMTouchlessController.ProfileInfo.Configuration;


namespace deplacement_forme
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RealSenseEngine rsEngine;
        private CubeController controller;

        private CursorDisplay cursorDisplay;
        private OutOfWindowBorder outOfScreenBorder;       

        private Button rotateGauche, rotateDroite, rotateHaut, rotateBas;

        private Cube Cube;
        private CameraScene Camera;
        
        public MainWindow()
        {
            InitializeComponent();

            InitRealSenseUi();
            InitRsEngine();

            InitWindow();
            InitGrid();

            InitButtons();
            InitScene();

            controller = new CubeController();
        }

        private void InitWindow()
        {
            CubeWindow.Title = "Intel RealSense - Cube Manipulation";

            CubeWindow.Width = 1000;
            CubeWindow.Height = 1000;
            CubeWindow.MinWidth = 1000;
            CubeWindow.MinHeight = 1000;
            CubeWindow.MaxWidth = 1000;
            CubeWindow.MaxHeight = 1000;

            CubeWindow.Closing += WindowClosing;

            CubeWindow.Background = Brushes.LightGray;

            CenterWindowOnScreen();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (rsEngine != null)
            {
                rsEngine.Shutdown();
            }
        }

        private void InitGrid()
        {
            // Column

            ColumnDefinition column0 = new ColumnDefinition();
            column0.Width = new GridLength(20.0, GridUnitType.Pixel);
            SchemaGrid.ColumnDefinitions.Add(column0);

            ColumnDefinition column1 = new ColumnDefinition();
            column1.Width = new GridLength(1.0, GridUnitType.Star);
            SchemaGrid.ColumnDefinitions.Add(column1);

            ColumnDefinition column2 = new ColumnDefinition();
            column2.Width = new GridLength(20.0, GridUnitType.Pixel);
            SchemaGrid.ColumnDefinitions.Add(column2);

            // Row

            RowDefinition row0 = new RowDefinition();
            row0.Height = new GridLength(25.0, GridUnitType.Pixel);
            SchemaGrid.RowDefinitions.Add(row0);

            RowDefinition row1 = new RowDefinition();
            row1.Height = new GridLength(1.0, GridUnitType.Star);
            SchemaGrid.RowDefinitions.Add(row1);

            RowDefinition row2 = new RowDefinition();
            row2.Height = new GridLength(25.0, GridUnitType.Pixel);
            SchemaGrid.RowDefinitions.Add(row2);            
        }

        private void InitButtons()
        {
            rotateDroite = new Button();
            rotateDroite.Content = "D";
            Grid.SetRow(rotateDroite, 1);
            Grid.SetColumn(rotateDroite, 2);
            rotateDroite.Click += rotateDroite_Click;
            SchemaGrid.Children.Add(rotateDroite);           

            rotateGauche = new Button();
            rotateGauche.Content = "G";
            Grid.SetRow(rotateGauche, 1);
            Grid.SetColumn(rotateGauche, 0);
            rotateGauche.Click += rotateGauche_Click;
            SchemaGrid.Children.Add(rotateGauche);

            rotateHaut= new Button();
            rotateHaut.Content = "H";
            Grid.SetRow(rotateHaut, 0);
            Grid.SetColumn(rotateHaut, 1);
            rotateHaut.Click += rotateHaut_Click;
            SchemaGrid.Children.Add(rotateHaut);

            rotateBas = new Button();
            rotateBas.Content = "B";
            Grid.SetRow(rotateBas, 2);
            Grid.SetColumn(rotateBas, 1);
            rotateBas.Click += rotateBas_Click;
            SchemaGrid.Children.Add(rotateBas);
        }

        public void rotateDroite_Click(object send, EventArgs e)
        {
            Console.WriteLine("Droite !");
            Cube.RotationDroite();
        }

        public void rotateGauche_Click(object send, EventArgs e)
        {
            Console.WriteLine("Gauche !");
            Cube.RotationGauche();
        }

        public void rotateHaut_Click(object send, EventArgs e)
        {
            Console.WriteLine("Haut !");
            Cube.RotationHaut();
        }

        public void rotateBas_Click(object send, EventArgs e)
        {
            Console.WriteLine("Bas !");
            Cube.RotationBas();
        }

        private void InitScene()
        {          
            Cube = new Cube();

            Grid.SetRow(Cube.GetViewport3D(), 1);
            Grid.SetColumn(Cube.GetViewport3D(), 1);
            SchemaGrid.Children.Add(Cube.GetViewport3D());

            Camera = new CameraScene();
            Cube.GetViewport3D().Camera = Camera.GetPerspectiveCamera();
        }

        private void InitRealSenseUi()
        {
            CreateCursorDisplay();
            CreateOutOfWindowBorder();

            Grid.SetRow(cursorDisplay, 0);
            Grid.SetColumn(cursorDisplay, 0);
            Grid.SetRowSpan(cursorDisplay, 2);
            Grid.SetColumnSpan(cursorDisplay, 2);

            Grid.SetRow(outOfScreenBorder, 0);
            Grid.SetColumn(outOfScreenBorder, 0);
            Grid.SetRowSpan(outOfScreenBorder, 2);
            Grid.SetColumnSpan(outOfScreenBorder, 2);

            SchemaGrid.Children.Add(cursorDisplay);
            SchemaGrid.Children.Add(outOfScreenBorder);

            cursorDisplay.ChangeCursorState(RealSenseNavigator.CursorDisplay.CursorStates.Hidden);
        }

        private void OnFiredUxEventDelegate(PXCMTouchlessController.UXEventData data)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                cursorDisplay.SetPosition(data.position.x, data.position.y, data.position.z,
                    data.bodySide == PXCMHandData.BodySideType.BODY_SIDE_RIGHT);
                
                controller.CheckCollision(data.position, Cube, SchemaGrid);               

                outOfScreenBorder.X = data.position.x - 0.5f;
                outOfScreenBorder.Y = data.position.y - 0.5f;

                if (data.type == PXCMTouchlessController.UXEventData.UXEventType.UXEvent_CursorVisible)
                {
                    cursorDisplay.ChangeCursorState(RealSenseNavigator.CursorDisplay.CursorStates.Normal);
                    cursorDisplay.ChangeCursorState(RealSenseNavigator.CursorDisplay.CursorStates.Visible);
                    outOfScreenBorder.Visibility = Visibility.Visible;
                }
                else if (data.type == PXCMTouchlessController.UXEventData.UXEventType.UXEvent_CursorNotVisible)
                {
                    cursorDisplay.ChangeCursorState(RealSenseNavigator.CursorDisplay.CursorStates.Hidden);
                    outOfScreenBorder.Visibility = Visibility.Collapsed;
                }
                else if (data.type == PXCMTouchlessController.UXEventData.UXEventType.UXEvent_ReadyForAction)
                {
                    cursorDisplay.ChangeCursorState(RealSenseNavigator.CursorDisplay.CursorStates.Scroll);
                    controller.SetHandClosed(true);
                    controller.SetStartX(data.position.x - 0.5f);
                    controller.SetStartY(data.position.x - 0.5f);
                }
                else if (data.type == PXCMTouchlessController.UXEventData.UXEventType.UXEvent_EndScroll)
                {
                    cursorDisplay.ChangeCursorState(RealSenseNavigator.CursorDisplay.CursorStates.Normal);
                    controller.SetHandClosed(false);
                    controller.SetStartX(-999999999);
                    controller.SetStartY(-999999999);
                    controller.SetPreviousX(-999999999);
                    controller.SetPreviousY(-999999999);
                }

            }));
        }
        
        private void OnFiredAlertDelegate(PXCMTouchlessController.AlertData data)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                switch (data.type)
                {
                    case PXCMTouchlessController.AlertData.AlertType.Alert_TooClose:
                        Console.WriteLine("Main est trop proche.");
                        break;
                    case PXCMTouchlessController.AlertData.AlertType.Alert_TooFar:
                        Console.WriteLine("Main est trop éloignée.");
                        break;
                    case PXCMTouchlessController.AlertData.AlertType.Alert_NoAlerts:
                        Console.WriteLine("Bien positionnée.");
                        break;
                }
            }));
        }

        private void InitRsEngine()
        {
            try
            {
                rsEngine = new RealSenseEngine();
                rsEngine.UXEventFired += OnFiredUxEventDelegate;
                rsEngine.AlertFired += OnFiredAlertDelegate;
                rsEngine.SetConfiguration(GetCurrentConfiguration());
                rsEngine.Start();
                CompositionTarget.Rendering += RenderingCursor;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void RenderingCursor(object sender, EventArgs e)
        {
            cursorDisplay.Update();
        }

        private void CreateCursorDisplay()
        {
            cursorDisplay = new CursorDisplay();
            cursorDisplay.Name = "handCursor";
            cursorDisplay.IsHitTestVisible = false;
        }

        private void CreateOutOfWindowBorder()
        {
            outOfScreenBorder = new OutOfWindowBorder();
            outOfScreenBorder.Name = "outOfScreenBorder";
        }

        private TouchlessControllerConfiguration GetCurrentConfiguration()
        {
            var config = TouchlessControllerConfiguration.Configuration_Allow_Zoom;
            config |= TouchlessControllerConfiguration.Configuration_Scroll_Horizontally;
            config |= TouchlessControllerConfiguration.Configuration_Scroll_Vertically;
            config |= TouchlessControllerConfiguration.Configuration_Edge_Scroll_Horizontally;
            config |= TouchlessControllerConfiguration.Configuration_Edge_Scroll_Vertically;
            config |= TouchlessControllerConfiguration.Configuration_Allow_Back;
            config |= TouchlessControllerConfiguration.Configuration_Allow_Selection;
            return config;
        }

        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }
    }
}
