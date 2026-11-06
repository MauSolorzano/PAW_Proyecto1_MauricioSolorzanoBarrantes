using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PAW_Proyecto1_MauricioSolorzanoBarrantes.Filtros
{
    public class Autenticacion : AuthorizeAttribute
    {
        //Variable para almacenar el rol permitido para acceder a la acción o controlador.
        public string RolPermitido { get; set; }
        //Método que se ejecuta para determinar si el usuario está autorizado a acceder a la acción o controlador.
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Verifica si el usuario está autenticado. Si no lo está, retorna false.
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }
            // Obtiene el rol del usuario desde la sesión.
            string rol = httpContext.Session["UsuarioRol"] as string;

            // Verifica si el rol del usuario es nulo o vacío. Si lo es, retorna false.
            if (string.IsNullOrEmpty(rol))
            {
                return false;
            }
            // Verifica si el rol del usuario coincide con el rol permitido. Si no coincide, retorna false.
            if (string.IsNullOrEmpty(RolPermitido))
            {
                return true;
            }
            // Retorna true si el rol del usuario coincide con el rol permitido, de lo contrario retorna false.
            return rol == RolPermitido;
        }
        // Método que se ejecuta cuando el usuario no está autorizado a acceder a la acción o controlador.
        protected override void HandleUnauthorizedRequest(
            AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("/Cuenta/Login");
            }
            else
            {
                filterContext.Result = new HttpStatusCodeResult(403);
            }
        }
    }
}