using EjercicioMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using EjercicioMVC.Permisos;
using Rotativa;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;



namespace EjercicioMVC.Controllers
{
    public class EmpleadoController : Controller
    {
        [ValidarSesion]   // ejecuto ValidarSesion y compruebo si la sesion está iniciada , y lo valido.Si es afirmativo sigo en las acciones, sino redirecciono al login
        // GET: Empleado
        public ActionResult Index(string Buscar)
        {
            List<EmpleadCLS> listaEmpleados = null;

            if (!String.IsNullOrEmpty(Buscar)) {

           
                using (var db = new EjercicioEntities2())
                {
                    listaEmpleados = (from empleado in db.Empleado     // aca Llamo al nombre del modelo Empleado (la tabla)
                                      where empleado.HABILITADO == 1 && empleado.NOMBRE == Buscar
                                      select new EmpleadCLS
                                      {
                                          IDEMPLEADO = empleado.IDEMPLEADO,
                                          NOMBRE = empleado.NOMBRE,
                                          APELLIDO = empleado.APELLIDO,
                                          FECHACONTRATO = empleado.FECHACONTRATO,
                                          SUELDO = empleado.SUELDO,
                                          IDSEXO = (int)empleado.IDSEXO
                                      }).ToList();

                }

            } else {
                using (var db = new EjercicioEntities2())
                {
                    listaEmpleados = (from empleado in db.Empleado     // aca Llamo al nombre del modelo Empleado (la tabla)
                                      where empleado.HABILITADO == 1
                                      select new EmpleadCLS
                                      {
                                          IDEMPLEADO = empleado.IDEMPLEADO,
                                          NOMBRE = empleado.NOMBRE,
                                          APELLIDO = empleado.APELLIDO,
                                          FECHACONTRATO = empleado.FECHACONTRATO,
                                          SUELDO = empleado.SUELDO,
                                          IDSEXO = (int)empleado.IDSEXO
                                      }).ToList();

                }

             }
       

            return View(listaEmpleados);
        }





        /*Variable Global*/
        List<SelectListItem> listaSexo; //Variable Global

       // IEnumerable<SelectListItem> listaSexo;

