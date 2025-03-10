﻿using Google.Protobuf.WellKnownTypes;
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

namespace Proyecto.HistorialCredito
{
    public partial class FormActualizarCredito : Form
    {
        public FormActualizarCredito()
        {
            InitializeComponent();
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            numericUpDown1.Enabled = false;
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
            this.Close();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void FormActualizarCredito_Resize(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int searchValue))
            {
                Mysql.Ccredito objetoCredito = new Mysql.Ccredito();
                DataRow credito = objetoCredito.buscarCredito(searchValue);

                if (credito != null)
                {
                    // Asignar valores a los controles
                    textBox2.Text = credito["Nombre Cliente"].ToString();
                    numericUpDown1.Value = Convert.ToDecimal(credito["cre_monto"]);
                    //  checkBoxPagado.Checked = Convert.ToBoolean(credito["cre_pagado"]);
                }
                else
                {
                    MessageBox.Show("Crédito no encontrado.");
                }
            }
            else
            {
                MessageBox.Show("Ingrese un valor numérico válido para buscar.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
                decimal abono = numericUpDown2.Value;
                DateTime fechaPagado = dateTimePicker1.Value;

                Mysql.Ccredito objetoCredito = new Mysql.Ccredito();
                objetoCredito.actualizarCredito(textBox1, abono, fechaPagado);

                // Actualizar el DataGridView después de la actualización
                //ActualizarDataGridView();
            
        }

        private void FormActualizarCredito_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
    }
}
