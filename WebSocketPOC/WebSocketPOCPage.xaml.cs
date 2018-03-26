using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Websockets;
using Xamarin.Forms;

namespace WebSocketPOC
{
    public partial class WebSocketPOCPage : ContentPage
    {
        private bool _echo, _failed;
        private readonly IWebSocketConnection _connection;

        public WebSocketPOCPage()
        {
            InitializeComponent();

            _connection = WebSocketFactory.Create();
            _connection.OnOpened += Connection_OnOpened;
            _connection.OnMessage += Connection_OnMessage;
            _connection.OnClosed += Connection_OnClosed;
            _connection.OnError += Connection_OnError;
            Debug.WriteLine("In constructor");
        }

        protected override async void OnAppearing()
        {
            Debug.WriteLine("Currently we are here");
            base.OnAppearing();
            _echo = _failed = false;
            _connection.Open("ws://192.168.98.231:8080/marco/websocket");

            while (!_connection.IsOpen && !_failed)
            {
                Debug.WriteLine("Connection is opened");
                await Task.Delay(10);
            }

            Message.Focus();
        }

        private void Connection_OnError(string obj)
        {
            _failed = true;
            Debug.WriteLine("ERROR " + obj);
        }

        private void Connection_OnClosed()
        {
            Debug.WriteLine("Closed !");
        }

        private void Connection_OnMessage(string obj)
        {
            _echo = true;
            Device.BeginInvokeOnMainThread(() =>
            {
                ReceivedData.Children.Add(new Label { Text = obj });
            });
        }

        private void Connection_OnOpened()
        {
            Debug.WriteLine("Opened !");
        }

        private async void BtnSend_OnClicked(object sender, EventArgs e)
        {
            Debug.WriteLine($"Have clicked to send message of: {Message.Text}");
            _echo = false;
            _connection.Send(Message.Text);
            Message.Text = "";

            while (!_echo && !_failed)
            {
                await Task.Delay(10);
            }

            Message.Focus();
        }
    }
}
