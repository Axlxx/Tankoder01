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
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Threading;

namespace TankArena
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        List<string> questions = new List<string>();
        List<string> A1 = new List<string>();
        List<string> A2 = new List<string>();
        List<string> A3 = new List<string>();
        List<string> Right = new List<string>();

        int myRand; 

        bool corect;

        public Window1()
        {
            InitializeComponent();
            {
            String question1 = "What animal is Bambi?";
            String question2 = "What currency does London use?";
            String question3 = "How much is 2+2*6+3*0";
            String question4 = "What currency does Japan use?";
            String question5 = "What states the first amendament of the USA constitution?";
            String question6 = "Who was the first president of USA?";
            String question7 = "In what year did the First World War start?";
            String question8 = "Who was Nikola Tesla?";
            String question9 = "Who stated the Theory of relativity?";
            String question10 = "When was the first Nobel prize given?";
            String question11 = "Which is the biggest ocean?";
            String question12 = "Whitch melody plays in Shrek?";
            String question13 = "What is the Capital of Constantinopol?";
            String question14 = "What is the religion of the Dalai Lama?";
            String question15 = "Which planet did Superman come from?";
            String question16 = "Who is Al Pacino ?";
            String question17 = "Who was the Greek or Roman God of War ? ";

                questions.Add(question1);
                questions.Add(question2);
                questions.Add(question3);
                questions.Add(question4);
                questions.Add(question5);
                questions.Add(question6);
                questions.Add(question7);
                questions.Add(question8);
                questions.Add(question9);
                questions.Add(question10);
                questions.Add(question11);
                questions.Add(question12);
                questions.Add(question13);
                questions.Add(question14);
                questions.Add(question15);
                questions.Add(question16);
                questions.Add(question17);

            String answ1_1 = "deer";
            String answ1_2 = "pound";
            String answ1_3 = "14";
            String answ1_4 = "yens";
            String answ1_5 = "freedom of speech";
            String answ1_6 = "George Washington";
            String answ1_7 = "1914";
            String answ1_8 = "physician";
            String answ1_9 = "Albert Einstein";
            String answ1_10 = "1901";
            String answ1_11 = "Pacific";
            String answ1_12 = "All star";
            String answ1_13 = "Constantinopol";
            String answ1_14 = "buddhism";
            String answ1_15 = "Crypton";
            String answ1_16 = "actor";
            String answ1_17 = "Ares";

                Right.Add(answ1_1);
                Right.Add(answ1_2);
                Right.Add(answ1_3);
                Right.Add(answ1_4);
                Right.Add(answ1_5);
                Right.Add(answ1_6);
                Right.Add(answ1_7);
                Right.Add(answ1_8);
                Right.Add(answ1_9);
                Right.Add(answ1_10);
                Right.Add(answ1_11);
                Right.Add(answ1_12);
                Right.Add(answ1_13);
                Right.Add(answ1_14);
                Right.Add(answ1_15);
                Right.Add(answ1_16);
                Right.Add(answ1_17);

                String answ2_1 = "wolf";
            String answ2_2 = "euro";
            String answ2_3 = "15";
            String answ2_4 = "pounds";
            String answ2_5 = "right to self defence";
            String answ2_6 = "Abraham Lincoln";
            String answ2_7 = "1915";
            String answ2_8 = "musician";
            String answ2_9 = "Thomas Edison";
            String answ2_10 = "1873";
            String answ2_11 = "Atlantic";
            String answ2_12 = "Take on me";
            String answ2_13 = "Rome";
            String answ2_14 = "catholicism";
            String answ2_15 = "Mercury";
            String answ2_16 = "politician";
            String answ2_17 = "Mars";

                A1.Add(answ2_1);
                A1.Add(answ2_2);
                A1.Add(answ2_3);
                A1.Add(answ2_4);
                A1.Add(answ2_5);
                A1.Add(answ2_6);
                A1.Add(answ2_7);
                A1.Add(answ2_8);
                A1.Add(answ2_9);
                A1.Add(answ2_10);
                A1.Add(answ2_11);
                A1.Add(answ2_12);
                A1.Add(answ2_13);
                A1.Add(answ2_14);
                A1.Add(answ2_15);
                A1.Add(answ2_16);
                A1.Add(answ2_17);

                String answ3_1 = "dog";
            String answ3_2 = "dollars";
            String answ3_3 = "13";
            String answ3_4 = "euro";
            String answ3_5 = "right to vote";
            String answ3_6 = "Benjamin Franklin";
            String answ3_7 = "1816";
            String answ3_8 = "painter";
            String answ3_9 = "Issac Newton";
            String answ3_10 = "1940";
            String answ3_11 = "Indian";
            String answ3_12 = "I need a hero";
            String answ3_13 = "Lichteinstein";
            String answ3_14 = "islamism";
            String answ3_15 = "Alpha-Centauri";
            String answ3_16 = "musician";
            String answ3_17 = "Hades";

                A2.Add(answ3_1);
                A2.Add(answ3_2);
                A2.Add(answ3_3);
                A2.Add(answ3_4);
                A2.Add(answ3_5);
                A2.Add(answ3_6);
                A2.Add(answ3_7);
                A2.Add(answ3_8);
                A2.Add(answ3_9);
                A2.Add(answ3_10);
                A2.Add(answ3_11);
                A2.Add(answ3_12);
                A2.Add(answ3_13);
                A2.Add(answ3_14);
                A2.Add(answ3_15);
                A2.Add(answ3_16);
                A2.Add(answ3_17);

                String answ4_1 = "cat";
            String answ4_2 = "yens";
            String answ4_3 = "17";
            String answ4_4 = "dollars";
            String answ4_5 = "equality";
            String answ4_6 = "Thomas Jefferson";
            String answ4_7 = "1917";
            String answ4_8 = "politician";
            String answ4_9 = "Arthur Compton";
            String answ4_10 = "1735";
            String answ4_11 = "Arctic";
            String answ4_12 = "Never gonna give you up";
            String answ4_13 = "Paris";
            String answ4_14 = "othodoxim";
            String answ4_15 = "Earth";
            String answ4_16 = "mathematician";
            String answ4_17 = "Hermes";

                A3.Add(answ4_1);
                A3.Add(answ4_2);
                A3.Add(answ4_3);
                A3.Add(answ4_4);
                A3.Add(answ4_5);
                A3.Add(answ4_6);
                A3.Add(answ4_7);
                A3.Add(answ4_8);
                A3.Add(answ4_9);
                A3.Add(answ4_10);
                A3.Add(answ4_11);
                A3.Add(answ4_12);
                A3.Add(answ4_13);
                A3.Add(answ4_14);
                A3.Add(answ4_15);
                A3.Add(answ4_16);
                A3.Add(answ4_17);
            } // strings'

            { 
            //string connString = @"";
            //string query="SELECT * FROM Table";

            //SqlConnection conn = new SqlConnection(connString);
            //try
            //{
            //    conn.Open();
            //}catch(Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}

            //SqlCommand command = new SqlCommand(query, conn);
            //SqlDataReader myTable = command.ExecuteReader();

            //myTable.
            //int myRand = Rand(20);

            //Question.Text = myTable[myRand][1].toString();
        }

            myRand = Rand(17);
            Question.Text= questions[myRand];
            butt1.Content = Right[myRand];
            butt2.Content = A1[myRand];
            butt3.Content = A2[myRand];
            butt4.Content = A3[myRand];
        }

        private int Rand(int max)
        {
            Thread.Sleep(10);
            Random rand = new Random();
            return rand.Next(max);
        }

        public bool Valid()
        {
            return corect;
        }

        private void Click1(object sender, RoutedEventArgs e)
        {

            corect = true;
            MessageBox.Show("Correct! ");

            this.Close();
        }

        private void Click2(object sender, RoutedEventArgs e)
        {
            corect = false;
            MessageBox.Show("Wrong! The answer was " + Right[myRand]);

            this.Close();
        }

        private void Click3(object sender, RoutedEventArgs e)
        {
            corect = false;
            MessageBox.Show("Wrong! The answer was " + Right[myRand]);

            this.Close();

        }

        private void Click4(object sender, RoutedEventArgs e)
        {
            corect = false;
            MessageBox.Show("Wrong! The answer was " + Right[myRand]);
            this.Close();
        }
    }
}
