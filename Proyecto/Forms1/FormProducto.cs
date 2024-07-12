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
    public partial class FormProducto : Form
    {
        public FormProducto()
        {
            InitializeComponent();
            Mysql.Cproducto objetoproducto = new Mysql.Cproducto();
            objetoproducto.mostrarproductos(dataGridProducto);
            dataGridProducto.Margin = new Padding(10); // Ajusta según lo necesario

            // Ajustar el padding de las celdas
            foreach (DataGridViewColumn column in dataGridProducto.Columns)
            {
                column.DefaultCellStyle.Padding = new Padding(0); // Ajusta según lo necesario
            }
            dataGridProducto.RowTemplate.Height = 80;
            dataGridProducto.ReadOnly=true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Mysql.Cproducto objetoProducto = new Mysql.Cproducto();
            objetoProducto.buscarProductoPorId(dataGridProducto, textBox1);
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
                using (Ingresar.FormIngresarProducto formIngresarProducto = new Ingresar.FormIngresarProducto())
                {
                    formIngresarProducto.StartPosition = FormStartPosition.CenterScreen;
                    formIngresarProducto.ShowDialog(FormBG);
                }

                // Cerrar el formulario oscuro cuando se cierre el formulario de ingreso de proveedor
                FormBG.Dispose();
            }
               //Actualizar el DataGridView y el conteo de registros
               Mysql.Cproducto objetoProducto = new Mysql.Cproducto();
               objetoProducto.mostrarproductos(dataGridProducto);
               ActualizarConteoRegistros();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Mysql.Cproducto objetoProducto = new Mysql.Cproducto();
            objetoProducto.mostrarproductos(dataGridProducto);
            ActualizarConteoRegistros();
        }

        private void button4_Click(object sender, EventArgs e)
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
                using (Editar.FormEditarProducto formEditarProducto = new Editar.FormEditarProducto())
                {
                    formEditarProducto.StartPosition = FormStartPosition.CenterScreen;
                    formEditarProducto.ShowDialog(FormBG);
                }

                // Cerrar el formulario oscuro cuando se cierre el formulario de ingreso de proveedor
                FormBG.Dispose();
            }
            Mysql.Cproducto objetoProducto = new Mysql.Cproducto();
            objetoProducto.mostrarproductos(dataGridProducto);
            ActualizarConteoRegistros();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Mysql.Cproducto objetoProducto = new Mysql.Cproducto();
            objetoProducto.eliminarProductos(textBox1, dataGridProducto);
            objetoProducto.mostrarproductos(dataGridProducto);
            ActualizarConteoRegistros();

        }

        private void ActualizarConteoRegistros()
        {
            int totalRegistros = dataGridProducto.RowCount;
            if (dataGridProducto.AllowUserToAddRows)
            {
                totalRegistros--; // Restar 1 si AllowUserToAddRows está activado
            }

            label2.Text = "Total de registros: " + totalRegistros;
        }

        private void FormProducto_Load(object sender, EventArgs e)
        {
            Mysql.Cproducto objetoProducto = new Mysql.Cproducto();
            objetoProducto.mostrarproductos(dataGridProducto);
            ActualizarConteoRegistros();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                // Crear una instancia de la clase Cbodega
                Cproducto bodegaProducto = new Cproducto();

                // Llamar al método exportarExcel con el DataGridView que contiene los datos
                bodegaProducto.exportarExcel(dataGridProducto);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar a Excel: " + ex.Message);
            }
        }

        private void dataGridProducto_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Mysql.Cproducto objetoProducto = new Mysql.Cproducto();
            objetoProducto.seleccionarProducto(dataGridProducto, textBox1);
        }

        private void dataGridProducto_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
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
