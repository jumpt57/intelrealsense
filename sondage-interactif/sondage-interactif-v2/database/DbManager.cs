using Npgsql;
using SondageInteractifv2.models;
using System;
using System.Collections.Generic;

namespace SondageInteractifv2.database
{
    /// <summary>
    /// Classe permettant d'interagir avec la bdd
    /// </summary>
    public class DbManager
    {
        public static String DB_URL = "localhost";
        public static String DB_PORT = "5432";
        public static String DB_PASSWORD = "sondage";
        public static String DB_USER = "sondage";
        public static String DB_NAME = "sondage";

        private NpgsqlConnection NpgsqlConnection;

        /// <summary>
        /// Permet de créer une connexion
        /// </summary>
        public DbManager()
        {
            try
            {
                string connstring = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                   DB_URL, DB_PORT, DB_USER, DB_PASSWORD, DB_NAME);
                NpgsqlConnection = new NpgsqlConnection(connstring);
                NpgsqlConnection.Open();
            }
            catch (Exception)
            {
                throw new Exception("Problème de création d'un SQL Connection !");
            }
            
        }

        /// <summary>
        /// Récupère toutes les questions
        /// </summary>
        /// <returns></returns>
        public List<Question> GetQuestions()
        {
            try
            {
                var cmd = new NpgsqlCommand();
                cmd.Connection = NpgsqlConnection;
                List<Question> Questions = new List<Question>();
                cmd.CommandText = "SELECT * FROM question ORDER BY position;";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var Question = new Question(reader.GetInt16(0), reader.GetString(1), 
                        reader.GetInt16(2), reader.GetInt16(3), reader.GetInt32(4));
                    Questions.Add(Question);
                }
                reader.Close();
                cmd.Cancel();
                return Questions;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("Problème lors de la requête de récupération des questions !");
            }            
        }

        /// <summary>
        /// Permet de créer une question
        /// </summary>
        /// <param name="LaQuestion">Le texte de la question</param>
        /// <param name="Id">Le plus grand Id</param>
        /// <param name="Pos">La plus grande position</param>
        public void CreateQuestion(string LaQuestion, int Id, int Pos)
        {
            try
            {
                var cmd = new NpgsqlCommand();
                cmd.Connection = NpgsqlConnection;
                var escaped = LaQuestion.Replace("'", "''");
                List<Question> Questions = new List<Question>();
                var sqlQuery = String.Format("INSERT INTO question VALUES ({0}, '{1}', 0, 0, {2});", Id + 1, escaped, Pos + 1);
                cmd.CommandText = sqlQuery;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("Problème lors de la requête de création d'une question !");
            }
        }

        /// <summary>
        /// Supprime une question
        /// </summary>
        /// <param name="Id">L'Id d'une question</param>
        public void DeleteQuestion(int Id)
        {
            try
            {
                var cmd = new NpgsqlCommand();
                cmd.Connection = NpgsqlConnection;
                List<Question> Questions = new List<Question>();
                var sqlQuery = String.Format("DELETE FROM question WHERE id = {0};", Id);
                cmd.CommandText = sqlQuery;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("Problème lors de la requête de suppression d'une question !");
            }
        }

        /// <summary>
        /// Permet de mettre à jour la texte d'une question
        /// </summary>
        /// <param name="Id">L'id de la question</param>
        /// <param name="Question">Le texte de la question</param>
        public void UpdateQuestion(int Id, String Question)
        {
            try
            {
                var cmd = new NpgsqlCommand();
                cmd.Connection = NpgsqlConnection;
                var escaped = Question.Replace("'", "''");
                List<Question> Questions = new List<Question>();
                var sqlQuery = String.Format("UPDATE question SET question = '{1}' WHERE id = {0};", Id, escaped);
                cmd.CommandText = sqlQuery;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("Problème lors de la requête de modification d'une question !");
            }
        }

        /// <summary>
        /// Permet d'ajouter un vote positif à une question
        /// </summary>
        /// <param name="Id">L'id de la question</param>
        /// <param name="Positif"></param>
        public void UpdateQuestionPositif(int Id, int Positif)
        {
            try
            {
                var cmd = new NpgsqlCommand();
                cmd.Connection = NpgsqlConnection;
                List<Question> Questions = new List<Question>();
                var sqlQuery = String.Format("UPDATE question SET positif = '{1}' WHERE id = {0};", Id, Positif);
                cmd.CommandText = sqlQuery;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("Problème lors de la requête de modification d'une question !");
            }
        }

        /// <summary>
        /// Permet d'ajouter un vote negatif à une question
        /// </summary>
        /// <param name="Id">L'id de la question</param>
        /// <param name="Negatif"></param>
        public void UpdateQuestionNegatif(int Id, int Negatif)
        {
            try
            {
                var cmd = new NpgsqlCommand();
                cmd.Connection = NpgsqlConnection;
                List<Question> Questions = new List<Question>();
                var sqlQuery = String.Format("UPDATE question SET negatif = '{1}' WHERE id = {0};", Id, Negatif);
                cmd.CommandText = sqlQuery;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("Problème lors de la requête de modification d'une question !");
            }
        }

        /// <summary>
        /// Permet de mettre à jour la position d'une question
        /// </summary>
        /// <param name="Id">L'id de la question</param>
        /// <param name="Position"></param>
        public void UpdateQuestionPosition(int Id, int Position)
        {
            try
            {
                var cmd = new NpgsqlCommand();
                cmd.Connection = NpgsqlConnection;
                List<Question> Questions = new List<Question>();
                var sqlQuery = String.Format("UPDATE question SET position = '{1}' WHERE id = {0};", Id, Position);
                cmd.CommandText = sqlQuery;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("Problème lors de la requête de modification de la position d'une question !");
            }
        }
    }
}
