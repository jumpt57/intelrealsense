using SondageInteractifv2.database;
using SondageInteractifv2.models;
using SondageInteractifv2.tools;
using SondageInteractifv2.tools.gesture;
using SondageInteractifv2.views.popups;
using System;
using System.Collections.Generic;
using static PXCMHandData;
using System.Timers;
using System.Windows.Controls;
using SondageInteractifv2.tools.file;

namespace SondageInteractifv2.controllers
{
    /// <summary>
    /// Classe qui permet de faire le lien entre
    /// les événements de la caméra, le feedback visuel
    /// et la bdd.
    /// 
    /// </summary>
    public class AppController
    {
        public static string GESTURE_NONE = "NONE";
        public static string MSG_NO_QUESTION = "Il n'y a pas de question !";
        public static string MSG_NO_HOLD_GESTURE = "Tenez la position !";
        public static string MSG_NEXT_QUESTION = "Changement de question.";
        public static string MSG_NO_SURE = "Geste non précis !";

        private readonly RealSenseEngine Engine;
        private readonly MainWindow SondageWindow;
        private DbManager DbManager;
        private List<Question> Questions;
        private List<Question>.Enumerator Enum;
        private Question SelectedQuestion;      
        private Timer MyTimer;
        private string PreviousGesture;
        private bool ValidationEncours;
       

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="SondageWindow"></param>
        public AppController(MainWindow SondageWindow)
        {
            this.Engine = new RealSenseEngine(this);
            this.SondageWindow = SondageWindow;

            this.MyTimer = new Timer();
            this.MyTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            this.MyTimer.Interval = 1000;
            this.ValidationEncours = false;
            this.PreviousGesture = GESTURE_NONE;
        }

        /// <summary>
        /// Permet de créer l'objet pour manipuler la BDD
        /// </summary>
        public void InitializeDb()
        {
            try
            {
                this.DbManager = new DbManager();
            }
            catch (Exception e)
            {
                new ErrorPopup(e.Message).Show();
            }
        }

        /// <summary>
        /// Permet de récupérer toutes les questions
        /// et de sélectionner la première
        /// </summary>
        public void InitializeQuestions()
        {
            try
            {
                this.Questions = this.DbManager.GetQuestions();
                this.Enum = this.Questions.GetEnumerator();
                this.Enum.MoveNext();
                this.SelectedQuestion = this.Enum.Current;

                if (this.SelectedQuestion == null)
                {
                    this.SelectedQuestion = null;
                    this.SondageWindow.ChangeQuestion(MSG_NO_QUESTION);
                    this.SondageWindow.OpeningPopup(null, null);
                    return;
                }

                this.SondageWindow.ChangeQuestion(this.SelectedQuestion.LaQuestion);
            }
            catch (Exception e)
            {
                new ErrorPopup(e.Message).Show();
            }
        }

        /// <summary>
        /// Permet de sélection une nouvelle question
        /// </summary>
        public void NextQuestion()
        {
            this.Pause();

            this.Enum.MoveNext();
            this.SelectedQuestion = Enum.Current;
            if (this.SelectedQuestion == null)
            {
                this.Enum.Dispose();
                this.Enum = this.Questions.GetEnumerator();
                this.NextQuestion();
            }

            this.SondageWindow.ChangeQuestion(this.SelectedQuestion.LaQuestion);
            this.Pursue();
        }

        /// <summary>
        /// Quand l'utilisateur fait un pouce vers le haut
        /// </summary>
        private void IncrementerPositif()
        {
            this.SondageWindow.ThumbUp();
            this.SelectedQuestion.Positif++;
            this.DbManager.UpdateQuestionPositif(this.SelectedQuestion.Id, this.SelectedQuestion.Positif);
            this.NextQuestion();
        }

        /// <summary>
        /// Quand l'utilisateur fait un pouce vers le bas
        /// </summary>
        private void IncrementerNegatif()
        {
            this.SondageWindow.ThumbDown();
            this.SelectedQuestion.Negatif++;
            this.DbManager.UpdateQuestionNegatif(this.SelectedQuestion.Id, this.SelectedQuestion.Negatif);
            this.NextQuestion();
        }

