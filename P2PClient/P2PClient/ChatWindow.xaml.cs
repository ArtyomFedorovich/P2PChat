// Written by Benjamin Watkins 2015
// watkins.ben@gmail.com

using System.Windows;
using System.Windows.Input;
using System.Net;
using Shared;
using System.IO;
using Microsoft.Win32;

namespace P2PChat
{
    /// <summary>
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {   
        public Client client;
        public new string Name;
        public IPEndPoint RemoteEP;
        public long ID;

        public ChatWindow(Client _client, string _Name, IPEndPoint _RemoteEP, long _ID)
        {
            InitializeComponent();

            client = _client;
            Name = _Name;
            RemoteEP = _RemoteEP;
            ID = _ID;

            Title = Name + " via " + RemoteEP;
        }
        
    public void ReceiveFileMessage(FileMessage F)
    {
      File.WriteAllBytes(F.FileName, F.Content);
      txtConversation.Text += F.From + ": " + F.FileName + '\n';
      txtConversation.CaretIndex = txtConversation.Text.Length;
      txtConversation.ScrollToEnd();
      txtMessage.Focus();
    }
        public void ReceiveMessage(Message M)
        {
            txtConversation.Text += M.From + ": " + M.Content + '\n';
            txtConversation.CaretIndex = txtConversation.Text.Length;
            txtConversation.ScrollToEnd();
            txtMessage.Focus();            
        }

        private void SendMessage(bool isCallNotify = false)
        {
            Message M;
            if (!isCallNotify)
            {
              M = new Message(client.LocalClientInfo.Name, Name, txtMessage.Text);
              client.SendMessageTCP(M);
              txtConversation.Text += client.LocalClientInfo.Name + ": " + txtMessage.Text + '\n';
              txtMessage.Text = string.Empty;
            }
            else
            {
        string message = ": Call request!" + '\n';
              M = new Message(client.LocalClientInfo.Name, Name, message);
              client.SendMessageUDP(M, RemoteEP);
              txtConversation.Text += client.LocalClientInfo.Name + ": " + "Call Request sended!" + '\n';
            }
            txtConversation.CaretIndex = txtConversation.Text.Length;
            txtConversation.ScrollToEnd();
            txtMessage.Focus();
        }
    private void SendFile()
    {
      string path = OpenFileMessageDialog();
      string fileName = Path.GetFileName(path);
      if (string.IsNullOrEmpty(fileName))
        return;
      FileMessage M;
      
        M = new FileMessage(client.LocalClientInfo.Name, Name, fileName, File.ReadAllBytes(path));
      M.RecipientID = ID;
      M.ID = client.LocalClientInfo.ID;
        client.SendMessageTCP(M);
        txtConversation.Text += client.LocalClientInfo.Name + ": file \'" + fileName + "\'" + "sended!\n";
        txtMessage.Text = string.Empty;
      
      txtConversation.CaretIndex = txtConversation.Text.Length;
      txtConversation.ScrollToEnd();
      txtMessage.Focus();
    }

    private string OpenFileMessageDialog()
    {
      OpenFileDialog dlg = new OpenFileDialog();
      dlg.DefaultExt = ".txt";
      string fileName = string.Empty;
      var result = dlg.ShowDialog();
      if (result == true)
      {
        fileName = dlg.FileName;
      }
      return fileName;
    }
    private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SendMessage();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void btnSendCallNotify_Click(object sender, RoutedEventArgs e)
        {
            SendMessage(true);
        }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      SendFile();
    }
  }
}

    
