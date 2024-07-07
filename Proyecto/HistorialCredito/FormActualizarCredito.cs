using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
        }

        private void button11_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {

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

        }
    }
}
