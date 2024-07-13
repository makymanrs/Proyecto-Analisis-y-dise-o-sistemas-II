using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Proyecto.Ingresar
{
    public partial class FormIngresarProveedor : Form
    {
        public FormIngresarProveedor()
        {
            InitializeComponent();
            this.ControlBox = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            // Set up the rounded button

        }

        // Redondear bordes de formulario y botones 
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

        // Para arrastrar y mover el formulario
        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        public void listas()
        {
            comboBox1.Items.Add("Ninguno");
            comboBox1.Items.Add("Alimentos");
            comboBox1.Items.Add("Bebidas");
            comboBox1.Items.Add("Alimentos y bebidas");
            comboBox1.Items.Add("Dulces");
            comboBox1.Items.Add("Cuidado del hogar");
            comboBox1.Items.Add("Cuidado Personal");
            comboBox1.Items.Add("Bandejas y vasos");
            comboBox1.Items.Add("Todos");

        }

        private void FormIngresarProveedor_Load(object sender, EventArgs e)
        {
            listas();
            button1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, button1.Width, button1.Height, 10, 10));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Mysql.Cproveedor objetoProveedor = new Mysql.Cproveedor();
            objetoProveedor.guardarProveedor(textBox1, textBox2, textBox3, comboBox1, textBox4, textBox5);
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void FormIngresarProveedor_Resize(object sender, EventArgs e)
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

        private void button11_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // key press eventos
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
                comboBox1.Focus();
                comboBox1.DroppedDown = true;
            }
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
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
                button1.Focus();
            }
        }
    }
}
