using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PruebaTecnicaDIAN.Models;


namespace PruebaTecnicaDIAN.Controllers
{
    public class HomeController : Controller
    {        
        private IWebHostEnvironment _env;
       
        /// <summary>
        /// Metodo para conocer información de la ruta en donde se va almacenar el archivo
        /// </summary>
        /// <param name="env"></param>
        public HomeController(IWebHostEnvironment env)
        {
            _env = env;
        }

       

        public IActionResult Index()
        {   
            return View(); 
        }

        public IActionResult Privacy()
        {
            return View();
        }
        /// <summary>
        /// Accion al Oprimir el boton cargar Archivo
        /// </summary>
        /// <param name="Archivo"></param>
        /// <returns></returns>
        public IActionResult ArchivoSimple(IFormFile Archivo)
        {
            var dir = _env.ContentRootPath;
            List<Pago> pagos = [];
            Mensaje a = new Mensaje();

            //Acciones dependiendo tipo de archivo
            if (Archivo != null)
            {
               
                if (Archivo.ContentType.Equals("application/json"))
                {

                    int total;
                    string json;
                    bool ErrorFecha, ErrorMonto, ErrorTipo;
                    //Guardar localmente el archivo
                    using (var fileStream = new FileStream(Path.Combine(dir, "file.json"), FileMode.Create, FileAccess.Write))
                    {
                        Archivo.CopyTo(fileStream);

                    }
                    dir = dir + "\\file.json";
                    //Convertir a la clase que almacena el json 
                    using (StreamReader r = new StreamReader(dir))
                    {
                        json = r.ReadToEnd();
                        pagos = JsonConvert.DeserializeObject<List<Pago>>(json);
                        total = pagos.Count;
                    }
                    //Validaciones
                    ErrorFecha = ValidarFechaPago(pagos);
                    if (ErrorFecha)
                    {
                        a.Error = "Hay registros con fecha invalida!";
                        DateTime actual = DateTime.Now;
                        var quitar = pagos.SingleOrDefault(x => x.Fecha > actual);
                        pagos.Remove(quitar);
                    }
                    ErrorMonto = ValidarMonto(pagos);
                    if (ErrorMonto)
                    {
                        a.Error = "Hay registros con monto invalido!";
                        var quitar = pagos.SingleOrDefault(x => x.Monto <= 0);
                        pagos.Remove(quitar);
                    }
                    ErrorTipo = ValidarTipoDocumento(pagos);
                    if (ErrorTipo)
                    {
                        a.Error = "Hay registros con Tipo de Documento invalido!";
                        var quitar = pagos.SingleOrDefault(x => x.TipoIdentificacion != "CC" && x.TipoIdentificacion != "CE" && x.TipoIdentificacion != "NIT" && x.TipoIdentificacion != "PEP" && x.TipoIdentificacion != "PA");
                        pagos.Remove(quitar);
                    }
                    if (pagos.Count > 0)
                    {
                        
                        a.Exito = "Se Procesaron " + pagos.Count + " de " + total + " Registros";
                    }
                       

                    return View("~/Views/Home/Index.cshtml", a);
                }
                else if (Archivo.ContentType.Equals("text/xml"))
                {


                    a.Exito = "Se cargaron json";
                    return View("~/Views/Home/Index.cshtml", a);
                }
                else
                {
                    a.Error = "Formato incorrecto!";
                    return View("~/Views/Home/Index.cshtml", a);
                }
            }
            else {
                a.Error = "Debe Seleccionar un archivo!";
                return View("~/Views/Home/Index.cshtml", a);

            }    
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}

        public bool ValidarTipoDocumento(List<Pago> pagos)        {
           
            return pagos.Any(x => x.TipoIdentificacion != "CC" && x.TipoIdentificacion != "CE" && x.TipoIdentificacion != "NIT" && x.TipoIdentificacion != "PEP" && x.TipoIdentificacion != "PA");
        }

        public bool ValidarFechaPago(List<Pago> pagos)
        {
            DateTime actual = DateTime.Now;
            return pagos.Any(x => x.Fecha >= actual);
        }

        public bool ValidarMonto(List<Pago> pagos)
        {
            return pagos.Any(x => x.Monto <= 0);
        }
    }
}
