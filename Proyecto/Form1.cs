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

namespace Proyecto
{
    public partial class Form1 : Form
    {
        // Variables
        private Button actualBoton; // currentButton
        private Random random;
        private int tempIndex;
        private Form activarform;

        private Timer animationTimer;
        private int targetMenuWidth;
        private int targetLogoWidth;
        private int initialMenuWidth;
        private int initialLogoWidth;
        private bool isExpanding;

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

        public Form1()
        {
            InitializeComponent();
            InitializeTimer();
            random = new Random();
            btnCloseChild.Visible = false;
            // quita los bordes de arriba
            this.Text = string.Empty;
            this.ControlBox = false;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            this.Resize += new EventHandler(Form1_Resize); // Agrega el manejador de eventos para el cambio de tamaño
            btnMenu.Enabled = true;
        }

        private void InitializeTimer()
        {
            animationTimer = new Timer();
            animationTimer.Interval = 15; // Intervalo de 15 ms para una animación suave
            animationTimer.Tick += new EventHandler(AnimationTick);
        }

        //Son parar arrastrar y mover
        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        // Métodos
        private Color SeleccionarColor() // SelectThemeColor()
        {
            int index = random.Next(TemasColores.listaColores.Count);
            // Evita seleccionar el mismo color dos veces seguidas
            while (tempIndex == index)
            {
                index = random.Next(TemasColores.listaColores.Count);
            }
            tempIndex = index;
            string color = TemasColores.listaColores[index];
            return ColorTranslator.FromHtml(color);
        }

        private void ActivarBoton(object btnSender) // ActivateButton
        {
            if (btnSender != null)
            {
                if (actualBoton != (Button)btnSender)
                {
                    DeshabilitarBoton();
                    Color color = SeleccionarColor();
                    actualBoton = (Button)btnSender;
                    actualBoton.BackColor = color;
                    actualBoton.ForeColor = Color.White;
                    actualBoton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    panelTitleBar.BackColor = color;
                    // panelLogo.BackColor = TemasColores.CambiarBrillo(color, -0.3);
                    TemasColores.PrimaryColor = color;
                    TemasColores.SecondaryColor = TemasColores.CambiarBrillo(color, -0.3);
                    btnCloseChild.Visible = true;
                }
            }
        }

        private void DeshabilitarBoton()
        {
            foreach (Control previousBtn in panelMenu.Controls)
            {
                if (previousBtn.GetType() == typeof(Button))
                {
                    previousBtn.BackColor = Color.FromArgb(35, 40, 45);
                    previousBtn.ForeColor = Color.Gainsboro;
                    previousBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                }
            }
        }

        private void OpenChildForm(Form childForm, object btnSender)
        {
            if (activarform != null)
            {
                activarform.Close();
            }
            ActivarBoton(btnSender);
            activarform = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            this.panelDesktopPane.Controls.Add(childForm);
            this.panelDesktopPane.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
            labelTittle.Text = childForm.Text;
        }

        private void btnCloseChild_Click(object sender, EventArgs e)
        {
            if (activarform != null)
                activarform.Close();
            Reset();
        }

        private void Reset()
        {
            DeshabilitarBoton();
            labelTittle.Text = "HOME";
            panelTitleBar.BackColor = Color.FromArgb(26, 25, 62);
            panelLogo.BackColor = Color.FromArgb(35, 40, 45);
            actualBoton = null;
            btnCloseChild.Visible = false;

            // Asegúrate de que todos los botones estén en su color predeterminado
            foreach (Control previousBtn in panelMenu.Controls)
            {
                if (previousBtn.GetType() == typeof(Button))
                {
                    previousBtn.BackColor = Color.FromArgb(35, 40, 45);
                    previousBtn.ForeColor = Color.Gainsboro;
                    previousBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                }
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMaximizar_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                Region = null; // Restablece la región a null al maximizar
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20)); // Aplica la región redondeada al restaurar
            }
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void panelTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void Form1_Resize(object sender, EventArgs e)
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

        private void button6_Click(object sender, EventArgs e)
        {
            ColapsarMenu();
        }

        private void ColapsarMenu()
        {
            if (panelMenu.Width == 200)
            {
                targetMenuWidth = 64;
                targetLogoWidth = 64;
                isExpanding = false;
            }
            else
            {
                targetMenuWidth = 200;
                targetLogoWidth = 200;
                isExpanding = true;
            }

            initialMenuWidth = panelMenu.Width;
            initialLogoWidth = panelLogo.Width;

            // Ocultar los textos de los botones antes de iniciar la animación
            foreach (Control control in panelMenu.Controls)
            {
                if (control is Button)
                {
                    control.Text = "";
                }
            }

            animationTimer.Start();
        }

        private void AnimationTick(object sender, EventArgs e)
        {
            int step = 20; // Aumenta este valor para hacer la animación más rápida

            // Animar el ancho del panelMenu
            if (panelMenu.Width != targetMenuWidth)
            {
                if (panelMenu.Width < targetMenuWidth)
                {
                    panelMenu.Width = Math.Min(panelMenu.Width + step, targetMenuWidth);
                }
                else
                {
                    panelMenu.Width = Math.Max(panelMenu.Width - step, targetMenuWidth);
                }
            }

            // Animar el ancho del panelLogo
            if (panelLogo.Width != targetLogoWidth)
            {
                if (panelLogo.Width < targetLogoWidth)
                {
                    panelLogo.Width = Math.Min(panelLogo.Width + step, targetLogoWidth);
                }
                else
                {
                    panelLogo.Width = Math.Max(panelLogo.Width - step, targetLogoWidth);
                }
            }

            // Ajustar la posición de btnMenu para que se mantenga en (3, 35)
            btnMenu.Location = new Point(3, 35);

            // Detener el temporizador cuando se alcance el tamaño objetivo y ajustar los textos de los botones
            if (panelMenu.Width == targetMenuWidth && panelLogo.Width == targetLogoWidth)
            {
                animationTimer.Stop();

                // Mostrar los textos de los botones solo si el panelMenu está expandido
                if (panelMenu.Width == 200)
                {
                    foreach (Control control in panelMenu.Controls)
                    {
                        if (control is Button button)
                        {
                            button1.Text = "           Producto"; // Reemplaza "Button" con el texto que desees para cada botón
                            button2.Text = "           Clientes";
                            button3.Text = "           Pagos";
                            button4.Text = "           Bodega";
                            button5.Text = "           Credito";
                            button6.Text = "            Orden";
                            button7.Text = "             Historial";
                            button8.Text = "             Proveedor";
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms1.FormProducto(), sender);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms1.FormCliente(), sender);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms1.FormFactura(), sender);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms1.FormBodega(), sender);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //OpenChildForm(new Forms1.FormProducto(), sender);
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            //OpenChildForm(new Forms1.FormProducto(), sender);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //OpenChildForm(new Forms1.FormProveedor(), sender);

        }

        private void button8_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms1.FormProveedor(), sender);
        }

        private void horafecha_Tick(object sender, EventArgs e)
        {
            labelhr.Text= DateTime.Now.ToString("h:mm:ss");
            labelfch.Text = DateTime.Now.ToLongDateString();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                Region = null; // Restablece la región a null al maximizar
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20)); // Aplica la región redondeada al restaurar
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
