using System;
using System.Reflection;
using ChatUI;
using Gtk;

public partial class MainWindow : Gtk.Window
{
    static ClientBase client;
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
        client = new ClientBase(this);
		/*this.SetIconFromFile(string.Format("{0}{1}Images{2}Logo.png", 
   System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), 
   System.IO.Path.DirectorySeparatorChar, 
   System.IO.Path.DirectorySeparatorChar));*/
    }

    public void AddChatMessage(string message)
    {
        chatTextView.Buffer.Text += Environment.NewLine + message;
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        client.StopClient();
        Application.Quit();
        a.RetVal = true;
    }

    protected void buttonConnect_Click(object sender, EventArgs e)
    {
        client.ConnectToServer(entryIP.Text, entryUserName.Text, entryPassword.Text);
    }

    protected void buttonSend_Click(object sender, EventArgs e)
    {
        client.SendMessage(entryMessage.Text);
        entryMessage.Text = "";
    }


    protected void messageEnter(object sender, EventArgs e)
    {
        buttonSend_Click(sender, e);
    }
}
