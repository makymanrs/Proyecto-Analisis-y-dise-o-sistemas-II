using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto.Forms1
{
    public partial class FormHistorial : Form
    {
        public FormHistorial()
        {
            InitializeComponent();
            Mysql.Cdetalle objetodetFactura = new Mysql.Cdetalle();
            objetodetFactura.MostrarFactura(dataGridViewFactura);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Mysql.Cdetalle objetoFactura = new Mysql.Cdetalle();
            objetoFactura.MostrarDetalleFactura(dataGridDetalle, textBox1);
        }
    }
}
