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

namespace Proyecto.Editar
{
    public partial class FormEditarBodega : Form
    {
        public FormEditarBodega()
        {
            InitializeComponent();
            this.ControlBox = false;
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
      (
          int nLeftRect,     // x-coordinate of upper-left corner
          int nTopRect,      // y-coordinate of upper-left corner
          int nRightRect,    // x-coordinate of lower-right corner
          int nBottomRect,   // y-coordinate of lower-right corner
          int nWidthEllipse, // height of ellipse
          int nHeightEllipse // width of ellipse
      );

        // Para arrastrar y mover el formulario usarlo para poder arrastrar de linea 35  a 41
        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void button1_Click(object sender, EventArgs e)
        {
            Mysql.Cbodega objetoBodega = new Mysql.Cbodega();
            objetoBodega.modificarBodegas(textBox1, textBox2, dateTimePicker1, dateTimePicker2, numericUpDown1, numericUpDown2, numericUpDown3);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int searchValue))
            {
                Mysql.Cbodega objetoBodega = new Mysql.Cbodega();
                DataRow bodega = objetoBodega.buscarBodega(searchValue);

                if (bodega != null)
                {
                    // Asignar valores a los controles
                    textBox2.Text = bodega["pro_nom"].ToString();
                    dateTimePicker1.Value = Convert.ToDateTime(bodega["bo_fecing"]);
                    dateTimePicker2.Value = Convert.ToDateTime(bodega["pro_cad"]);
                    numericUpDown1.Value = Convert.ToDecimal(bodega["pro_can"]);
                    numericUpDown2.Value = Convert.ToDecimal(bodega["pro_cos"]);
                    numericUpDown3.Value = Convert.ToDecimal(bodega["pro_pre"]);
                }
                else
                {
                    MessageBox.Show("Bodega no encontrada.");
                }
            }
            else
            {
                MessageBox.Show("Ingrese un valor numérico válido para buscar.");
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void FormEditarBodega_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                Region = null; // Restablece la región a null al maximizar
            }
            else if (WindowState == FormWindowState.Normal)
            {
                Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20)); // Aplica la región redondeada al restaurar
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                textBox2.Focus();
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                dateTimePicker1.Focus();
            }
        }

        private void dateTimePicker1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                dateTimePicker2.Focus();
            }
        }

        private void dateTimePicker2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                numericUpDown1.Focus();
            }
        }

        private void numericUpDown1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                numericUpDown2.Focus();
            }
        }

        private void numericUpDown2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                numericUpDown3.Focus();
            }
        }

        private void FormEditarBodega_Load(object sender, EventArgs e)
        {
            numericUpDown3.Enabled = false;
        }
    }
}
