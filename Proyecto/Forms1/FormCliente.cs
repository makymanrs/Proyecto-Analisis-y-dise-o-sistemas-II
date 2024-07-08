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
    public partial class FormCliente : Form
    {
        public FormCliente()
        {
            InitializeComponent();
            Mysql.CCliente objetoCliente = new Mysql.CCliente();
            objetoCliente.mostrarCliente(dataGridCliente);
            dataGridCliente.Margin = new Padding(10); // Ajusta según lo necesario
            ActualizarConteoRegistros();
            // Ajustar el padding de las celdas
            foreach (DataGridViewColumn column in dataGridCliente.Columns)
            {
                column.DefaultCellStyle.Padding = new Padding(0); // Ajusta según lo necesario
            }
            dataGridCliente.RowTemplate.Height = 40;
            dataGridCliente.ReadOnly = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (Form FormBG = new Form())
            {
                FormBG.StartPosition = FormStartPosition.Manual;
                FormBG.FormBorderStyle = FormBorderStyle.None;
                FormBG.Opacity = 0.70d;
                FormBG.BackColor = Color.Black;
                FormBG.WindowState = FormWindowState.Maximized;
                FormBG.TopMost = true;
                FormBG.ShowInTaskbar = false;
                FormBG.Show();

                // Crear y mostrar el formulario para ingresar Cliente
                using (Ingresar.FormIngresarCliente formIngresarCliente = new Ingresar.FormIngresarCliente())
                {
                    formIngresarCliente.StartPosition = FormStartPosition.CenterScreen;
                    formIngresarCliente.ShowDialog(FormBG);
                }

                // Cerrar el formulario oscuro cuando se cierre el formulario de ingreso de Cliente
                FormBG.Dispose();
            }
            //Actualizar el DataGridView y el conteo de registros
            Mysql.CCliente objetoCliente = new Mysql.CCliente();
            objetoCliente.mostrarCliente(dataGridCliente);
            ActualizarConteoRegistros();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Mysql.CCliente objetoCliente = new Mysql.CCliente();
            objetoCliente.mostrarCliente(dataGridCliente);
            ActualizarConteoRegistros();
        }

        private void ActualizarConteoRegistros()
        {
            int totalRegistros = dataGridCliente.RowCount;
            if (dataGridCliente.AllowUserToAddRows)
            {
                totalRegistros--; // Restar 1 si AllowUserToAddRows está activado
            }

            label2.Text = "Total de registros: " + totalRegistros;
        }

        private void FormCliente_Load(object sender, EventArgs e)
        {
            Mysql.CCliente objetoCliente = new Mysql.CCliente();
            objetoCliente.mostrarCliente(dataGridCliente);
            ActualizarConteoRegistros();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Mysql.CCliente objetoCliente = new Mysql.CCliente();
            objetoCliente.buscarclienteporcodigo(dataGridCliente, textBox1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Mysql.CCliente objetoCliente = new Mysql.CCliente();
            objetoCliente.eliminarCliente(textBox1, dataGridCliente);
            objetoCliente.mostrarCliente(dataGridCliente);
            ActualizarConteoRegistros();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (Form FormBG = new Form())
            {
                FormBG.StartPosition = FormStartPosition.Manual;
                FormBG.FormBorderStyle = FormBorderStyle.None;
                FormBG.Opacity = 0.70d;
                FormBG.BackColor = Color.Black;
                FormBG.WindowState = FormWindowState.Maximized;
                FormBG.TopMost = true;
                FormBG.ShowInTaskbar = false;
                FormBG.Show();

                // Crear y mostrar el formulario para ingresar proveedor
                using (Editar.FormEditarCliente formEditarCliente = new Editar.FormEditarCliente())
                {
                    formEditarCliente.StartPosition = FormStartPosition.CenterScreen;
                    formEditarCliente.ShowDialog(FormBG);
                }

                // Cerrar el formulario oscuro cuando se cierre el formulario de ingreso de proveedor
                FormBG.Dispose();
            }
            Mysql.CCliente objetoCliente = new Mysql.CCliente();
            objetoCliente.mostrarCliente(dataGridCliente);
            ActualizarConteoRegistros();
        }

        private void dataGridCliente_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Mysql.CCliente objetoCliente = new Mysql.CCliente();
            objetoCliente.seleccionarCliente(dataGridCliente, textBox1);
        }
    }
}
