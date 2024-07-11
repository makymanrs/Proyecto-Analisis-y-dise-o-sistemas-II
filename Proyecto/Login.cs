using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Proyecto
{
    public partial class Login : Form
    {
        private Timer fadeOutTimer;
        private Timer fadeInTimer;
        private Form loginRegisterForm;

        public Login()
        {
            InitializeComponent();
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            // Initialize the fade out timer
            fadeOutTimer = new Timer();
            fadeOutTimer.Interval = 10; // Adjust this value to change the speed of the animation
            fadeOutTimer.Tick += FadeOutTimer_Tick;

            // Initialize the fade in timer
            fadeInTimer = new Timer();
            fadeInTimer.Interval = 10; // Adjust this value to change the speed of the animation
            fadeInTimer.Tick += FadeInTimer_Tick;
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

        private void button11_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            // Crear una instancia del formulario LoginRegistro
            loginRegisterForm = new LoginRegistro();
            loginRegisterForm.Opacity = 0; // Iniciar con opacidad cero
            loginRegisterForm.Show();

            // Iniciar la animación de desvanecimiento del formulario actual
            fadeOutTimer.Start();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
              
        }

        private void FadeOutTimer_Tick(object sender, EventArgs e)
        {
            this.Opacity -= 0.05;
            if (this.Opacity <= 0)
            {
                fadeOutTimer.Stop();
                this.Hide();
                fadeInTimer.Start(); // Start fade in for the new form
            }
        }

        private void FadeInTimer_Tick(object sender, EventArgs e)
        {
            if (loginRegisterForm != null)
            {
                loginRegisterForm.Opacity += 0.05;
                if (loginRegisterForm.Opacity >= 1)
                {
                    loginRegisterForm.Opacity = 1;
                    fadeInTimer.Stop();
                }
            }
        }
    }
}
