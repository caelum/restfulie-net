using System;
using System.IO;
using System.Net;
using System.Net.Browser;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using RestfulieClient.resources;

namespace RestfulieSilverlightClientTest
{
    public partial class MainPage : UserControl
    {
        private static bool _inProgress = false;

        public MainPage() {
            InitializeComponent();

            bool httpResult = WebRequest.RegisterPrefix("http://", WebRequestCreator.ClientHttp);
            bool httpsResult = WebRequest.RegisterPrefix("https://", WebRequestCreator.ClientHttp);
        }

        private void Write(string format, params object[] args) {
            Message.Text = String.Format(format, args);
        }

        private void RestfulieResponseHandler(IResource resource, object state) {
            _inProgress = false;

            dynamic dynamicResource = resource;

            Dispatcher.BeginInvoke(() => Write(
                "Response received, is empty = {0}, status = {1}, state = {2}, order id = {3}", 
                    resource.IsEmpty, resource.WebResponse.StatusCode, state,
                    resource.IsEmpty ? null : dynamicResource.id
                )
            );
        }

        private void GetResource(object state) {
            _inProgress = true;

            Thread.Sleep(2000); // simulate remote request

            Restfulie.At("http://localhost:62968/order.xml")
                .Asynch(RestfulieResponseHandler, "Test State")
                .Get();
        }

        private void DisplayProgress(object state) {
            if (!_inProgress)
                return;
            int dots = (int)state;
            Dispatcher.BeginInvoke(() => Write("Fetching".PadRight(8 + dots, '.')));
            Thread.Sleep(1000);
            if (dots == 3)
                dots = 0;
            DisplayProgress(dots + 1);
        }

        private void Go_Click(object sender, RoutedEventArgs e) {
            ThreadPool.QueueUserWorkItem(GetResource);
            ThreadPool.QueueUserWorkItem(DisplayProgress, 1);
        }
    }
}
