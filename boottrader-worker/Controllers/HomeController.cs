using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace boottrader_worker.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConnectionFactory _rabbitConnectionFactory;
        private IModel channelForEventing;

        public HomeController(IConnectionFactory rabbitConnectionFactory)
        {
            _rabbitConnectionFactory = rabbitConnectionFactory;
            SslOption opt = (_rabbitConnectionFactory as ConnectionFactory).Ssl;
            if (opt != null && opt.Enabled)
            {
                opt.Version = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;

                // Only needed if want to disable certificate validations
                opt.AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateChainErrors |
                    SslPolicyErrors.RemoteCertificateNameMismatch | SslPolicyErrors.RemoteCertificateNotAvailable;
            }

            IConnection _rabbitConnection = _rabbitConnectionFactory.CreateConnection();
            channelForEventing = _rabbitConnection.CreateModel();
            channelForEventing.BasicQos(0, 1, false);
            EventingBasicConsumer consumer = new EventingBasicConsumer(channelForEventing);

            consumer.Received += EventingBasicConsumer_Received;
            channelForEventing.BasicConsume("boottrader-order", false, consumer);
        }
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        private static void EventingBasicConsumer_Received(object sender, BasicDeliverEventArgs e)
        {
            IBasicProperties basicProperties = e.BasicProperties;
            Console.WriteLine("Message received by the event based consumer. Check the debug window for details.");
            Debug.WriteLine(string.Concat("Message received from the exchange ", e.Exchange));
            Debug.WriteLine(string.Concat("Content type: ", basicProperties.ContentType));
            Debug.WriteLine(string.Concat("Consumer tag: ", e.ConsumerTag));
            Debug.WriteLine(string.Concat("Delivery tag: ", e.DeliveryTag));
            Debug.WriteLine(string.Concat("Message: ", Encoding.UTF8.GetString(e.Body)));

        }
    }
}
