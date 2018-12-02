﻿using ETicketDesktopClient.ETicketServiceClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ETicketDesktopClient
{
    public partial class Orders : Form
    {
        Thread thread;

        public Orders()
        {
            InitializeComponent();
        }

        public List<Order> findOrders(string Username)
        {
            using (ETicketServiceClient.OrderServiceClient orderClient = new ETicketServiceClient.OrderServiceClient())
            {
                orderClient.ClientCredentials.UserName.UserName = "ETicket";
                orderClient.ClientCredentials.UserName.Password = "ETicketPass";
                List<Order> orders = new List<Order>(orderClient.GetCustomerOrdersByUsername(Username).Cast<Order>());
                return orders;
            }
        }

        private void searchBt_Click(object sender, EventArgs e)
        {
            String Username = usernameTb.Text;
            var orders = new BindingList<Order>(findOrders(Username));
            if (orders.Count > 0)
            {
                ordersGrid.DataSource = orders;
                ordersGrid.ReadOnly = true;
                ordersGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                ordersGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                ordersGrid.ClearSelection();
                ordersGrid.MultiSelect = false;
            }
            else
            {
                MessageBox.Show("No orders found!");
            }
        }

        private void ordersGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string orderId = ordersGrid.Rows[e.RowIndex].Cells[3].Value.ToString();
            if (string.IsNullOrEmpty(orderId))
            {
                int id = Convert.ToInt32(orderId);
                this.Close();
                thread = new Thread(OpenNewWindow);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
        }

        private void OpenNewWindow(object obj)
        {
            Application.Run(new CurrentOrder());
        }
    }
}
