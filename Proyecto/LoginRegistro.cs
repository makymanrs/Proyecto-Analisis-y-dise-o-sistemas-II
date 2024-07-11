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
        private bool isRegistering = false;
        private Mysql.Cusuario cusuario;

        public LoginRegistro()
        {
            InitializeComponent();
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            fadeOutTimer = new Timer();
            fadeOutTimer.Interval = 10; // Ajusta este valor para cambiar la velocidad de la animación
            fadeOutTimer.Tick += FadeOutTimer_Tick;

            fadeInTimer = new Timer();
            fadeInTimer.Interval = 10; // Ajusta este valor para cambiar la velocidad de la animación
            fadeInTimer.Tick += FadeInTimer_Tick;

            // Inicializar el formulario de inicio de sesión
            loginForm = new Login();
            loginForm.Opacity = 0;

            // Inicializar instancia de Cusuario
            cusuario = new Mysql.Cusuario();
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
            isRegistering = false;
            FadeOutForm();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox2.PasswordChar = '\0';
                textBox3.PasswordChar = '\0';
            }
            else
            {
                textBox2.PasswordChar = '*';
                textBox3.PasswordChar = '*';
            }
        }

        private void FadeOutForm()
        {
            fadeOutTimer.Start();
        }

        private void FadeInForm()
        {
            fadeInTimer.Start();
        }

        private void FadeOutTimer_Tick(object sender, EventArgs e)
        {
            this.Opacity -= 0.05;
            if (this.Opacity <= 0)
            {
                fadeOutTimer.Stop();
                if (isRegistering)
                {
                    // Ejecutar el registro después de que el formulario se haya ocultado
                    if (cusuario.RegistrarUsuario(textBox1, textBox2, textBox3))
                    {
                        // Registro exitoso, mostrar el formulario de inicio de sesión
                        this.Hide();
                        ShowLoginForm();
                    }
                    else
                    {
                        // Registro fallido, volver a mostrar el formulario de registro
                        this.Opacity = 1;
                    }
                }
                else
                {
                    // No es un registro, mostrar el formulario de inicio de sesión
                    this.Hide();
                    ShowLoginForm();
                }
            }
        }

        private void ShowLoginForm()
        {
            if (loginForm != null)
            {
                loginForm.Show();
                FadeInForm();
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

        private void button2_Click(object sender, EventArgs e)
        {
            isRegistering = true;
            FadeOutForm();
        }
    }
}
