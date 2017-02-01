using RealSenseNavigator;
using schema_android_like.controller;
using schema_android_like.tools;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TouchlessControllerConfiguration = PXCMTouchlessController.ProfileInfo.Configuration;

namespace schema_android_like
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int GRID_LENGTH = 3;
        
        private SchemaController controller;

        private RealSenseEngine rsEngine;
        private CursorDisplay cursorDisplay;
        private OutOfWindowBorder outOfScreenBorder;

        public MainWindow()
        {
            InitializeComponent();  
                      
            InitRealSenseUi();
            InitRsEngine();
            InitWindow();
            InitGrid();
            InitButtons();            
        }        

        private void InitWindow()
        {
            SchemaWindow.Title = "Intel RealSense - Android Unlock POC";

            SchemaWindow.Width = 1000;
            SchemaWindow.Height = 1000;
            SchemaWindow.MinWidth = 1000;
            SchemaWindow.MinHeight = 1000;
            SchemaWindow.MaxWidth = 1000;
            SchemaWindow.MaxHeight = 1000;

            SchemaWindow.Closing += WindowClosing;

            SchemaWindow.Background = Brushes.LightGray;

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
            for (int i = 0; i < GRID_LENGTH; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(1.0, GridUnitType.Star);
                SchemaGrid.RowDefinitions.Add(row);

                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(1.0, GridUnitType.Star); 
                SchemaGrid.ColumnDefinitions.Add(column);               
            }
        }

        private void InitButtons()
        {
            controller = new SchemaController();

            for (int l = 0; l < GRID_LENGTH; l++)
            {
                for (int c = 0; c < GRID_LENGTH; c++)
                {      
                    var forme = controller.CreateFormeRonde(c, l).Get();
                    Grid.SetRow(forme, l);
                    Grid.SetColumn(forme, c);
                    SchemaGrid.Children.Add(forme);
                }
            }
            controller.CreateSchemaToFind();
        }

        private void InitRealSenseUi()
        {
            CreateCursorDisplay();
            CreateOutOfWindowBorder();

            Grid.SetRow(cursorDisplay, 0);
            Grid.SetColumn(cursorDisplay, 0);
            Grid.SetRowSpan(cursorDisplay, 3);
            Grid.SetColumnSpan(cursorDisplay, 3);

            Grid.SetRow(outOfScreenBorder, 0);
            Grid.SetColumn(outOfScreenBorder, 0);
            Grid.SetRowSpan(outOfScreenBorder, 3);
            Grid.SetColumnSpan(outOfScreenBorder, 3);

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

                controller.CheckCollision(data.position, SchemaGrid);
                               
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
                }
                else if(data.type == PXCMTouchlessController.UXEventData.UXEventType.UXEvent_EndScroll)
                {
                    cursorDisplay.ChangeCursorState(RealSenseNavigator.CursorDisplay.CursorStates.Normal);
                    controller.SetHandClosed(false);
                    controller.ResetFormes();
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
