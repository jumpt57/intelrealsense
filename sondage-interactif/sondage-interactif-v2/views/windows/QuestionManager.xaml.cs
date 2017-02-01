using System.Windows;
using SondageInteractifv2.database;
using SondageInteractifv2.models;
using System.Collections.ObjectModel;
using SondageInteractifv2.views.popups;
using System.Windows.Controls;
using Microsoft.Win32;
using System;
using SondageInteractifv2.controllers;

namespace SondageInteractifv2.views.windows
{
    /// <summary>
    /// Logique d'interaction pour QuestionManager.xaml
    /// </summary>
    public partial class QuestionManager : Window
    {
        private AppController Controller;

        private ObservableCollection<Question> QuestionListe;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="Controller">Controller principal de l'application</param>
        public QuestionManager(AppController Controller)
        {
            this.Controller = Controller;
            this.InitializeQuestions();
            this.InitializeComponent();
            this.UpdatePanel.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Permet de récupérer l'ensemble des questions dans une collection bindée
        /// à l'interface WPF.
        /// </summary>
        private void InitializeQuestions()
        {
            if (this.QuestionListe == null)
            {
                this.QuestionListe = new ObservableCollection<Question>();
            }
            else
            {
                this.QuestionListe.Clear();
            }           

            foreach (Question Question in this.Controller.GetDbManager().GetQuestions())
            {
                this.QuestionListe.Add(Question);
            }
        }

        /// <summary>
        /// Accès à la liste pour le binding WPF.
        /// </summary>
        public ObservableCollection<Question> GetQuestionListe
        {
            get {
                return QuestionListe;
            }
        }

        /// <summary>
        /// Ferme la fenêtre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fermer_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Permet d'enregistrer une nouvelle question.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            if (this.NouvelleQuestion.Text.Length > 20)
            {
                Question LastItem = (Question)this.QuestionsListe.Items.GetItemAt(this.QuestionsListe.Items.Count - 1);
                this.Controller.GetDbManager().CreateQuestion(NouvelleQuestion.Text, LastItem.Id, LastItem.Position);
                this.NouvelleQuestion.Text = "";
                this.InitializeQuestions();
            }else
            {
                new ErrorPopup("Question trop courte !").Show();
            }           
        }

        /// <summary>
        /// Supprime une question.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            Question SelectedQuestion = (Question) this.QuestionsListe.SelectedItem;
            if (SelectedQuestion != null)
            {
                this.Controller.GetDbManager().DeleteQuestion(SelectedQuestion.Id);
                this.InitializeQuestions();
            }
        }
        
        /// <summary>
        /// Permet d'afficher le popup de modification d'une question
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Modifier_Click(object sender, RoutedEventArgs e)
        {  
            if (this.QuestionsListe.SelectedItems.Count == 1)
            {
                Question SelectedQuestion = (Question) this.QuestionsListe.SelectedItem;
                this.QuestionsListe.IsEnabled = false;
                this.UpdatePanel.Visibility = Visibility.Visible;
                this.QuestionModif.Text = SelectedQuestion.LaQuestion;                
            }            
        }

        /// <summary>
        /// Change la position d'une question vers le haut.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Monter_Click(object sender, RoutedEventArgs e)
        {
            if (this.QuestionsListe.SelectedItems.Count == 1)
            {
                Question SelectedQuestion = (Question) this.QuestionsListe.SelectedItem;
                var Index = this.QuestionsListe.SelectedIndex;
                if (Index > 0)
                {
                    var Position = SelectedQuestion.Position;
                    Question QuestionAuDessus = (Question)this.QuestionsListe.Items.GetItemAt(Index - 1);

                    SelectedQuestion.Position = Position - 1;
                    QuestionAuDessus.Position = Position;

                    this.Controller.GetDbManager().UpdateQuestionPosition(SelectedQuestion.Id, 999998);
                    this.Controller.GetDbManager().UpdateQuestionPosition(QuestionAuDessus.Id, 999999);

                    this.Controller.GetDbManager().UpdateQuestionPosition(SelectedQuestion.Id, SelectedQuestion.Position);
                    this.Controller.GetDbManager().UpdateQuestionPosition(QuestionAuDessus.Id, QuestionAuDessus.Position);

                    this.InitializeQuestions();

                    this.QuestionsListe.SelectedIndex = Index - 1;
                }
            }
        }

        /// <summary>
        /// Change la position d'une question vers le bas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Descendre_Click(object sender, RoutedEventArgs e)
        {
            if (this.QuestionsListe.SelectedItems.Count == 1)
            {
                Question SelectedQuestion = (Question)this.QuestionsListe.SelectedItem;
                var Index = this.QuestionsListe.SelectedIndex;
                if (Index < this.QuestionsListe.Items.Count - 1)
                {
                    var Position = SelectedQuestion.Position;
                    Question QuestionEnDessous = (Question)this.QuestionsListe.Items.GetItemAt(Index + 1);

                    SelectedQuestion.Position = Position + 1;
                    QuestionEnDessous.Position = Position;

                    this.Controller.GetDbManager().UpdateQuestionPosition(SelectedQuestion.Id, 999998);
                    this.Controller.GetDbManager().UpdateQuestionPosition(QuestionEnDessous.Id, 999999);

                    this.Controller.GetDbManager().UpdateQuestionPosition(SelectedQuestion.Id, SelectedQuestion.Position);
                    this.Controller.GetDbManager().UpdateQuestionPosition(QuestionEnDessous.Id, QuestionEnDessous.Position);

                    this.InitializeQuestions();

                    this.QuestionsListe.SelectedIndex = Index + 1;
                }
            }
        }

        /// <summary>
        /// Crée un fichier avec les résultats.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exporter_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Title = "Choose file to save to",
                FileName = "example.csv",
                Filter = "CSV (*.csv)|*.csv",
                FilterIndex = 0,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (sfd.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(sfd.FileName, this.Controller.QuestionToCSV(this.QuestionsListe));
            }
        }

        /// <summary>
        /// Fermer le Popup de modification d'une question
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.UpdatePanel.Visibility = Visibility.Collapsed;
            this.QuestionsListe.IsEnabled = true;
        }

        /// <summary>
        /// Permet de valider la modification d'une question dans le popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifierBtn_Click(object sender, RoutedEventArgs e)
        {
            Question SelectedQuestion = (Question) this.QuestionsListe.SelectedItem;
            this.Controller.GetDbManager().UpdateQuestion(SelectedQuestion.Id, QuestionModif.Text);
            this.UpdatePanel.Visibility = Visibility.Collapsed;
            this.QuestionsListe.IsEnabled = true;
            this.InitializeQuestions();
        }

        
    }
}
