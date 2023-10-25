using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;

namespace Torre_Di_Pizza
{
    public partial class Form3 : Form
    {
        public Form1 form1;
        public Form2 form2;
        public event Action OrderRetrieved;
        public event Action NotifyContactClient;
        private ListView orderListView = new ListView();
        private Button retrieveOrderButton = new Button();
        public Form3(Form2 form2, Form1 form1)
        {
        this.form1 = form1;  // Stockez une référence à Form1
        this.form2 = form2;
        InitializeComponent();
        InitializeOrderListView();

        if (form2 != null)
        {
            form2.OrderTimedOut += Form2_OrderTimedOut;
        }
        }

        private void InitializeOrderListView()
        {
            orderListView.Bounds = new Rectangle(10, 10, 600, 150);
            retrieveOrderButton.Location = new Point(10, orderListView.Bottom + 20);

            // Ajouter des colonnes pour les détails de la commande
            orderListView.Columns.Add("Client", 150);
            orderListView.Columns.Add("Adress", 150);
            orderListView.Columns.Add("Phone Number", 100);
            orderListView.Columns.Add("Total", 70);
            orderListView.Columns.Add("Pizzas", 130);

            // D'autres propriétés de la ListView
            orderListView.View = View.Details;
            orderListView.FullRowSelect = true;

            retrieveOrderButton.Text = "Récupérer la commande";
            retrieveOrderButton.Click += RetrieveOrderButton_Click;

            this.Controls.Add(orderListView);
            this.Controls.Add(retrieveOrderButton);
        }

        private void Form2_OrderTimedOut(OrderDetails order)
        {
            this.Invoke((MethodInvoker)delegate 
            {
                ListViewItem item = new ListViewItem($"{order.FirstName} {order.LastName}");
                item.SubItems.Add(order.Address);
                item.SubItems.Add(order.PhoneNumber);
                item.SubItems.Add(order.TotalPrice.ToString("C"));
                item.SubItems.Add(string.Join(", ", order.Pizzas));
                orderListView.Items.Add(item);
            });
        }

        public void RetrieveOrderButton_Click(object sender, EventArgs e)
        {
            if (retrieveOrderButton.Text == "Récupérer la commande")
            {
                if (orderListView.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Veuillez sélectionner une commande.");
                    return;
                }

                orderListView.SelectedItems[0].Remove();
                retrieveOrderButton.Text = "Contacter le client";
            }
            else if (retrieveOrderButton.Text == "Contacter le client")
            {
                // Ici, vous pouvez ajouter le code pour contacter le client si nécessaire.
                MessageBox.Show("Contactez le client.");
                retrieveOrderButton.Text = "Récupérer la commande";
                NotifyContactClient?.Invoke();
            }
        }

        public void RemoveOrder()
        {
            if (orderListView.SelectedItems.Count > 0)
            {
                orderListView.SelectedItems[0].Remove();
            }
        }
    }
}