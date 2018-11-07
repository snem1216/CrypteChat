using System;
using ChatUI;
using Gtk;

public partial class MainWindow : Gtk.Window
{
    static ClientBase client;
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
        client = new ClientBase(this);
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