        /// <summary>
        /// Permet de gérer les événements de gestes
        /// de reconnaissance de la main.
        /// </summary>
        /// <param name="Alert">Un geste</param>
        public void Action(string Action)
        {
            
            if (Action.Equals(HandDataType.GESTURE_HAND_THUMB_UP) && !this.PreviousGesture.Equals(HandDataType.GESTURE_HAND_THUMB_UP) && !this.ValidationEncours)
            {
                this.SondageWindow.ShowHandHelper(MSG_NO_HOLD_GESTURE);
                this.MyTimer.Stop();
                this.SondageWindow.ThumbUpHighlighted();
                this.PreviousGesture = Action;
                this.MyTimer.Enabled = true;
                return;
            }
            else if (Action.Equals(HandDataType.GESTURE_HAND_THUMB_DOWN) && !this.PreviousGesture.Equals(HandDataType.GESTURE_HAND_THUMB_DOWN) && !this.ValidationEncours)
            {
                this.SondageWindow.ShowHandHelper(MSG_NO_HOLD_GESTURE);
                this.MyTimer.Stop();
                this.SondageWindow.ThumbDownHighlighted();
                this.PreviousGesture = Action;
                this.MyTimer.Enabled = true;
                return;
            }
          
        }

        /// <summary>
        /// Méthode déclanchée à la fin du timer
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {           
            this.ValidationEncours = true;
            this.MyTimer.Stop();

            if (this.PreviousGesture.Equals(HandDataType.GESTURE_HAND_THUMB_UP)) this.IncrementerPositif();
            if (this.PreviousGesture.Equals(HandDataType.GESTURE_HAND_THUMB_DOWN)) this.IncrementerNegatif();

            this.SondageWindow.ShowHandHelper(MSG_NEXT_QUESTION);
            System.Threading.Thread.Sleep(2000);
            this.PreviousGesture = GESTURE_NONE;
            this.SondageWindow.HideHandHelper();
            this.SondageWindow.ResetThumb();
           
            this.ValidationEncours = false;                        
        }

        /// <summary>
        /// Permet de gérer les événements d'informations
        /// de reconnaissance de la main.
        /// </summary>
        /// <param name="Alert">Une information</param>
        public void Alert(AlertType Alert)
        {
            switch (Alert)
            {
                case PXCMHandData.AlertType.ALERT_HAND_TRACKED:
                case PXCMHandData.AlertType.ALERT_HAND_INSIDE_BORDERS:
                case PXCMHandData.AlertType.ALERT_HAND_DETECTED:
                    this.SondageWindow.HandDetected();
                    break;

                case PXCMHandData.AlertType.ALERT_HAND_TOO_FAR:
                case PXCMHandData.AlertType.ALERT_HAND_TOO_CLOSE:
                case PXCMHandData.AlertType.ALERT_HAND_OUT_OF_BORDERS:
                case PXCMHandData.AlertType.ALERT_HAND_NOT_DETECTED:                   
                    this.SondageWindow.HandNotDetected();
                    this.SondageWindow.ResetThumb();                                        
                    this.PreviousGesture = GESTURE_NONE;                    
                    this.SondageWindow.HideHandHelper();
                    this.MyTimer.Stop();
                    break;

                case PXCMHandData.AlertType.ALERT_HAND_LOW_CONFIDENCE:
                    this.SondageWindow.ShowHandHelper(MSG_NO_SURE);
                    this.SondageWindow.ResetThumb();
                    this.PreviousGesture = GESTURE_NONE;                    
                    this.MyTimer.Stop();
                    break;
            }
        }

        /// <summary>
        /// Permet de transformer les données d'un listview de
        /// questions en csv. 
        /// </summary>
        /// <param name="ListView"></param>
        /// <returns></returns>
        public string QuestionToCSV(ListView ListView)
        {
            return ListViewToCSV.ProcessQuestion(ListView);
        }

        /// <summary>
        /// Permet d'éteindre la caméra.
        /// </summary>
        public void Shutdown()
        {
            this.Engine.Shutdown();
        }

        /// <summary>
        /// Permet de mettre en pause la
        /// caméra.
        /// </summary>
        public void Pause()
        {
            this.Engine.Pause();
        }

        /// <summary>
        /// Permet d'enlever la pause
        /// de la caméra.
        /// </summary>
        public void Pursue()
        {
            this.Engine.Pursue();
        }

        /// <summary>
        /// Getter du DbManager
        /// </summary>
        /// <returns></returns>
        public DbManager GetDbManager()
        {
            return this.DbManager;
        }
    }
}
