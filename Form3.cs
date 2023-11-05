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
        public event Action<OrderDetails> OrderStatus;
        private ListView orderListView = new ListView();
        private Button retrieveOrderButton = new Button();
        private Button contactClientButton = new Button();
        private OrderDetails selectedOrder = null;
        
        public Form3(Form2 form2, Form1 form1)
        {
        this.form1 = form1; 
        this.form2 = form2;
        InitializeComponent();
        InitializeOrderListView();  
        }

        private void InitializeOrderListView()
        {
            orderListView.Visible = true;
            orderListView.Bounds = new Rectangle(10, 10, 600, 150);
            retrieveOrderButton.Location = new Point(10, orderListView.Bottom + 20);

            orderListView.Columns.Add("Customer", 150);
            orderListView.Columns.Add("Adress", 150);
            orderListView.Columns.Add("Phone Number", 100);
            orderListView.Columns.Add("Total", 70);
            orderListView.Columns.Add("Pizzas", 130);

            orderListView.View = View.Details;
            orderListView.FullRowSelect = true;
            this.Controls.Add(orderListView);

            retrieveOrderButton.Text = "Pick the order";
            retrieveOrderButton.Bounds = new Rectangle(10, orderListView.Bottom + 10, 270, 25);
            retrieveOrderButton.Click += RetrieveOrderButton_Click;
            
            this.Controls.Add(retrieveOrderButton);

            contactClientButton.Bounds = new Rectangle(10 + retrieveOrderButton.Right, orderListView.Bottom + 10, 270, 25);
            contactClientButton.Text = "Call the customer";
            contactClientButton.Click += ContactClientButton_Click;

            this.Controls.Add(contactClientButton);
        }


        public void ReceiveOrderFromForm2(OrderDetails order)
        {
            MessageBox.Show("New order !");
            ListViewItem item = new ListViewItem($"{order.FirstName} {order.LastName}");
            item.SubItems.Add(order.Address);
            item.SubItems.Add(order.PhoneNumber);
            item.SubItems.Add(order.TotalPrice.ToString("C"));
            item.SubItems.Add(string.Join(", ", order.Pizzas));

            orderListView.Items.Add(item);
            orderListView.Refresh();
        }

        public void RetrieveOrderButton_Click(object sender, EventArgs e)
        {
            if (orderListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Choose an order, please.");
                return;
         }
            selectedOrder = orderListView.SelectedItems[0].Tag as OrderDetails;
            OrderStatus?.Invoke(selectedOrder);
        }

   

    public void ContactClientButton_Click(object sender, EventArgs e)
    {
        MessageBox.Show("Call the client");
        NotifyContactClient?.Invoke();
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