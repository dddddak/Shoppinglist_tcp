using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace OnlineShopClient
{
    public partial class LoginForm : Form
    {
        private readonly IOnlineShopData m_session;
        public LoginForm(IOnlineShopData session)
        {
            InitializeComponent();
            m_session = session;
            btnConnect.Enabled = false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (Int32.Parse(tbxAccount.Text) == 1)
                m_session.UserName = "Kim";
            else if (Int32.Parse(tbxAccount.Text) == 2)
                m_session.UserName = "Lee";
            else if (Int32.Parse(tbxAccount.Text) == 3)
                m_session.UserName = "Park";
            m_session.AccountNo = Int32.Parse(tbxAccount.Text);
            
        }
        private void tbxAccount_TextChanged(object sender, EventArgs e) => btnConnect.Enabled = !string.IsNullOrEmpty(tbxAccount.Text);
    }
}
