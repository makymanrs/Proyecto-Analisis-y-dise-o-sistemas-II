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
    public partial class FormCredito : Form
    {
        public FormCredito()
        {
            InitializeComponent();
            Mysql.Ccredito objetoCredito = new Mysql.Ccredito();
            objetoCredito.mostrarCredito(dataGridViewCredito);
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
                using (HistorialCredito.FormActualizarCredito formActualizarCredito = new HistorialCredito.FormActualizarCredito())
                {
                    formActualizarCredito.StartPosition = FormStartPosition.CenterScreen;
                    formActualizarCredito.ShowDialog(FormBG);
                }

                // Cerrar el formulario oscuro cuando se cierre el formulario de ingreso de Cliente
                FormBG.Dispose();
            }
            //Actualizar el DataGridView y el conteo de registros
            Mysql.CCliente objetoCliente = new Mysql.CCliente();
            //objetoCliente.mostrarCliente(dataGridCliente);
            //ActualizarConteoRegistros();
        }
    }
}
