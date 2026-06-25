using System.Drawing;
using System.Windows.Forms;

namespace ProjetoFinal
{
    public partial class ReciboFaturaForm
    {
        private void InitializeComponent()
        {
            this.Text = "Fatura";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;
            this.Size = new Size(520, 720);
            this.Padding = new Padding(15);
        }
    }
}
