using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnlineShopClient
{
    public partial class OnlineShopClientForm : Form
    {
        private readonly IOnlineShopData m_onlineShop;
        public static OnlineShopClientForm onlineShopClientForm;
        public OnlineShopClientForm(IOnlineShopData onlineShop)
        {
            InitializeComponent();
            m_onlineShop = onlineShop;
            onlineShopClientForm = this;
            btnPurchase.Enabled = false;
            cbxProject.Items.Add("Scones");
            cbxProject.Items.Add("Cake");
            cbxProject.Items.Add("Muffins");
            cbxProject.Items.Add("Cookies");
            cbxProject.Items.Add("Croissants");
        }
        private void OnlineShopClientForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_onlineShop.Disconnect();
        }
        private void OnlineShopClientForm_Load(object sender, EventArgs e)
        {
            m_onlineShop.Progress = new Progress<OnlineShop>(OnlineShopUpdate_ProgressReport);
            if (SetHost())
            {
                m_onlineShop.Connect(m_onlineShop.AccountNo);
                this.Text = $"ShopClient, User: {m_onlineShop.UserName}";
                m_onlineShop.GetProducts();
            }
            else
                Application.Exit();
        }

        private void OnlineShopUpdate_ProgressReport(OnlineShop onlineShopData)
        {
            if (null == onlineShopData)
            {
                MessageBox.Show("Server Unavailable", "Server Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            else
            {
                if (onlineShopData.AccountNo == 1 || onlineShopData.AccountNo == 2 || onlineShopData.AccountNo == 3)
                {
                    this.Text = $"ShopClient, User: {onlineShopData.UserName}";
                    m_onlineShop.UserName = onlineShopData.UserName;
                }
            }
        }

        private bool SetHost() => new LoginForm(m_onlineShop).ShowDialog(this) == DialogResult.OK;

        private void btnPurchase_Click(object sender, EventArgs e)
        {
            m_onlineShop.ProductName = cbxProject.Text;
            m_onlineShop.Purchase(m_onlineShop.UserName, m_onlineShop.ProductName);
            if (!string.IsNullOrEmpty(cbxProject.Text))
            {
                
                //lstProject.Items.Add($"{m_onlineShop.ProductName},1,{m_onlineShop.UserName}"); // 모든 창에 보여져야함.
                //
            }
            //m_onlineShop.GetOrders(m_onlineShop.UserName, m_onlineShop.ProductName);
        }

        private void cbxProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cbxProject.Text))
                btnPurchase.Enabled = true;
        }
    }
}
