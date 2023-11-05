using System;
using System.Windows.Forms;

namespace Torre_Di_Pizza
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
       
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form1 form1 = new Form1();
            Form2 form2 = new Form2();
          

            form1.Load += (sender, e) =>
            {
                form2.Show();
            };

            Application.Run(form1); 
        }
    }
}