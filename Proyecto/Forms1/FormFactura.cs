using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto.Forms1
{
    public partial class FormFactura : Form
    {
        public FormFactura()
        {
            InitializeComponent();
            ConfigurarDataGridView();
            ActualizarTotalPagar();
            ActualizarSubtotal();
            ActualizarTotalImpuesto();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Obtener los valores de los controles
            string codigoProducto = textBox4.Text;
            string nombreProducto = textBox5.Text;
            int cantidad = (int)numericUpDown1.Value;
            decimal precio = numericUpDown2.Value;
            decimal subtotal = cantidad * precio; // Calcular el subtotal
            decimal impuesto = subtotal * 0.15m; // Calcular el impuesto del 15%
            decimal total = subtotal + impuesto; // Calcular el total

            // Actualizar los controles de impuestos y total
            numericUpDown3.Value = impuesto;
            numericUpDown4.Value = total;

            // Agregar una nueva fila al DataGridView
            dataGridFactura.Rows.Add(codigoProducto, nombreProducto, cantidad, precio, impuesto, total);

            // Actualizar el total a pagar, el subtotal y el total del impuesto
            ActualizarTotalPagar();
            ActualizarSubtotal();
            ActualizarTotalImpuesto();
        }

        private void ActualizarTotalPagar()
        {
            decimal totalPagar = 0;

            foreach (DataGridViewRow row in dataGridFactura.Rows)
            {
                if (row.Cells["total"].Value != null)
                {
                    totalPagar += Convert.ToDecimal(row.Cells["total"].Value);
                }
            }

            //label total a pagar
            labelTotalPagar.Text = "Total a Pagar: " + totalPagar.ToString("C"); // Mostrar el total en el label
            label20.Text = "Total a Pagar..: " + totalPagar.ToString("C");
        }

        // Método para calcular el subtotal
        private void ActualizarSubtotal()
        {
            decimal subtotal = 0;

            foreach (DataGridViewRow row in dataGridFactura.Rows)
            {
                if (row.Cells["cantidad"].Value != null && row.Cells["precio"].Value != null)
                {
                    int cantidad = Convert.ToInt32(row.Cells["cantidad"].Value);
                    decimal precio = Convert.ToDecimal(row.Cells["precio"].Value);
                    subtotal += cantidad * precio;
                }
            }

            //label subtotal
            labelSub.Text = "Subtotal.........: " + subtotal.ToString("C"); // Mostrar el subtotal en el label
        }

        // Método para calcular el total del impuesto
        private void ActualizarTotalImpuesto()
        {
            decimal totalImpuesto = 0;

            foreach (DataGridViewRow row in dataGridFactura.Rows)
            {
                if (row.Cells["impuesto"].Value != null)
                {
                    totalImpuesto += Convert.ToDecimal(row.Cells["impuesto"].Value);
                }
            }

            //label impuesto
            labelImp.Text = "Total Impuesto: " + totalImpuesto.ToString("C"); // Mostrar el total del impuesto en el label
        }
        private void ConfigurarDataGridView()
        {
            // Agregar columnas al DataGridView manualmente
            dataGridFactura.Columns.Add("codigoProducto", "Código de Producto");
            dataGridFactura.Columns.Add("nombreProducto", "Nombre de Producto");
            dataGridFactura.Columns.Add("cantidad", "Cantidad");
            dataGridFactura.Columns.Add("precio", "Precio");
            dataGridFactura.Columns.Add("impuesto", "Impuesto");
            dataGridFactura.Columns.Add("total", "Total");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Obtener el monto a restar ingresado en textBox9
            if (decimal.TryParse(textBox9.Text, out decimal montoAPagar))
            {
                // Mostrar el contenido actual de labelTotalPagar para depuración
                MessageBox.Show("Contenido de labelTotalPagar: " + labelTotalPagar.Text);

                // Obtener el texto actual en labelTotalPagar
                string labelText = labelTotalPagar.Text;

                // Verificar si el texto contiene "Total a Pagar: "
                if (labelText.Contains("Total a Pagar: "))
                {
                    // Extraer la parte del texto que representa el valor a pagar
                    string totalPagarText = labelText.Replace("Total a Pagar: ", "").Trim();

                    // Intentar convertir el texto a decimal
                    if (decimal.TryParse(totalPagarText, System.Globalization.NumberStyles.Currency, System.Globalization.CultureInfo.CurrentCulture, out decimal totalAPagar))
                    {
                        // Calcular el saldo después del pago
                        decimal saldo = totalAPagar - montoAPagar;

                        // Mostrar el saldo en labelDev
                        if (saldo >= 0)
                        {
                            labelDev.Text = "Devolución: " + saldo.ToString("C");
                        }
                        else
                        {
                            decimal devolucion = Math.Abs(saldo);
                            labelDev.Text = "Devolución: " + devolucion.ToString("C");
                        }
                    }
                    else
                    {
                        MessageBox.Show("No se pudo convertir el total a pagar a un número decimal.");
                    }
                }
                else
                {
                    MessageBox.Show("El formato de labelTotalPagar no es válido.");
                }
            }
            else
            {
                MessageBox.Show("Ingrese un monto válido en textBox9.");
            }
            decimal subtotal = decimal.Parse(labelSub.Text.Replace("Subtotal: ", "").Trim(), System.Globalization.NumberStyles.Currency);

            //Informacion de Cfactura
            Mysql.Cfactura objetoFactura = new Mysql.Cfactura();

            // Llamar al método para insertar la factura
            objetoFactura.InsertarFactura(dateTimePicker1, textBox2, label20);

            // Llamar al método para insertar el detalle de la factura
            objetoFactura.InsertarDetalleFactura(textBox1, dataGridFactura, subtotal);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Mysql.Cfactura objetoFactura = new Mysql.Cfactura();
            objetoFactura.BuscarClientePorCodigo(textBox2, textBox3);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            Mysql.Cfactura objetoFactura = new Mysql.Cfactura();
            objetoFactura.BuscarProductoPorCodigo(textBox4, textBox5, numericUpDown2);
        }

        private void FormFactura_Load(object sender, EventArgs e)
        {
            numericUpDown3.Enabled = false;
            numericUpDown4.Enabled = false;
        }
    }
}
