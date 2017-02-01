using SondageInteractifv2.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SondageInteractifv2.tools.file
{
    /// <summary>
    /// Classe utilitaire qui permet de transformer une listeview
    /// de question en texte formaté en CSV.
    /// </summary>
    public class ListViewToCSV
    {

        public static string ProcessQuestion(ListView ListView)
        {
            GridView gridView = (GridView)ListView.View;
            string[] headers = new string[gridView.Columns.Count];

            for (int i = 0; i < headers.Length; i++)
            {
                headers[i] = gridView.Columns[i].Header.ToString();
            }

            string[,] items = new string[ListView.Items.Count, gridView.Columns.Count];
            for (int i = 0; i < items.GetLength(0); i++)
            {
                var  Question = (Question) ListView.Items.GetItemAt(i);

                for (var l = 0; l < gridView.Columns.Count; l++)
                {
                    items[i, l] = Question.ToArrayString()[l];
                }
            }

            string table = string.Join(",", headers) + Environment.NewLine;

            int Column = 0;
            foreach (string a in items)
            {
                table += a + ",";

                if (Column == gridView.Columns.Count)
                {
                    table += a + Environment.NewLine; ;
                    Column = 0;
                }

                Column++;
            }
            table = table.TrimEnd('\r', '\n');

            return table;
        }


    }
}
