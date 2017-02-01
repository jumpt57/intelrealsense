using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SondageInteractifv2.models
{
    /// <summary>
    /// Classe qui représente une question
    /// </summary>
    public class Question
    {
        public int Id { get; set; }
        public String LaQuestion { get; set; }
        public int Positif { get; set; }
        public int Negatif { get; set; }
        public int Position { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="Id">L'Id d'une question</param>
        /// <param name="LaQuestion">Le texte de la question</param>
        /// <param name="Positif">Le nombre de vote positif</param>
        /// <param name="Negatif">Le nombre de vote negatif</param>
        /// <param name="Position">La position de la question dans la liste</param>
        public Question(int Id, String LaQuestion, int Positif, int Negatif, int Position)
        {
            this.Id = Id;
            this.LaQuestion = LaQuestion;
            this.Positif = Positif;
            this.Negatif = Negatif;
            this.Position = Position;
        }

        /// <summary>
        /// Permet de transformer la question en tableau de string
        /// facilite la transformation en csv.
        /// </summary>
        /// <returns></returns>
        public string[] ToArrayString()
        {
            string[] arr1 = { Id.ToString(), Position.ToString(), LaQuestion, Positif.ToString(), Negatif.ToString() };

            return arr1;
        }


    }
}
