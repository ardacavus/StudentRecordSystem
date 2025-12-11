using System;
using System.Windows.Forms;

namespace StudentSystem
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Başına System.Windows.Forms ekleyerek karışıklığı çözüyoruz
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            // Burası önemli: LoginForm'u başlatıyoruz
            System.Windows.Forms.Application.Run(new LoginForm());
        }
    }
}