        private void llenarSexo() 
        {
            using (var db = new EjercicioEntities2())
            {
                listaSexo = (from sexo in db.Sexo_Empleado
                             where sexo.HABILITADO == 1
                             select new SelectListItem
                             {
                                 Text = sexo.SEXO,
                                 Value = sexo.ID_SEXO.ToString()
                             }).ToList();


                listaSexo.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" } );
            }  
           
        
        }
        public ActionResult HabilitarEmpleado(int id)
        {
            try
            {
                using (var db = new EjercicioEntities2())
                {
                    Empleado empleado = db.Empleado.FirstOrDefault(e => e.IDEMPLEADO == id);

                    if (empleado != null)
                    {
                        empleado.HABILITADO = 1;
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al habilitar el empleado: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        

        public ActionResult VerTodos()
        {
            List<EmpleadCLS> listaEmpleados = null;

            using (var db = new EjercicioEntities2())
            {
                listaEmpleados = (from empleado in db.Empleado
                                  select new EmpleadCLS
                                  {
                                      IDEMPLEADO = empleado.IDEMPLEADO,
                                      NOMBRE = empleado.NOMBRE,
                                      APELLIDO = empleado.APELLIDO,
                                      FECHACONTRATO = empleado.FECHACONTRATO,
                                      SUELDO = empleado.SUELDO,
                                      IDSEXO = (int)empleado.IDSEXO
                                  }).ToList();
            }

            return View("Index", listaEmpleados);
           // return RedirectToAction("Index");
        }

        public ActionResult VerHabilitados()
        {
            List<EmpleadCLS> listaEmpleados = null;

            using (var db = new EjercicioEntities2())
            {
                listaEmpleados = (from empleado in db.Empleado     // aca Llamo al nombre del modelo Empleado (la tabla)
                                  where empleado.HABILITADO == 1
                                  select new EmpleadCLS
                                  {
                                      IDEMPLEADO = empleado.IDEMPLEADO,
                                      NOMBRE = empleado.NOMBRE,
                                      APELLIDO = empleado.APELLIDO,
                                      FECHACONTRATO = empleado.FECHACONTRATO,
                                      SUELDO = empleado.SUELDO,
                                      IDSEXO = (int)empleado.IDSEXO
                                  }).ToList();
            }

           // return View("Index", listaEmpleados);
            return RedirectToAction("Index");
        }







        [HttpGet]
        public ActionResult Agregar()
        {
            llenarSexo();
            ViewBag.lista = listaSexo;
            EmpleadCLS oEmpleadoCLS = new EmpleadCLS();
            return View(oEmpleadoCLS);
        }

        [HttpPost]
        public ActionResult Agregar(EmpleadCLS oEmpleadoCLS)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    llenarSexo();
                    ViewBag.lista = listaSexo;
                    return View(oEmpleadoCLS);
                }
                else
                {
                    using (var db = new EjercicioEntities2())
                    {
                        // Verificar si ya existe un empleado con el mismo nombre y apellido
                        bool empleadoExistente = db.Empleado.Any(e => e.NOMBRE == oEmpleadoCLS.NOMBRE && e.APELLIDO == oEmpleadoCLS.APELLIDO);

                        if (empleadoExistente)
                        {
                            ViewBag.ErrorMessage = "Ya existe un empleado con el mismo nombre y apellido.";
                            llenarSexo();
                            ViewBag.lista = listaSexo;
                            return View(oEmpleadoCLS);
                        }

                        Empleado oEmpleado = new Empleado();
                        oEmpleado.NOMBRE = oEmpleadoCLS.NOMBRE;
                        oEmpleado.APELLIDO = oEmpleadoCLS.APELLIDO;
                        oEmpleado.FECHACONTRATO = oEmpleadoCLS.FECHACONTRATO;
                        oEmpleado.SUELDO = oEmpleadoCLS.SUELDO;
                        oEmpleado.IDSEXO = oEmpleadoCLS.IDSEXO;
                        oEmpleado.HABILITADO = 1;

                        db.Empleado.Add(oEmpleado);
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //Controlar error
                ViewBag.ErrorMessage = "Error al agregar el empleado: " + ex.Message;
                llenarSexo();
                ViewBag.lista = listaSexo;
                return View(oEmpleadoCLS);
            }
        }


        [HttpGet]
        public ActionResult Modificar(int id)
        {
            llenarSexo();

            using (var db = new EjercicioEntities2())
            {
                Empleado oEmpleado = db.Empleado.Find(id);

                if (oEmpleado == null)
                {
                    return RedirectToAction("Index");
                }

                EmpleadCLS oEmpleadoCLS = new EmpleadCLS
                {
                    IDEMPLEADO = oEmpleado.IDEMPLEADO,
                    NOMBRE = oEmpleado.NOMBRE,
                    APELLIDO = oEmpleado.APELLIDO,
                    FECHACONTRATO = oEmpleado.FECHACONTRATO,
                    SUELDO = oEmpleado.SUELDO,
                    IDSEXO = (int)oEmpleado.IDSEXO
                };

                ViewBag.lista = listaSexo;
                return View(oEmpleadoCLS);
            }
        }



        [HttpPost]
        public ActionResult Modificar(EmpleadCLS oEmpleadoCLS)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    llenarSexo();
                    ViewBag.lista = listaSexo;
                    return View(oEmpleadoCLS);
                }
                else
                {
                    using (var db = new EjercicioEntities2())
                    {
                        Empleado oEmpleado = db.Empleado.Find(oEmpleadoCLS.IDEMPLEADO);

                        if (oEmpleado == null)
                        {
                            return RedirectToAction("Index");
                        }

                        oEmpleado.NOMBRE = oEmpleadoCLS.NOMBRE;
                        oEmpleado.APELLIDO = oEmpleadoCLS.APELLIDO;
                        oEmpleado.FECHACONTRATO = oEmpleadoCLS.FECHACONTRATO;
                        oEmpleado.SUELDO = oEmpleadoCLS.SUELDO;

                        // Obtener el objeto Sexo_Empleado
                        Sexo_Empleado sexoEmpleado = db.Sexo_Empleado.Find(oEmpleadoCLS.IDSEXO);
                        if (sexoEmpleado != null)
                        {
                            oEmpleado.IDSEXO = sexoEmpleado.ID_SEXO;
                        }

                        db.Entry(oEmpleado).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //Controlar Error
                ViewBag.ErrorMessage = "Error al modificar el empleado: " + ex.Message;
                return View(oEmpleadoCLS);
            }
        }


        public ActionResult Eliminar(int id)
        {
            try
            {
                using (var db = new EjercicioEntities2())
                {
                    Empleado empleado = db.Empleado.FirstOrDefault(e => e.IDEMPLEADO == id);

                    if (empleado != null)
                    {
                        empleado.HABILITADO = 0;
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Manejar el error aquí, por ejemplo, mostrar un mensaje de error en la vista
                ViewBag.ErrorMessage = "Error al eliminar el empleado: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

   



        // Metodo para cerrar sesion  vuelvo nulo Session["usuario"] , redirijo a  la pag login en el controlador de Acceso
        public ActionResult CerrarSesion()
        {
            Session["usuario"] = null;
            return RedirectToAction("Login", "Acceso");
        }


        public ActionResult PrintPDF(string Buscar)
        {
            List<EmpleadCLS> listaEmpleados = null;

            if (!String.IsNullOrEmpty(Buscar))
            {
                using (var db = new EjercicioEntities2())
                {
                    listaEmpleados = (from empleado in db.Empleado
                                      where empleado.HABILITADO == 1 && empleado.NOMBRE == Buscar
                                      select new EmpleadCLS
                                      {
                                          IDEMPLEADO = empleado.IDEMPLEADO,
                                          NOMBRE = empleado.NOMBRE,
                                          APELLIDO = empleado.APELLIDO,
                                          FECHACONTRATO = empleado.FECHACONTRATO,
                                          SUELDO = empleado.SUELDO,
                                          IDSEXO = (int)empleado.IDSEXO
                                      }).ToList();
                }
            }
            else
            {
                using (var db = new EjercicioEntities2())
                {
                    listaEmpleados = (from empleado in db.Empleado
                                      where empleado.HABILITADO == 1
                                      select new EmpleadCLS
                                      {
                                          IDEMPLEADO = empleado.IDEMPLEADO,
                                          NOMBRE = empleado.NOMBRE,
                                          APELLIDO = empleado.APELLIDO,
                                          FECHACONTRATO = empleado.FECHACONTRATO,
                                          SUELDO = empleado.SUELDO,
                                          IDSEXO = (int)empleado.IDSEXO
                                      }).ToList();
                }
            }

            return new PartialViewAsPdf("VistaListaEmpleados", listaEmpleados)
            {
                FileName = "ListaEmpleados.pdf"
            };
        }

        public ActionResult ExportarAExcel()
        {
            List<EmpleadCLS> listaEmpleados = null;

            using (var db = new EjercicioEntities2())
            {
                listaEmpleados = (from empleado in db.Empleado
                                  where empleado.HABILITADO == 1
                                  select new EmpleadCLS
                                  {
                                      IDEMPLEADO = empleado.IDEMPLEADO,
                                      NOMBRE = empleado.NOMBRE,
                                      APELLIDO = empleado.APELLIDO,
                                      FECHACONTRATO = empleado.FECHACONTRATO,
                                      SUELDO = empleado.SUELDO,
                                      IDSEXO = (int)empleado.IDSEXO
                                  }).ToList();
            }

            byte[] fileContentenido;     //variable para almacenar el archivo exel

            using (var package = new ExcelPackage())
            { 
                // nueva hoja de calculo worksheet 
                var excelWorksheet = package.Workbook.Worksheets.Add("Empleados");

                // Definir el encabezado de las celdas 
                excelWorksheet.Cells[1, 1].Value = "ID";
                excelWorksheet.Cells[1, 2].Value = "Nombre";
                excelWorksheet.Cells[1, 3].Value = "Apellido";
                excelWorksheet.Cells[1, 4].Value = "Fecha de Contrato";
                excelWorksheet.Cells[1, 5].Value = "Sueldo";
                excelWorksheet.Cells[1, 6].Value = "Sexo";

                // Establecer estilo de encabezado
                using (var range = excelWorksheet.Cells[1, 1, 1, 6])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.AntiqueWhite);
                }

                // Agregar los datos de los empleados
                int row = 2;
                foreach (var empleado in listaEmpleados)
                {
                    excelWorksheet.Cells[row, 1].Value = empleado.IDEMPLEADO;
                    excelWorksheet.Cells[row, 2].Value = empleado.NOMBRE;
                    excelWorksheet.Cells[row, 3].Value = empleado.APELLIDO;
                    excelWorksheet.Cells[row, 4].Value = empleado.FECHACONTRATO.ToString();
                    excelWorksheet.Cells[row, 5].Value = empleado.SUELDO;
                    excelWorksheet.Cells[row, 6].Value = empleado.IDSEXO;

                    row++;
                }

                // Autoajustar el ancho de las columnas
                excelWorksheet.Cells.AutoFitColumns();

                // Convertir el paquete a un arreglo de bytes
                fileContentenido = package.GetAsByteArray();
            }

            // Devolver el archivo Excel para su descarga
            return File(fileContentenido, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Empleados.xlsx");    // estrucrura File (nombreArchivodebytes,tipoMIME,NombredelArchivo)

            /*Para saber: El tipo MIME (Multipurpose Internet Mail Extensions) una convención para identificar los tipos de contenido de los archivos que se
            transmiten a través de Internet. Es un estándar que define un formato de etiqueta para los tipos de datos y proporciona un mecanismo para asociar
            extensiones de archivo con tipos de contenido específicos.se utiliza para identificar un archivo de Excel en formato OpenXML. 
            Al especificar este tipo MIME en la respuesta HTTP, el cliente reconoce que se trata de un archivo de Excel y puede realizar las acciones correspondientes,
            como descargarlo o abrirlo con una aplicación compatible con Excel.*/
        }



    }



}