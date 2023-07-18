using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using EjercicioMVC.Models;


namespace TestLoginEmpleado.Controllers
{
    public class AccesoController : Controller
    {

       

        static string cadena = "Data Source=DESKTOP-U4J8EC0;Initial Catalog=Ejercicio;Integrated Security=true";  //cadena local



        // GET: Acceso
        public ActionResult Login()
        {
            return View();
        }


        public ActionResult Registrar()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Registrar(USUARIOS oUsuario)
        {
            bool registrado;
            string mensaje;


            // Se valida que las dos contraseñas sean Iguales
            if (oUsuario.Clave == oUsuario.ConfirmarClave)
            {
                // Convierto en sha256 la clave del usuario , asi nos e ve en al base de datos.
                oUsuario.Clave = ConvertirSha256(oUsuario.Clave);
            }
            else
            {
                ViewData["Mensaje"] = "Las Conrtaseñas introducidas no son iguales, intente nuevamente";
                return View();
            }

            // uso la cadena de conexion  con sql server
            using (SqlConnection cn = new SqlConnection(cadena))
            {

                SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", cn); //Uso el comando cdm  y paso  por parametros  el procedimiento almacenado  y cn (donde guarde la cadena de conexion)
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);  //@Correo del procedimiento y de la tabla Usuario.
                cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);

                // Parametros de salida
                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                cmd.ExecuteNonQuery();

                registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                mensaje = cmd.Parameters["Mensaje"].Value.ToString();


            }

            ViewData["Mensaje"] = mensaje;

            if (registrado)
            {
                return RedirectToAction("Login", "Acceso");
            }
            else
            {
                return View();
            }

        }

        [HttpPost]
        public ActionResult Login(USUARIOS oUsuario)
        {
            oUsuario.Clave = ConvertirSha256(oUsuario.Clave);

            using (SqlConnection cn = new SqlConnection(cadena))
            {

                SqlCommand cmd = new SqlCommand("sp_ValidadUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                oUsuario.IdUsuario = Convert.ToInt32(cmd.ExecuteScalar().ToString());

            }

            if (oUsuario.IdUsuario != 0)
            {

                Session["usuario"] = oUsuario;
                return RedirectToAction("Index", "Empleado"); //redirecciono al Metodo  y luego al controlador si las credenciales estan correctas
            }
            else
            {
                ViewData["Mensaje"] = "Usuario no encontrado";
                return View();
            }


        }



        public static string ConvertirSha256(string texto)
        {
            //using System.Text;
            //USAR LA REFERENCIA DE "System.Security.Cryptography" para la conversion de sha259 y que no se pueda
            //acceder en base de datos  a  la contraseña real del usuario.

            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }



    }
}