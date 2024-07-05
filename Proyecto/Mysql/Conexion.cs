using MySql.Data.MySqlClient;
using MySqlConnector;
using System;
using System.Data;
using System.Windows.Forms;

namespace Proyecto.Mysql
{
    internal class Conexion
    {
        private static string servidor = "localhost";
        private static string bd = "Lafavorita";  // Asegúrate de que este es el nombre correcto de tu base de datos
        private static string usuario = "root";
        private static string password = "200212";
        private static string puerto = "3306";

        private string cadenaConexion = $"server={servidor};port={puerto};user id={usuario};password={password};database={bd};";

        public MySql.Data.MySqlClient.MySqlConnection establecerConexion()
        {
            MySql.Data.MySqlClient.MySqlConnection conexionBD = new MySql.Data.MySqlClient.MySqlConnection(cadenaConexion);
            try
            {
                conexionBD.Open();
                // MessageBox.Show("Conexión establecida correctamente.");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show("Error al establecer la conexión: " + ex.Message);
            }
            return conexionBD;
        }

        public void cerrarConexion(MySql.Data.MySqlClient.MySqlConnection conexionBD)
        {
            if (conexionBD != null && conexionBD.State == ConnectionState.Open)
            {
                conexionBD.Close();
            }
        }
    }
}
