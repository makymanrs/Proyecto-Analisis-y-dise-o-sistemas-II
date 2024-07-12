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
            foreach (DataGridViewColumn column in dataGridViewFactura.Columns)
            {
                column.DefaultCellStyle.Padding = new Padding(0); // Ajusta según lo necesario
            }
            dataGridViewFactura.RowTemplate.Height = 50;
            dataGridViewFactura.ReadOnly= true;
            ActualizarConteoRegistros();
        }
        private void ActualizarConteoRegistros()
        {
            int totalRegistros = dataGridViewFactura.RowCount;
            if (dataGridViewFactura.AllowUserToAddRows)
            {
                totalRegistros--; // Restar 1 si AllowUserToAddRows está activado
            }
            label2.Text = "Total de registros: " + totalRegistros;
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

                // Crear y mostrar el formulario para ingresar Cliente
                using (HistorialCredito.FormHistorialDetalles historialDetalle = new HistorialCredito.FormHistorialDetalles())
                {
                    historialDetalle.StartPosition = FormStartPosition.CenterScreen;
                    historialDetalle.ShowDialog(FormBG);
                }

                // Cerrar el formulario oscuro cuando se cierre el formulario de ingreso de Cliente
                FormBG.Dispose();
            }
            //Actualizar el DataGridView y el conteo de registros
              Mysql.Cdetalle historialfactura = new Mysql.Cdetalle();
              historialfactura.MostrarFactura(dataGridViewFactura);
              ActualizarConteoRegistros();
        }

       public void listas()
        {
            comboBox1.Items.Add("Ninguno");
            comboBox1.Items.Add("Codigo Factura");
            comboBox1.Items.Add("Nombre del cliente");
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Mysql.Cdetalle objetoDetalle = new Mysql.Cdetalle();
            objetoDetalle.BuscarFacturaPorFiltros(dataGridViewFactura,textBox1 ,comboBox1);
            ActualizarConteoRegistros();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Mysql.Cdetalle objetoDetalle = new Mysql.Cdetalle();
            objetoDetalle.MostrarFactura(dataGridViewFactura);
            Mysql.Cdetalle historialfactura = new Mysql.Cdetalle();
            historialfactura.MostrarFactura(dataGridViewFactura);
            ActualizarConteoRegistros();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            Mysql.Cdetalle objetoDetalle = new Mysql.Cdetalle();
            objetoDetalle.eliminarFactura(textBox1);
            Mysql.Cdetalle historialfactura = new Mysql.Cdetalle();
            historialfactura.MostrarFactura(dataGridViewFactura);
            ActualizarConteoRegistros();
        }

        private void FormHistorial_Load(object sender, EventArgs e)
        {
            listas();
        }

        private void dataGridViewFactura_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Mysql.Cdetalle objetoDetalle = new Mysql.Cdetalle();
            objetoDetalle.selecciondarDetalle(dataGridViewFactura, textBox1);
        }

        private void dataGridViewFactura_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
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
