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
            foreach (DataGridViewColumn column in dataGridViewCredito.Columns)
            {
                column.DefaultCellStyle.Padding = new Padding(0); // Ajusta según lo necesario
            }
            dataGridViewCredito.RowTemplate.Height = 50;
            dataGridViewCredito.ReadOnly = true;
            ActualizarConteoRegistros();
        }
        public void listas()
        {
            comboBox1.Items.Add("Ninguno");
            comboBox1.Items.Add("Nombre del cliente");
            comboBox1.Items.Add("Codigo Factura");
            comboBox1.Items.Add("Codigo Credito");
            comboBox1.SelectedIndex = 0;

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
            Mysql.Ccredito objetoCredito = new Mysql.Ccredito();
            objetoCredito.mostrarCredito(dataGridViewCredito);
            ActualizarConteoRegistros();
        }
        private void ActualizarConteoRegistros()
        {
            int totalRegistros = dataGridViewCredito.RowCount;
            if (dataGridViewCredito.AllowUserToAddRows)
            {
                totalRegistros--; // Restar 1 si AllowUserToAddRows está activado
            }
            label2.Text = "Total de registros: " + totalRegistros;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Mysql.Ccredito objetoCredito = new Mysql.Ccredito();
            objetoCredito.mostrarCredito(dataGridViewCredito);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Mysql.Ccredito objetoCredito = new Mysql.Ccredito();
            objetoCredito.BuscarCreditosporFiltros(dataGridViewCredito, textBox1, comboBox1);
            ActualizarConteoRegistros();
        }

        private void FormCredito_Load(object sender, EventArgs e)
        {
            listas();
            comboBox1.Focus();
            comboBox1.DroppedDown = true;
        }

        private void dataGridViewCredito_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Mysql.Ccredito objetoCredito = new Mysql.Ccredito();
            objetoCredito.seleccionarCredito(dataGridViewCredito, textBox1);
        }

        private void dataGridViewCredito_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
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

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                textBox1.Focus();
            }
        }
    }
}
