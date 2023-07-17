using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EjercicioMVC.Permisos
{
    public class ValidarSesionAttribute : ActionFilterAttribute
    {


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["usuario"] == null)  // si la session es nula  redirecciono  al Controlador Accesso/Metodo Login en este caso luego pongo en el home el using del proyecto y [ValidarSesion] (se omite el Attribute))
            {

                filterContext.Result = new RedirectResult("~/Acceso/Login");
            }

            base.OnActionExecuting(filterContext);
        }



    }
}