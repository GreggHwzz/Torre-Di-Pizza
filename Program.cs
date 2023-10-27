using System;
using System.Windows.Forms;

namespace Torre_Di_Pizza
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => 
            {
                Exception ex = (Exception)e.ExceptionObject;
                MessageBox.Show($"Unhandled exception: {ex.Message}");
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Créer les instances de vos forms
            Form1 form1 = new Form1();
            Form2 form2 = new Form2(); // Si Form2 dépend de Form1, vous pouvez passer Form1 comme argument
            Form3 form3 = new Form3(form2,form1); // De même, si Form3 dépend de Form2

            form1.Load += (sender, e) =>
            {
                form2.Show();
                form3.Show();
            };

            // Commencez à exécuter la boucle de message de l'application sur le thread principal
            // et faites de form1 la fenêtre principale (elle contrôle la durée de vie de l'application)
            Application.Run(form1); 
        }
    }
}