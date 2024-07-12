using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto.Forms1
{
    public partial class FormProveedor : Form
    {
        public FormProveedor()
        {
            InitializeComponent();
            Mysql.Cproveedor objetoProveedor = new Mysql.Cproveedor();
            objetoProveedor.mostrarProveedor(dataGridProveedor);
            ActualizarConteoRegistros(); // Actualizar el conteo de registros después de mostrar los datos
            dataGridProveedor.ReadOnly=true;
            
            dataGridProveedor.RowTemplate.Height = 50;
        }

        private void button3_Click_1(object sender, EventArgs e)
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
                using (Ingresar.FormIngresarProveedor formIngresarProveedor = new Ingresar.FormIngresarProveedor())
                {
                    formIngresarProveedor.StartPosition = FormStartPosition.CenterScreen;
                    formIngresarProveedor.ShowDialog(FormBG);
                }

                // Cerrar el formulario oscuro cuando se cierre el formulario de ingreso de proveedor
                FormBG.Dispose();
            }
            // Actualizar el DataGridView y el conteo de registros
            Mysql.Cproveedor objetoProveedor = new Mysql.Cproveedor();
            objetoProveedor.mostrarProveedor(dataGridProveedor);
            ActualizarConteoRegistros();
        }

        // Conteo de registros
        private void ActualizarConteoRegistros()
        {
            int totalRegistros = dataGridProveedor.RowCount;
            if (dataGridProveedor.AllowUserToAddRows)
            {
                totalRegistros--; // Restar 1 si AllowUserToAddRows está activado
            }

            label2.Text = "Total de registros: " + totalRegistros;
        }

        private void FormProveedor_Load(object sender, EventArgs e)
        {
            Mysql.Cproveedor objetoProveedor = new Mysql.Cproveedor();
            objetoProveedor.mostrarProveedor(dataGridProveedor);
            ActualizarConteoRegistros();
        }

        private void button1_Click(object sender, EventArgs e)
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
                using (Editar.FormEditarProveedor formEditarProveedor = new Editar.FormEditarProveedor())
                {
                    formEditarProveedor.StartPosition = FormStartPosition.CenterScreen;
                    formEditarProveedor.ShowDialog(FormBG);
                }

                // Cerrar el formulario oscuro cuando se cierre el formulario de ingreso de proveedor
                FormBG.Dispose();
            }
            // Actualizar el DataGridView y el conteo de registros
            Mysql.Cproveedor objetoProveedor = new Mysql.Cproveedor();
            objetoProveedor.mostrarProveedor(dataGridProveedor);
            ActualizarConteoRegistros();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Mysql.Cproveedor objetoProveedor = new Mysql.Cproveedor();
            objetoProveedor.eliminarProveedor(textBox1, dataGridProveedor);
            objetoProveedor.mostrarProveedor(dataGridProveedor);
            ActualizarConteoRegistros(); // Actualizar el conteo de registros después de eliminar un proveedor
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Mysql.Cproveedor objetoProveedor = new Mysql.Cproveedor();
            objetoProveedor.buscarProveedorPorId(dataGridProveedor, textBox1);
            ActualizarConteoRegistros(); // Actualizar el conteo de registros después de buscar un proveedor
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Mysql.Cproveedor objetoProveedor = new Mysql.Cproveedor();
            objetoProveedor.mostrarProveedor(dataGridProveedor);
            ActualizarConteoRegistros(); // Actualizar el conteo de registros después de mostrar proveedores
        }

        private void dataGridProveedor_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Mysql.Cproveedor objetoProveedor = new Mysql.Cproveedor();
            objetoProveedor.seleccionarProveedor(dataGridProveedor, textBox1);
        }

        private void dataGridProveedor_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
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
    }
}
