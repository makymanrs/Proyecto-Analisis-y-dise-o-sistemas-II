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
    public partial class FormEditarCliente : Form
    {
        public FormEditarCliente()
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

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void FormEditarCliente_Resize(object sender, EventArgs e)
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
                textBox3.Focus();
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                textBox4.Focus();
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                textBox5.Focus();
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                textBox6.Focus();
            }
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                textBox7.Focus();
            }
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                richTextBox1.Focus();
            }
        }

        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                textBox8.Focus();
            }
        }
    }
}
