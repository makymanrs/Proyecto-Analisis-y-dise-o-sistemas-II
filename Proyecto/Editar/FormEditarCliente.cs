using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto.Editar
{
    public partial class FormEditarCliente : Form
    {
        public FormEditarCliente()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int searchValue))
            {
                Mysql.CCliente objetoCliente = new Mysql.CCliente();
                DataRow cliente = objetoCliente.buscarCliente(searchValue);

                if (cliente != null)
                {
                    // Asignar valores a los controles
                    textBox2.Text = cliente["cli_iden"].ToString();
                    textBox3.Text = cliente["cli_pnom"].ToString();   
                    textBox4.Text = cliente["cli_snom"].ToString();
                    textBox5.Text = cliente["cli_pape"].ToString();
                    textBox6.Text = cliente["cli_sape"].ToString();
                    textBox7.Text = cliente["cli_edad"].ToString();
                    richTextBox1.Text = cliente["cli_dir"].ToString();
                    textBox8.Text = cliente["cli_tel"].ToString();
                }
                else
                {
                    MessageBox.Show("Cliente no encontrado.");
                }
            }
            else
            {
                MessageBox.Show("Ingrese un valor numérico válido para buscar.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Mysql.CCliente objetoCliente = new Mysql.CCliente();
            objetoCliente.modificarCliente(textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, richTextBox1, textBox8);
        }
    }
}
