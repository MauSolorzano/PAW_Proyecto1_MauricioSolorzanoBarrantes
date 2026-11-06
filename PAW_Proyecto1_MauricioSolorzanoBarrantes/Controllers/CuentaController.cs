using PAW_Proyecto1_MauricioSolorzanoBarrantes.Data;
using PAW_Proyecto1_MauricioSolorzanoBarrantes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PAW_Proyecto1_MauricioSolorzanoBarrantes.Controllers
{
    public class CuentaController : Controller
    {
        
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        //Metodo que maneja la solicitud POST para el inicio de sesión.
        //Valida los datos ingresados, busca el usuario en la base de datos y verifica la contraseña.
        //Si todo es correcto, establece la cookie de autenticación y guarda información del usuario en la sesión.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(contrasena))
            {
                ViewBag.Mensaje = "Ingrese el correo y la contraseña.";
                return View();
            }

            UsuarioData datos = new UsuarioData();
            Usuario usuario = datos.BuscarPorEmail(email.Trim());

            if (usuario == null ||
                !usuario.Estado ||
                !Seguridad.VerificarContrasena(
                    contrasena, usuario.ContrasenaHash))
            {
                ViewBag.Mensaje = "Correo o contraseña incorrectos.";
                return View();
            }

            FormsAuthentication.SetAuthCookie(usuario.Email, false);

            Session["UsuarioId"] = usuario.Id;
            Session["UsuarioNombre"] = usuario.Nombre;
            Session["UsuarioRol"] = usuario.Rol;

            return RedirectToAction("Index", "Home");
        }
        //Metodo que maneja la solicitud GET para cerrar sesión.
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();

            return RedirectToAction("Login");
        }
    }
}