using System;
//using ChatClient;
using Gtk;

namespace ChatUI
{
    class MainClass
    {
        //private ClientBase chatClient = new ClientBase();
        public static void Main(string[] args)
        {
            Application.Init();
            MainWindow win = new MainWindow();
            win.Show();
            Application.Run();
        }
    }
}
