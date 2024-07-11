using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Proyecto
{
    public partial class Login : Form
    {
        private Timer fadeOutTimerLoginRegistro;
        private Timer fadeInTimerLoginRegistro;
        private Form loginRegisterForm;
        private Form form1;

        public Login()
        {
            InitializeComponent();
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            // Initialize the fade out and fade in timers for LoginRegistro
            fadeOutTimerLoginRegistro = new Timer();
            fadeOutTimerLoginRegistro.Interval = 10; // Adjust this value to change the speed of the animation
            fadeOutTimerLoginRegistro.Tick += FadeOutTimerLoginRegistro_Tick;

            fadeInTimerLoginRegistro = new Timer();
            fadeInTimerLoginRegistro.Interval = 10; // Adjust this value to change the speed of the animation
            fadeInTimerLoginRegistro.Tick += FadeInTimerLoginRegistro_Tick;
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
            // Create an instance of LoginRegistro form
            loginRegisterForm = new LoginRegistro();
            loginRegisterForm.Opacity = 0; // Start with zero opacity

            // Show LoginRegistro form
            loginRegisterForm.Show();

            // Start fade out animation for current form (Login)
            fadeOutTimerLoginRegistro.Start();
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox2.PasswordChar = '\0';
            }
            else
            {
                textBox2.PasswordChar = '*';
            }
        }

        private void FadeOutTimerLoginRegistro_Tick(object sender, EventArgs e)
        {
            this.Opacity -= 0.05;
            if (this.Opacity <= 0)
            {
                fadeOutTimerLoginRegistro.Stop();
                this.Hide();

                // Show LoginRegistro after current form has completely hidden
                if (loginRegisterForm == null || loginRegisterForm.IsDisposed)
                {
                    loginRegisterForm = new LoginRegistro();
                    loginRegisterForm.Opacity = 0;
                }
                loginRegisterForm.Show();

                // Start fade in animation for LoginRegistro
                fadeInTimerLoginRegistro.Start();
            }
        }

        private void FadeInTimerLoginRegistro_Tick(object sender, EventArgs e)
        {
            if (loginRegisterForm != null)
            {
                loginRegisterForm.Opacity += 0.05;
                if (loginRegisterForm.Opacity >= 1)
                {
                    loginRegisterForm.Opacity = 1;
                    fadeInTimerLoginRegistro.Stop();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Mysql.Cusuario cusuario = new Mysql.Cusuario();
            cusuario.inicioSesion(textBox1, textBox2,this);
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
                button2.Focus();
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
    }
}
