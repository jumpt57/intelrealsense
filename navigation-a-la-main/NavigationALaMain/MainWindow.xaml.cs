using NavigationALaMain.tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using TouchlessControllerConfiguration = PXCMTouchlessController.ProfileInfo.Configuration;

namespace NavigationALaMain
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Random rnd = new Random();
        private RealSenseEngine m_rsEngine;
        private List<Button> buttons;
        private int[,] patternToFind;
        private int[,] ghost;
        private int hintFind;   

        #region Main
        public MainWindow()
        {
            InitializeComponent();
            InitRsEngine();
            CreateButtons();
            CenterWindowOnScreen();
            InitPatternToFind();
            InitHandCursor();
        }       
        #endregion

        #region Initialisation des composants RealSense
        private void  InitRsEngine()
        {
            try
            {
                m_rsEngine = new RealSenseEngine();
                m_rsEngine.UXEventFired += OnFiredUxEventDelegate;
                m_rsEngine.AlertFired += OnFiredAlertDelegate;
                m_rsEngine.SetConfiguration(GetCurrentConfiguration());
                m_rsEngine.Start();
                CompositionTarget.Rendering += CompositionTarget_Rendering;
                TB_RealSenseState.Text = "Chargement des modules RealSense OK";
            }
            catch (Exception e)
            {
                TB_RealSenseState.Text = "Erreur de chargement des modules RealSense : " + e.Message;              
            }
        }
        #endregion

        #region Détection de la position de la main et affichage de message
        private void OnFiredAlertDelegate(PXCMTouchlessController.AlertData data)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                switch (data.type)
                {
                    case PXCMTouchlessController.AlertData.AlertType.Alert_TooClose:
                        TB_HandDetectionInfo.Text = "Main est trop proche.";                        
                        break;
                    case PXCMTouchlessController.AlertData.AlertType.Alert_TooFar:
                        TB_HandDetectionInfo.Text = "Main est trop éloignée.";
                        break;
                    case PXCMTouchlessController.AlertData.AlertType.Alert_NoAlerts:
                        TB_HandDetectionInfo.Text = "Bien positionnée.";
                        break;
                }
            }));
        }
        #endregion

        #region Configuration des actions possibles avec la main
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
        #endregion

        #region Événement de fermeture de la fenêtre
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (m_rsEngine != null)
            {
                m_rsEngine.Shutdown();
            }
        }
        #endregion

        #region Permet de mettre à jour la position du curseur à chaque événement
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            handCursor.Update();
        }
        #endregion

        #region Détection de la position de la main
        private void OnFiredUxEventDelegate(PXCMTouchlessController.UXEventData data)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                handCursor.SetPosition(data.position.x, data.position.y, data.position.z,
                    data.bodySide == PXCMHandData.BodySideType.BODY_SIDE_RIGHT);

                // Permet de détecter si le curseur est hors de la fenêtre
                outOfScreenBorder.X = data.position.x - 0.5f;
                outOfScreenBorder.Y = data.position.y - 0.5f;

                switch (data.type)
                {
                    // Détection de la main à l'entrée dans la fenêtre
                    case PXCMTouchlessController.UXEventData.UXEventType.UXEvent_CursorVisible:
                        handCursor.ChangeCursorState(RealSenseNavigator.CursorDisplay.CursorStates.Normal);
                        handCursor.ChangeCursorState(RealSenseNavigator.CursorDisplay.CursorStates.Visible);
                        outOfScreenBorder.Visibility = Visibility.Visible;
                        break;
                    // Détection de la main si elle de la fenêtre
                    case PXCMTouchlessController.UXEventData.UXEventType.UXEvent_CursorNotVisible:
                        handCursor.ChangeCursorState(RealSenseNavigator.CursorDisplay.CursorStates.Hidden);
                        outOfScreenBorder.Visibility = Visibility.Collapsed;                        
                        break;
                    // Action à la fermeture de la main
                    case PXCMTouchlessController.UXEventData.UXEventType.UXEvent_ReadyForAction:
                        handCursor.ChangeCursorState(RealSenseNavigator.CursorDisplay.CursorStates.Scroll);                       
                        CheckButtonClicked(data.position);                       
                        break;
                    // Action re-ouverture de la main
                    case PXCMTouchlessController.UXEventData.UXEventType.UXEvent_EndScroll:
                        handCursor.ChangeCursorState(RealSenseNavigator.CursorDisplay.CursorStates.Normal);
                        break;
                    // Repérage du mouvement de la main
                    case PXCMTouchlessController.UXEventData.UXEventType.UXEvent_CursorMove:
                        UpdateCursorPositionText(data.position);
                        break;
                    // Détection d'un wafe 
                    case PXCMTouchlessController.UXEventData.UXEventType.UXEvent_GotoStart:
                        handCursor.ChangeCursorState(RealSenseNavigator.CursorDisplay.CursorStates.Wave);
                        handCursor.ChangeCursorState(RealSenseNavigator.CursorDisplay.CursorStates.Normal);
                        OnWave();
                        break;

                }

            }));
        }
        #endregion

        #region Gestion de la position du cuseur
        private void UpdateCursorPositionText(PXCMPoint3DF32 pos)
        {
            double height = VisualFeedbackGrid.ActualHeight;
            double width = VisualFeedbackGrid.ActualWidth;
            var p = new PXCMPoint3DF32((float)(width * pos.x), (float)(height * pos.y), pos.z);
            UpdateGestureText(String.Format("Gesture: Navigation [Hand at {0}]", p.ToFormattedString()));
        }

        private void UpdateGestureText(string s)
        {
            TB_HandPositionInfo.Text = s;
        }
        #endregion

        #region Permet de savoir quel bouton a été cliqué
        private void CheckButtonClicked(PXCMPoint3DF32 pos)
        {
            Point point = new Point();
            point.X = Math.Max(Math.Min(0.9F, pos.x), 0.1F);
            point.Y = Math.Max(Math.Min(0.9F, pos.y), 0.1F);

            foreach (Button button in buttons)
            {
                if (HelperMethods.PointOnButton(point, button, VisualFeedbackGrid))
                {

                   if( ValidateMovement(Grid.GetColumn(button), Grid.GetRow(button))){
                        TB_IsClicked.Text = button.Name + " is clicked";

                        var solidColorBrush = new SolidColorBrush();
                        solidColorBrush.Opacity = 0.25;
                        solidColorBrush.Color = Color.FromRgb(95, 158, 160);
                        button.Background = solidColorBrush;

                        if (hintFind == 0)
                        {
                            var popup = new TimedPopUp("End", "Fin", new TimeSpan(0, 0, 2));
                            popup.Show();
                        }

                    }

                    break;
                }
            }

        }
        #endregion

        #region Après avoir trouvé le bon bouton validation de la position dans le tableau     
        private Boolean ValidateMovement(int c, int l)
        {
            if (patternToFind[c, l] == 1 && ghost[c,l] == 0)
            {
                ghost[c, l] = 1;
                hintFind--;
                return true;
            }

            return false;
        }
        #endregion

        #region Permet de créer tous les boutons
        public void CreateButtons()
        {
            buttons = new List<Button>();
            for (int rowCounter = 0; rowCounter < 3; rowCounter++)
            {
                for (int colCounter = 0; colCounter < 3; colCounter++)
                {

                    var button = new Button();
                    button.Name = "BT_L" + rowCounter + "C" + colCounter;
                    button.Height = 200;
                    button.Width = 200;
                    button.Content = "";
                    button.HorizontalContentAlignment = HorizontalAlignment.Center;
                    button.VerticalContentAlignment = VerticalAlignment.Center;
                    var solidColorBrush = new SolidColorBrush();
                    solidColorBrush.Opacity = 0.25;
                    solidColorBrush.Color = Color.FromRgb(100,149,237);
                    button.Background = solidColorBrush;

                    buttons.Add(button);

                    Grid.SetRow(button, rowCounter);
                    Grid.SetColumn(button, colCounter);                    
                                     
                    VisualFeedbackGrid.Children.Add(button);
                }
            }                
        }
        #endregion

        #region Permet de centrer la fenêtre au démarrage
        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }
        #endregion

        #region Initialise le tableau de réponses
        private void InitPatternToFind()
        {
            patternToFind = new int[3, 3];
            ghost = new int[3, 3];

            bool okey = false;
          
            while (!okey && hintFind < 3)
            {
                
                var c = rnd.Next(0, 3);
                var l = rnd.Next(0, 3);

                if (patternToFind[c, l] != 1)
                {
                    patternToFind[c, l] = 1;
                    hintFind++;
                }
            }

        }
        #endregion

        #region Permet de cacher la main au démarrage de l'application
        private void InitHandCursor()
        {
            handCursor.ChangeCursorState(RealSenseNavigator.CursorDisplay.CursorStates.Hidden);
        }
        #endregion

        #region Permet de reset la grille et la patern
        private void OnWave()
        {
            ResetButtons();
            InitPatternToFind();
        }
        #endregion

        #region Permet de reset la couleur des boutons
        private void ResetButtons()
        {
            foreach (Button button in buttons)
            {

                var solidColorBrush = new SolidColorBrush();
                solidColorBrush.Opacity = 0.25;
                solidColorBrush.Color = Color.FromRgb(100, 149, 237);
                button.Background = solidColorBrush;

            }
        }
        #endregion

       }
}