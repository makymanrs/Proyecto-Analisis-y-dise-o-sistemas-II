using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;

namespace Proyecto.Mysql
{
    internal class Ccredito
    {
        public void mostrarCredito(DataGridView tablaCredito)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                string query = @"SELECT 
                            cre.cre_cod AS 'Credito ID',
                            cre.fac_cod AS 'Factura Codigo',
                            CONCAT(c.cli_pnom, ' ', IFNULL(c.cli_snom, ''), ' ', c.cli_pape, ' ', IFNULL(c.cli_sape, '')) AS 'Nombre Cliente',
                            cre.cre_monto as 'Monto a pagar',
                            f.fac_fec AS 'Fecha Factura',
                            cre.cre_fecpagado as 'Fecha Pagada',   
                            cre.cre_pagado AS 'Pagado'
                         FROM 
                            credito cre
                         JOIN 
                            factura f ON cre.fac_cod = f.fac_cod
                         JOIN 
                            cliente c ON f.cli_id = c.cli_cod";

                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conexion);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                tablaCredito.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se encontraron los datos de la base de datos, error: " + ex.ToString());
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }


    }
}
