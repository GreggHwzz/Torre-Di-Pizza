using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;

namespace Torre_Di_Pizza
{
    public partial class Form2 : Form
    {
        public event Action<OrderDetails> OrderTimedOut;
        private ListView listViewOrders = new ListView();
        private Button sendButton = new Button();
        private OrderDetails selectedOrder = null;
        private System.Timers.Timer orderTimer;
        public Form3 form3;
        
        public Form2()
        {
        InitializeComponent();
        InitializeListView();
        InitializeSendButton();
        

        form3 = new Form3(this, null);

        form3.OrderRetrieved += Form3_OrderRetrieved;

        this.Load += Form2_Load;
        }
        
        private void Form2_Load(object sender, EventArgs e)
        {
            ListenForOrders();
        }
        private void InitializeListView()
        {
            listViewOrders.Width = 850;
            listViewOrders.Height = 300;
            listViewOrders.Location = new Point(10, 10);  

            // Ajoutez des colonnes pour les détails de la commande
            listViewOrders.Columns.Add("Client", 150);
            listViewOrders.Columns.Add("Adress", 250);
            listViewOrders.Columns.Add("Phone number", 100);
            listViewOrders.Columns.Add("Pizzas", 300);
            listViewOrders.Columns.Add("Price", 50);
            listViewOrders.Columns.Add("State", 100);

            // D'autres propriétés de la ListView
            listViewOrders.View = View.Details;
            listViewOrders.FullRowSelect = true;

            this.Controls.Add(listViewOrders);
        }

        private void InitializeSendButton()
        {
            sendButton.Text = "Start Timer for Selected Order";
            sendButton.Width = 200;
            sendButton.Height = 30;
            sendButton.Location = new Point(10, listViewOrders.Bottom + 10);
            sendButton.Click += SendButton_Click;
            this.Controls.Add(sendButton);
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (listViewOrders.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an order first.");
                return;
            }

            var selectedItem = listViewOrders.SelectedItems[0];
            selectedOrder = selectedItem.Tag as OrderDetails;  // I assume you stored OrderDetails in Tag.
            selectedItem.SubItems[5].Text = "En cuisine";  

            if(orderTimer != null)  // Si le timer existe déjà, arrêtez-le et désinscrivez-vous de l'événement
            {
                orderTimer.Stop();
                orderTimer.Elapsed -= Timer_Elapsed;
            }

            orderTimer = new System.Timers.Timer(5000);
            orderTimer.Elapsed += Timer_Elapsed;
            orderTimer.AutoReset = false;
            orderTimer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (selectedOrder != null)
            {
                this.Invoke((MethodInvoker)delegate 
                {
                    var items = listViewOrders.Items.OfType<ListViewItem>().Where(i => i.Tag == selectedOrder).ToList();
                    Console.WriteLine($"Number of matched items: {items.Count}");
                    if (items.Count > 0)
                    {
                        items[0].SubItems[5].Text = "Prêt";   // Set order state to "Prêt"
                    }
                });
                form3.ReceiveOrderFromForm2(selectedOrder);
                selectedOrder = null;  // Reset the selected order.
            }
        }

        

        private void Form3_OrderRetrieved()
        {
            if (selectedOrder != null)
            {
                if (orderTimer != null)
                {
                    orderTimer.Stop();
                    orderTimer.Elapsed -= Timer_Elapsed;
                }

                selectedOrder = null;
            }
        }
        private void ListenForOrders()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "orderQueue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var order = DeserializeOrder(message);

                this.Invoke((MethodInvoker)delegate {
                    ListViewItem item = new ListViewItem(order.LastName+" "+order.FirstName);
                    item.SubItems.Add(order.Address);
                    item.SubItems.Add(order.PhoneNumber);
                    item.SubItems.Add(string.Join(", ", order.Pizzas));
                    item.SubItems.Add(order.TotalPrice.ToString("F2"));
                    item.SubItems.Add("Payé");   
                    item.Tag = order;
                    listViewOrders.Items.Add(item);

                    listViewOrders.Refresh();
                });
            };

            channel.BasicConsume(queue: "orderQueue",
                                 autoAck: true,
                                 consumer: consumer);
        }

        

        private OrderDetails DeserializeOrder(string message)
        {
            return JsonConvert.DeserializeObject<OrderDetails>(message);
        }

        public void UpdateOrderStatus(string status)
        {
            if (selectedOrder != null)
            {
                var items = listViewOrders.Items.OfType<ListViewItem>().Where(i => i.Tag == selectedOrder).ToList();
                if (items.Count > 0)
                {
                    items[0].SubItems[5].Text = status;   // Mettez à jour le statut
                }
            }
        }

        public void SetOrderToContactClient()
        {
            if (selectedOrder != null)
            {
                var items = listViewOrders.Items.OfType<ListViewItem>().Where(i => i.Tag == selectedOrder).ToList();
                if (items.Count > 0)
                    {
                        items[0].SubItems[5].Text = "Livré";
                    }
            }
        }

        
        private void NotifyForm3(OrderDetails order)
        {
            OrderTimedOut?.Invoke(order);
        }
    }
}