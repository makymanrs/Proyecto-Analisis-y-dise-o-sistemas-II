using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Proyecto
{
    public partial class LoginRegistro : Form
    {
        private Timer fadeOutTimer;
        private Timer fadeInTimer;
        private Form loginForm;

        public LoginRegistro()
        {
            InitializeComponent();
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            fadeOutTimer = new Timer();
            fadeOutTimer.Interval = 10; // Adjust this value to change the speed of the animation
            fadeOutTimer.Tick += FadeOutTimer_Tick;

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

        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void label5_Click(object sender, EventArgs e)
        {
            if (loginForm == null || loginForm.IsDisposed)
            {
                loginForm = new Login();
                loginForm.Opacity = 0;
            }

            // Ocultar el formulario actual con animación de desvanecimiento
            fadeOutTimer.Start();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.PasswordChar = checkBox1.Checked ? '\0' : '*';
            textBox3.PasswordChar = checkBox1.Checked ? '\0' : '*';
        }

        private void FadeOutTimer_Tick(object sender, EventArgs e)
        {
            this.Opacity -= 0.05;
            if (this.Opacity <= 0)
            {
                fadeOutTimer.Stop();
                this.Hide();
                ShowLoginForm();
            }
        }

        private void ShowLoginForm()
        {
            if (loginForm != null)
            {
                loginForm.Show();
                fadeInTimer.Start();
            }
        }

        private void FadeInTimer_Tick(object sender, EventArgs e)
        {
            if (loginForm != null)
            {
                loginForm.Opacity += 0.05;
                if (loginForm.Opacity >= 1)
                {
                    loginForm.Opacity = 1;
                    fadeInTimer.Stop();
                }
            }
        }
    }
}
