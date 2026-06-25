using System;
using System.Windows.Forms;

namespace ProjetoFinal
{
    public partial class Formprofissionalprinc : Form
    {
        public Formprofissionalprinc()
        {
            InitializeComponent();
            this.Load += Formprofissionalprinc_Load;
        }

        private void Formprofissionalprinc_Load(object sender, EventArgs e)
        {
            DashboardProfissional dashboard = new DashboardProfissional();
            dashboard.Show();
            this.Hide();
        }
    }
}
