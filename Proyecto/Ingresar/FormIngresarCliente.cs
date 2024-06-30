using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto.Ingresar
{
    public partial class FormIngresarCliente : Form
    {
        public FormIngresarCliente()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Mysql.CCliente objetoCliente = new Mysql.CCliente();
            objetoCliente.guardarcliente(textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, richTextBox1, textBox8);
        }
    }
}
