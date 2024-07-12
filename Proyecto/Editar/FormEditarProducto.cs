using Proyecto.Mysql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Proyecto.Editar
{
    public partial class FormEditarProducto : Form
    {
        public FormEditarProducto()
        {
            InitializeComponent();
            this.ControlBox = false;
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            
        }

        // esquinas redondeadas obligatorio agregarlo las coordenadas
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

        public void proveedores()
        {
            comboBox1.Items.Add("No Requiere");
            Cproducto objetoProducto = new Cproducto();
            objetoProducto.cargarNombresProveedores(comboBox1);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                Mysql.Cproducto objetoProducto = new Mysql.Cproducto();
                DataRow producto = objetoProducto.buscarProducto(textBox1.Text);

                if (producto != null)
                {
                    // Asignar valores a los controles
                    textBox2.Text = producto["pro_nom"].ToString();
                    DateTime fechaCaducidad = Convert.ToDateTime(producto["pro_cad"]);
                    dateTimePicker1.Value = fechaCaducidad; // Asignar la fecha de caducidad
                    numericUpDown1.Value = Convert.ToDecimal(producto["pro_cos"]);
                    numericUpDown2.Value = Convert.ToDecimal(producto["pro_pre"]);
                    numericUpDown3.Value = Convert.ToDecimal(producto["pro_can"]);
                    comboBox1.Text = producto["prove_nom"].ToString();
                    textBox3.Text = producto["bo_id"].ToString();

                    // Mostrar la imagen del producto si existe
                    if (producto["pro_img"] != DBNull.Value)
                    {
                        byte[] imgBytes = (byte[])producto["pro_img"];
                        using (MemoryStream ms = new MemoryStream(imgBytes))
                        {
                            pictureBox2.Image = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        pictureBox2.Image = null; // Limpiar el PictureBox si no hay imagen
                    }
                }
                else
                {
                    MessageBox.Show("Producto no encontrado.");
                }
            }
            else
            {
                MessageBox.Show("Ingrese un código válido para buscar.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Mysql.Cproducto objetoProducto = new Mysql.Cproducto();
            objetoProducto.modificarProducto(textBox1, textBox2, dateTimePicker1, numericUpDown1, numericUpDown2, numericUpDown3, textBox3, comboBox1, pictureBox2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pictureBox2.Image = Image.FromFile(ofd.FileName);
                }
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormEditarProducto_Resize(object sender, EventArgs e)
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

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
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
                numericUpDown1.Focus();
            }
        }

        private void numericUpDown1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                numericUpDown3.Focus();
            }
        }

        private void numericUpDown2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                numericUpDown3.Focus();
            }
        }

        private void numericUpDown3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                comboBox1.Focus();
                comboBox1.DroppedDown = true;
            }
        }

        private void FormEditarProducto_Load(object sender, EventArgs e)
        {
            numericUpDown2.Enabled = false;
            proveedores();
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                textBox3.Focus();
            }
        }
    }
}
