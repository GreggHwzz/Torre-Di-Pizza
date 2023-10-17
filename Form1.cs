using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

namespace Torre_Di_Pizza
{
    public partial class Form1 : Form
    {
        private class Pizza
        {
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }

            public override string ToString()
            {
                return $"{Name} - {Price:C}";
            }
        }

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

            int rows = 3;
            int cols = 4;
            int totalButtons = rows * cols;
            int btnWidth = 100;
            int btnHeight = 30;
            int btnHorizontalSpacing = 15;
            int btnVerticalSpacing = 10;
            int initialX = 10;
            int initialY = 10;

            totalButtons = Math.Min(totalButtons, pizzas.Count);

            for (int i = 0; i < totalButtons; i++)
            {
                int row = i / cols; // la rangée actuelle
                int col = i % cols; // la colonne actuelle

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

            orderListBox.Bounds = new Rectangle(10, initialY + (btnHeight + btnVerticalSpacing) * rows, 300, 150);
            this.Controls.Add(orderListBox);

            totalPriceLabel.Text = "Total: 0.00€";
            totalPriceLabel.Bounds = new Rectangle(10, initialY + (btnHeight + btnVerticalSpacing) * rows + 160, 300, 25);
            this.Controls.Add(totalPriceLabel);
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
                UpdateTotalPriceLabel();
            }
        }

        private class PizzaOrderEntry
        {
            public Pizza Pizza { get; set; }
            public int Count { get; set; } = 1;

            public override string ToString()
            {
                return $"{Pizza.Name}   x{Count}   {Pizza.Price * Count:C}";
            }
        }

        private void UpdateTotalPriceLabel()
        {
            totalPriceLabel.Text = $"Total: {totalPrice:C}";
        }
    }
}
