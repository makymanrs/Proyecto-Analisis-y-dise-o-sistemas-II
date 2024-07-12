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
    public partial class FormBodega : Form
    {
        public FormBodega()
        {
            InitializeComponent();
            Mysql.Cbodega objetobodega = new Mysql.Cbodega();
            objetobodega.mostrarBodegas(dataGridBodega);
            dataGridBodega.Margin = new Padding(10); // Ajusta según lo necesario

            // Ajustar el padding de las celdas
            foreach (DataGridViewColumn column in dataGridBodega.Columns)
            {
                column.DefaultCellStyle.Padding = new Padding(0); // Ajusta según lo necesario
            }
            dataGridBodega.RowTemplate.Height = 50;
            ActualizarConteoRegistros();
            dataGridBodega.ReadOnly = true;
        }

        private void ActualizarConteoRegistros()
        {
            int totalRegistros = dataGridBodega.RowCount;
            if (dataGridBodega.AllowUserToAddRows)
            {
                totalRegistros--; // Restar 1 si AllowUserToAddRows está activado
            }

            label2.Text = "Total de registros: " + totalRegistros;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Mysql.Cbodega objetoBodega = new Mysql.Cbodega();
            objetoBodega.buscarBodegaPorId(dataGridBodega, textBox1);
        }
        private void button2_Click(object sender, EventArgs e)
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

                // Crear y mostrar el formulario para ingresar proveedor
                using (Ingresar.FormIngresarBodega formIngresarBodega = new Ingresar.FormIngresarBodega())
                {
                    formIngresarBodega.StartPosition = FormStartPosition.CenterScreen;
                    formIngresarBodega.ShowDialog(FormBG);
                }

                // Cerrar el formulario oscuro cuando se cierre el formulario de ingreso de proveedor
                FormBG.Dispose();
            }
            Mysql.Cbodega objetoBodega = new Mysql.Cbodega();
            objetoBodega.mostrarBodegas(dataGridBodega);
            ActualizarConteoRegistros();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Mysql.Cbodega objetoBodega = new Mysql.Cbodega();
            objetoBodega.mostrarBodegas(dataGridBodega);
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

                // Crear y mostrar el formulario para ingresar proveedor
                using (Editar.FormEditarBodega formEditarBodega = new Editar.FormEditarBodega())
                {
                    formEditarBodega.StartPosition = FormStartPosition.CenterScreen;
                    formEditarBodega.ShowDialog(FormBG);
                }

                // Cerrar el formulario oscuro cuando se cierre el formulario de ingreso de proveedor
                FormBG.Dispose();
            }
            Mysql.Cbodega objetoBodega = new Mysql.Cbodega();
            objetoBodega.mostrarBodegas(dataGridBodega);
            ActualizarConteoRegistros();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Mysql.Cbodega objetoBodega = new Mysql.Cbodega();
            objetoBodega.eliminarBodega(textBox1, dataGridBodega);
            objetoBodega.mostrarBodegas(dataGridBodega);
            ActualizarConteoRegistros();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                // Crear una instancia de la clase Cbodega
                Cbodega bodegaManager = new Cbodega();

                // Llamar al método exportarExcel con el DataGridView que contiene los datos
                bodegaManager.exportarExcel(dataGridBodega);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar a Excel: " + ex.Message);
            }
        }

        private void dataGridBodega_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Mysql.Cbodega objetoBodega = new Mysql.Cbodega();
            objetoBodega.seleccionarBodega(dataGridBodega, textBox1);
        }

        private void dataGridBodega_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView dataGridView = sender as DataGridView;
            if (dataGridView != null)
            {
                DataGridViewRow row = dataGridView.Rows[e.RowIndex];
                if (row.Selected)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        cell.Style.Font = new Font(dataGridView.Font.FontFamily, 10, FontStyle.Bold); // Cambia el tamaño de letra a 12 y lo pone en negrita
                        cell.Style.ForeColor = Color.Black;
                    }
                }
                else
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        cell.Style.Font = new Font(dataGridView.Font.FontFamily, 10, FontStyle.Regular); // Restablece el tamaño de letra a 10 y quita la negrita
                        cell.Style.ForeColor = Color.Black;
                    }
                }
            }
        }

        private void FormBodega_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
    }
}
