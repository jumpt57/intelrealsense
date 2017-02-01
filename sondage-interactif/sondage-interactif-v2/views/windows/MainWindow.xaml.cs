using SondageInteractifv2.controllers;
using SondageInteractifv2.database;
using SondageInteractifv2.tools.window;
using SondageInteractifv2.views.popups;
using SondageInteractifv2.views.windows;
using System;
using System.Windows;
using System.Windows.Media;

namespace SondageInteractifv2
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AppController Controller;

        /// <summary>
        /// Constructeur
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.InitializeWindow();            
            this.Controller = new AppController(this.SondageWindow);
            this.Controller.InitializeDb();
            this.Controller.InitializeQuestions();
            this.HandHelper.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Paramétrage de la fenêtre principale.
        /// </summary>
        private void InitializeWindow()
        {
            this.SondageWindow.Title = "Intel RealSense - Sondage Interactif";
            this.SondageWindow.Width = SondageWindow.MinWidth = 1024;
            this.SondageWindow.Height = SondageWindow.MinHeight = 768;
            this.SondageWindow.Closing += WindowClosing;
        }

        /// <summary>
        /// Evénement lors de la fermeture de la fenêtre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Controller.Shutdown();
            App.Current.Shutdown();
        }

        /// <summary>
        /// Permet d'afficher le pouce vers le haut.
        /// </summary>
        public void ThumbUp()
        {
            Dispatcher.Invoke((Action)(() =>
            {
                this.ThumbsUp.Fill = Brushes.LightGreen;
                this.ThumbsDown.Fill = Brushes.DimGray;
            }));
        }

        /// <summary>
        /// Permet de mettre en surbrillance le pouce vers le haut.
        /// </summary>
        public void ThumbUpHighlighted()
        {
            Dispatcher.Invoke((Action)(() =>
            {
                this.ThumbsUp.Fill = Brushes.LightGray;
                this.ThumbsDown.Fill = Brushes.DimGray;
            }));
        }

        /// <summary>
        /// Permet d'afficher le pouce vers le bas.
        /// </summary>
        public void ThumbDown()
        {
            Dispatcher.Invoke((Action)(() =>
            {
                this.ThumbsUp.Fill = Brushes.DimGray;
                this.ThumbsDown.Fill = Brushes.Red;
            }));
        }

        /// <summary>
        /// Permet de mettre en surbrillance le pouce vers le bas.
        /// </summary>
        public void ThumbDownHighlighted()
        {
            Dispatcher.Invoke((Action)(() =>
            {
                this.ThumbsUp.Fill = Brushes.DimGray;
                this.ThumbsDown.Fill = Brushes.LightGray;
            }));
        }

        /// <summary>
        /// Permet d'afficher de remettre normal les pouce.
        /// </summary>
        public void ResetThumb()
        {
            Dispatcher.Invoke((Action)(() =>
            {
                this.ThumbsUp.Fill = Brushes.DimGray;
                this.ThumbsDown.Fill = Brushes.DimGray;
            }));
        }

        /// <summary>
        /// Feedback lors lorcequ'on ne détecte pas la main.
        /// </summary>
        public void HandNotDetected()
        {
            Dispatcher.Invoke((Action)(() =>
            {
                this.HandDetection.Background = Brushes.Red;               
            }));
        }

        /// <summary>
        /// Feedback lors lorcequ'on détecte la main.
        /// </summary>
        public void HandDetected()
        {
            Dispatcher.Invoke((Action)(() =>
            {
                this.HandDetection.Background = Brushes.Green;
            }));
        }

        /// <summary>
        /// Feedback pour aider à bien placer sa main.
        /// </summary>
        public void ShowHandHelper(string msg)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                this.HandHelper.Visibility = Visibility.Visible;
                this.LabelHandHelper.Content = msg;              
            }));
        }

        /// <summary>
        /// Cache l'aide.
        /// </summary>
        public void HideHandHelper()
        {
            Dispatcher.Invoke((Action)(() =>
            {
                this.HandHelper.Visibility = Visibility.Collapsed;
            }));
        }

        /// <summary>
        /// Change le texte de la question.
        /// </summary>
        public void ChangeQuestion(string question)
        {            
            Dispatcher.Invoke((Action)(() =>
            {
                Question.Content = question;
            }));                          
        }

        /// <summary>
        /// Lorsce qu'on ferme l'app.
        /// </summary>
        private void AppExit_Click(object sender, RoutedEventArgs e)
        {
            this.Controller.Shutdown();
            this.Close();
        }

        /// <summary>
        /// Affichage des infos de l'app.
        /// </summary>
        private void About_Click(object sender, RoutedEventArgs e)
        {
            var popup = new VersionPopup();
            popup.Activated += OpeningPopup;
            popup.Closing += ClosingPopup;
            popup.Show();
        }

        /// <summary>
        /// Affichage de la fenêtre de gestion des questions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManageQuestion_Click(object sender, RoutedEventArgs e)
        {
            var popup = new QuestionManager(this.Controller);
            popup.Activated += OpeningPopup;
            popup.Closing += ClosingPopup;
            popup.Show();
        }

        /// <summary>
        /// Permet de désactiver la détection de la main lors de l'ouverture
        /// d'une popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OpeningPopup(object sender, EventArgs e)
        {
            this.Controller.Pause();
            Dispatcher.Invoke((Action)(() =>
            {
                HandDetection.Background = Brushes.Orange;
            }));
        }

        /// <summary>
        /// Permet de reactiver la détection de la main après la fermeture
        /// d'une popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ClosingPopup(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Controller.InitializeQuestions();
            this.Controller.Pursue();
            this.HandNotDetected();
        }

    }

}