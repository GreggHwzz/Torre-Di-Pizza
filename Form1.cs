using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using RabbitMQ.Client;
using Newtonsoft.Json;

namespace Torre_Di_Pizza
{
    public class Pizza
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public override string ToString()
        {
            return $"{Name} - {Price:C}";
        }
    }
    public class OrderDetails
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public List<PizzaOrderEntry> Pizzas { get; set; } = new List<PizzaOrderEntry>();
        public decimal TotalPrice { get; set; }
    }

    public class PizzaOrderEntry
        {
            public Pizza Pizza { get; set; }
            public int Count { get; set; } = 1;

            public override string ToString()
            {
                return $"{Pizza.Name}   x{Count}   {Pizza.Price * Count:C}";
            }
        }
    public partial class Form1 : Form
    {
        public event Action OrderRetrieved;
        private Button btnRemoveItem = new Button();
        private Button btnResetListBox = new Button();
        private Button btnResetAll = new Button();
        private Button retrievePizzaButton = new Button();
        private Button sendOrderButton = new Button();
        private TextBox textBoxFirstName = new TextBox();
        private TextBox textBoxLastName = new TextBox();
        private TextBox textBoxAddress = new TextBox();
        private TextBox textBoxPhone = new TextBox();

        private Label labelFirstName = new Label();
        private Label labelLastName = new Label();
        private Label labelAddress = new Label();
        private Label labelPhone = new Label();

        private Form2 form2Instance;  
        private Form3 form3Instance; 
        private List<Pizza> pizzas = new List<Pizza>
        {
            new Pizza { Name = "Margherita", Price = 8.5m },
            new Pizza { Name = "Pepperoni", Price = 9.5m },
            new Pizza { Name = "Vegetariana", Price = 8.0m },
            new Pizza { Name = "Romana", Price = 9.0m },
            new Pizza { Name = "Sixxo Formaggi", Price = 10.0m },
            new Pizza { Name = "Hawaiian", Price = 9.5m },
            new Pizza { Name = "Mexicana", Price = 10.0m },
            new Pizza { Name = "Regina", Price = 9.0m },
            new Pizza { Name = "Frutti di Mare", Price = 12.0m },
            new Pizza { Name = "Chèvre Miel", Price = 11.0m },
            new Pizza { Name = "Royale Pouleto", Price = 10.5m },
            new Pizza { Name = "Cannibalo", Price = 10.5m },
            
        };

        private ListBox orderListBox = new ListBox();
        private Label totalPriceLabel = new Label();
        private decimal totalPrice = 0.0m;

        
        public Form1()
        {
            InitializeComponent();

            int spacing = 40;  

            labelFirstName.Text = "First Name:";
            labelFirstName.Location = new Point(10, 10);
            this.Controls.Add(labelFirstName);

            textBoxFirstName.Location = new Point(150, 10);
            textBoxFirstName.Width = 200;
            this.Controls.Add(textBoxFirstName);

            labelLastName.Text = "Last name:";
            labelLastName.Location = new Point(10, 10 + spacing);
            this.Controls.Add(labelLastName);

            textBoxLastName.Location = new Point(150, 10 + spacing);
            textBoxLastName.Width = 200;
            this.Controls.Add(textBoxLastName);

            labelAddress.Text = "Adress:";
            labelAddress.Location = new Point(10, 10 + spacing * 2);
            this.Controls.Add(labelAddress);

            textBoxAddress.Location = new Point(150, 10 + spacing * 2);
            textBoxAddress.Width = 200;
            this.Controls.Add(textBoxAddress);
            
            labelPhone.Text = "Phone number:";
            labelPhone.Location = new Point(10, 10 + spacing * 3);
            this.Controls.Add(labelPhone);

            textBoxPhone.Location = new Point(150, 10 + spacing * 3);
            textBoxPhone.Width = 200;
            this.Controls.Add(textBoxPhone);

            
            int rows = 3;
            int cols = 4;
            int btnWidth = 100;
            int btnHeight = 30;
            int btnHorizontalSpacing = 15;
            int btnVerticalSpacing = 10;
            int initialX = 10;
            int initialY = textBoxPhone.Bottom + 20;

            for (int i = 0; i < Math.Min(pizzas.Count, rows * cols); i++)
            {
                int row = i / cols;
                int col = i % cols;

                Button pizzaButton = new Button
                {
                    Text = pizzas[i].Name,
                    Width = btnWidth,
                    Height = btnHeight,
                    Left = initialX + col * (btnWidth + btnHorizontalSpacing),
                    Top = initialY + row * (btnHeight + btnVerticalSpacing),
                    Tag = pizzas[i]
                };

                pizzaButton.Click += PizzaButton_Click;
                this.Controls.Add(pizzaButton);
            }

            btnRemoveItem.Text = "Remove";
            btnRemoveItem.Location = new Point(320, 290);
            btnRemoveItem.Click += BtnRemoveItem_Click;
            this.Controls.Add(btnRemoveItem);

            btnResetListBox.Text = "Clear";
            btnResetListBox.Location = new Point(320, 330); 
            btnResetListBox.Click += BtnResetListBox_Click;
            this.Controls.Add(btnResetListBox);

            btnResetAll.Text = "Reset";
            btnResetAll.Location = new Point(320, 370);
            btnResetAll.Click += BtnResetAll_Click;
            this.Controls.Add(btnResetAll);

            orderListBox.Bounds = new Rectangle(10, initialY + (btnHeight + btnVerticalSpacing) * rows, 300, 150);
            this.Controls.Add(orderListBox);

            totalPriceLabel.Text = "Total: 0.00€";
            totalPriceLabel.Bounds = new Rectangle(10, orderListBox.Bottom + 10, 300, 25);
            this.Controls.Add(totalPriceLabel);


            sendOrderButton.Text = "Send Order";
            sendOrderButton.Bounds = new Rectangle(10, totalPriceLabel.Bottom + 10, 300, 25);
            sendOrderButton.Click += SendOrderButton_Click;
            this.Controls.Add(sendOrderButton);

            retrievePizzaButton.Text = "Pick the order";
            retrievePizzaButton.Bounds = new Rectangle(10, sendOrderButton.Bottom + 10, 300, 25);
            retrievePizzaButton.Click += RetrievePizzaButton_Click;
            retrievePizzaButton.Enabled = false; 
            this.Controls.Add(retrievePizzaButton);

            form2Instance = new Form2();
            form3Instance = new Form3(form2Instance, this);

            form3Instance.OrderRetrieved += () =>
            {
                retrievePizzaButton.Enabled = true;
            };
        }
        
        private void RetrievePizzaButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Your order is ready. Thanks you for your purchase !");
            retrievePizzaButton.Enabled = false; 
            form2Instance.UpdateOrderStatus("Delivered");
            form3Instance.RemoveOrder();
        }
        
        private void PizzaButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            Pizza selectedPizza = clickedButton?.Tag as Pizza;

            if (selectedPizza != null)
            {
                bool found = false;

                for (int i = 0; i < orderListBox.Items.Count; i++)
                {
                    if (orderListBox.Items[i] is PizzaOrderEntry entry && entry.Pizza.Name == selectedPizza.Name)
                    {
                        entry.Count++;
                        orderListBox.Items[i] = entry;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    orderListBox.Items.Add(new PizzaOrderEntry { Pizza = selectedPizza });
                }

                totalPrice += selectedPizza.Price;
                UpdateTotalPrice();
            }
        }

        private void BtnRemoveItem_Click(object sender, EventArgs e)
        {
            if (orderListBox.SelectedItem != null)
            {
                orderListBox.Items.Remove(orderListBox.SelectedItem);
                UpdateTotalPrice();
            }
        }

        private void BtnResetListBox_Click(object sender, EventArgs e)
        {
            orderListBox.Items.Clear();
            UpdateTotalPrice();
        }

        
        private void BtnResetAll_Click(object sender, EventArgs e)
        {
            textBoxFirstName.Text = "";
            textBoxLastName.Text = "";
            textBoxAddress.Text = "";
            textBoxPhone.Text = "";
            orderListBox.Items.Clear();
            UpdateTotalPrice();
        }
        private void UpdateTotalPrice()
        {
            decimal total = 0.0m;

            foreach (var item in orderListBox.Items)
            {
                if (item is PizzaOrderEntry entry)
                {
                    total += entry.Pizza.Price * entry.Count;
                }
            }

            totalPriceLabel.Text = $"Total: {total:C}";  
            totalPrice = total; 
        }

        private void SendOrderToForm2(OrderDetails order)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "orderQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = SerializeOrder(order);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "orderQueue",
                                     basicProperties: null,
                                     body: body);

            }
        }

        private void SendOrderButton_Click(object sender, EventArgs e)
        {
            List<PizzaOrderEntry> pizzaOrders = new List<PizzaOrderEntry>();

            foreach (var item in orderListBox.Items)
            {
                if (item is PizzaOrderEntry entry)
                {
                    pizzaOrders.Add(entry);
                }
            }

            OrderDetails order = new OrderDetails
            {
                FirstName = textBoxFirstName.Text,
                LastName = textBoxLastName.Text,
                Address = textBoxAddress.Text,
                PhoneNumber = textBoxPhone.Text,
                Pizzas = pizzaOrders,
                TotalPrice = totalPrice
            };

            SendOrderToForm2(order);
            
        }

        private string SerializeOrder(OrderDetails order)
        {
            return JsonConvert.SerializeObject(order);
        }
    }
}
