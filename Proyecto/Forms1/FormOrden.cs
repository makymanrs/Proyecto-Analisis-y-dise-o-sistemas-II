using MySql.Data.MySqlClient;
using Proyecto.Mysql;
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
    public partial class Orden : Form
    {
        public Orden()
        {
            InitializeComponent();
            Mysql.Corden objetoOrden = new Mysql.Corden();
            objetoOrden.mostrarOrden(dataGridOrden);
            ActualizarConteoRegistros(); // Actualizar el conteo de registros después de mostrar los datos
            dataGridOrden.ReadOnly = true;

            dataGridOrden.RowTemplate.Height = 40;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

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

                // Crear y mostrar el formulario para ingresar orden
                using (Ingresar.FormIngresarOrden formIngresarOrden = new Ingresar.FormIngresarOrden())
                {
                    formIngresarOrden.StartPosition = FormStartPosition.CenterScreen;
                    formIngresarOrden.ShowDialog(FormBG);
                }

                // Cerrar el formulario oscuro cuando se cierre el formulario de ingreso de proveedor
                FormBG.Dispose();
            }
            // Actualizar el DataGridView y el conteo de registros
            Mysql.Corden objetoOrden = new Mysql.Corden();
            objetoOrden.mostrarOrden(dataGridOrden);
            ActualizarConteoRegistros();
        }

        // Conteo de registros
        private void ActualizarConteoRegistros()
        {
            int totalRegistros = dataGridOrden.RowCount;
            if (dataGridOrden.AllowUserToAddRows)
            {
                totalRegistros--; // Restar 1 si AllowUserToAddRows está activado
            }
            label2.Text = "Total de registros: " + totalRegistros;
        }

        private void FormOrden_Load(object sender, EventArgs e)
        {
            Mysql.Corden objetoOrden = new Mysql.Corden();
            objetoOrden.mostrarOrden(dataGridOrden);
            ActualizarConteoRegistros();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Crear un nuevo formulario oscuro como fondo
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

                // Crear y mostrar el formulario para editar Orden de Proveedor
                using (Editar.FormEditarOrden formEditarOrden = new Editar.FormEditarOrden())
                {
                    formEditarOrden.StartPosition = FormStartPosition.CenterScreen;
                    formEditarOrden.ShowDialog(FormBG);
                }

                // Cerrar el formulario oscuro cuando se cierre el formulario de ingreso de proveedor
                FormBG.Dispose();
            }
            // Actualizar el DataGridView y el conteo de registros
            Mysql.Corden objetoOrden = new Mysql.Corden();
            objetoOrden.mostrarOrden(dataGridOrden);
            ActualizarConteoRegistros();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Mysql.Corden objetoOrden = new Mysql.Corden();
            objetoOrden.eliminarOrden(textBox1, dataGridOrden);
            objetoOrden.mostrarOrden(dataGridOrden);
            ActualizarConteoRegistros(); // Actualizar el conteo de registros después de eliminar una orden
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Mysql.Corden objetoOrden = new Mysql.Corden();
            objetoOrden.buscarOrdenPorId(dataGridOrden, textBox1);
            ActualizarConteoRegistros(); // Actualizar el conteo de registros después de buscar una orden
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Mysql.Corden objetoOrden = new Mysql.Corden();
            objetoOrden.mostrarOrden(dataGridOrden);
            ActualizarConteoRegistros(); // Actualizar el conteo de registros después de mostrar órdenes
        }

    }
}
