using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace Proyecto.Mysql
{
    internal class Cproveedor
    {
        // asi va la estructura solo seguirla y cambiarla segun el boton asignado aqui en este caso fue proveedor
        public void mostrarProveedor(DataGridView tablaProveedor)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();
                string query = "SELECT prove_id as 'Proveedor Id', prove_nom as 'Nombre', prove_tel as 'Telefono', prove_sc as 'Sector Comercial', prove_gmai as 'Gmail', prove_dir as 'Dirección' " +
                               "FROM proveedor";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conexion);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                tablaProveedor.DataSource = dt;
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

        public void guardarProveedor(TextBox id, TextBox nombre, TextBox telefono, ComboBox sc, TextBox gmail, TextBox direccion)
        {
            // Verificar si alguno de los campos está vacío
            if (string.IsNullOrWhiteSpace(id.Text) || string.IsNullOrWhiteSpace(nombre.Text) ||
                string.IsNullOrWhiteSpace(telefono.Text) || sc.SelectedItem == null ||
                string.IsNullOrWhiteSpace(gmail.Text) || string.IsNullOrWhiteSpace(direccion.Text))
            {
                MessageBox.Show("Todos los campos deben estar llenos.");
                return;
            }

            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Verificar si ya existe el proveedor con el mismo prove_id
                string queryVerificarProveedor = "SELECT COUNT(*) FROM proveedor WHERE prove_id = @proveId";
                MySqlCommand commandVerificarProveedor = new MySqlCommand(queryVerificarProveedor, conexion);
                commandVerificarProveedor.Parameters.AddWithValue("@proveId", id.Text);
                int countProveedor = Convert.ToInt32(commandVerificarProveedor.ExecuteScalar());

                if (countProveedor > 0)
                {
                    MessageBox.Show("Ya existe un proveedor con el mismo código.");
                    return; // Salir del método sin guardar
                }

                // Insertar en tabla proveedor
                string query = "INSERT INTO proveedor(prove_id, prove_nom, prove_tel, prove_sc, prove_gmai, prove_dir) " +
                               "VALUES (@proveId, @proveNom, @proveTel, @proveSc, @proveGmai, @proveDir)";
                MySqlCommand myCommandProveedor = new MySqlCommand(query, conexion);
                myCommandProveedor.Parameters.AddWithValue("@proveId", id.Text);
                myCommandProveedor.Parameters.AddWithValue("@proveNom", nombre.Text);
                myCommandProveedor.Parameters.AddWithValue("@proveTel", telefono.Text);
                myCommandProveedor.Parameters.AddWithValue("@proveSc", sc.SelectedItem.ToString());
                myCommandProveedor.Parameters.AddWithValue("@proveGmai", gmail.Text);
                myCommandProveedor.Parameters.AddWithValue("@proveDir", direccion.Text);
                myCommandProveedor.ExecuteNonQuery();
                MessageBox.Show("Se guardó el registro");
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo guardar el registro: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }
        // busqueda para la hora de editar 
        public DataRow buscarProveedor(int id)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                string query = "SELECT prove_id, prove_nom, prove_tel, prove_sc, prove_gmai, prove_dir FROM proveedor WHERE prove_id = @id";
                MySqlCommand command = new MySqlCommand(query, conexion);
                command.Parameters.AddWithValue("@id", id);

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]; // Devuelve la primera fila encontrada
                }
                else
                {
                    return null; // No se encontraron resultados
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar proveedor: " + ex.Message);
                return null;
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }

        public void modificarProveedor(TextBox id, TextBox nombre, TextBox telefono, ComboBox sc, TextBox gmail, TextBox direccion)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                string query = "UPDATE proveedor SET prove_nom=@nombre, prove_tel=@telefono, prove_sc=@proveSc, prove_gmai=@proveGmail, prove_dir=@direccion WHERE prove_id = @id";
                MySqlCommand command = new MySqlCommand(query, conexion);
                command.Parameters.AddWithValue("@id", id.Text);
                command.Parameters.AddWithValue("@nombre", nombre.Text);
                command.Parameters.AddWithValue("@telefono", telefono.Text);
                command.Parameters.AddWithValue("@proveSc", sc.SelectedItem.ToString());
                command.Parameters.AddWithValue("@proveGmail", gmail.Text);
                command.Parameters.AddWithValue("@direccion", direccion.Text);

                command.ExecuteNonQuery();
                MessageBox.Show("Se modificaron los registros correctamente");
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo modificar el registro: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }
        // este es para el formulario
        public void buscarProveedorPorId(DataGridView tablaProveedor, TextBox id)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Query para buscar proveedores por ID
                string query = "SELECT prove_id as 'Proveedor Id', prove_nom as 'Nombre', prove_tel as 'Telefono', prove_sc as 'Sector Comercial', prove_gmai as 'Gmail', prove_dir as 'Dirección' " +
                               "FROM proveedor " +
                               "WHERE prove_id = @id";

                MySqlCommand command = new MySqlCommand(query, conexion);
                command.Parameters.AddWithValue("@id", id.Text.Trim());

                // Adaptador para llenar los datos en un DataTable
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Limpiar el DataGridView antes de mostrar los resultados
                tablaProveedor.DataSource = null;
                tablaProveedor.Rows.Clear();

                // Asignar el DataTable con los resultados al DataGridView
                tablaProveedor.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar proveedor: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }
        public void eliminarProveedor(TextBox cod, DataGridView tablaProveedor)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Eliminar relación con órdenes
                /*
                string queryOrden = "DELETE FROM orden WHERE prove_id = @proveId";
                MySqlCommand commandOrden = new MySqlCommand(queryOrden, conexion);
                commandOrden.Parameters.AddWithValue("@proveId", cod.Text);
                commandOrden.ExecuteNonQuery();
                */

                // Eliminar el proveedor de la tabla proveedor
                string queryProveedor = "DELETE FROM proveedor WHERE prove_id = @proveId";
                MySqlCommand commandProveedor = new MySqlCommand(queryProveedor, conexion);
                commandProveedor.Parameters.AddWithValue("@proveId", cod.Text);

                int rowsAffected = commandProveedor.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Se eliminó el registro del proveedor y las entradas relacionadas en la tabla 'orden'.");
                }
                else
                {
                    MessageBox.Show("No se encontró ningún proveedor con ese ID.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo eliminar el registro del proveedor y/o las entradas relacionadas en la tabla 'orden'. Error: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }
        public void seleccionarProveedor(DataGridView tablaProveedor, TextBox textboxProveedorId)
        {
            try
            {
                // Asume que la columna del ID de bodega es la primera columna (índice 0)
                textboxProveedorId.Text = tablaProveedor.CurrentRow.Cells[0].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se logró seleccionar, error: " + ex.ToString());
            }
        }

    }
}
