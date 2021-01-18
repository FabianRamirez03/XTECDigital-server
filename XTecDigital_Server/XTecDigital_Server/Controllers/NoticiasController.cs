using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using XTecDigital_Server.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace XTecDigital_Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NoticiasController : ControllerBase   
    {
        private string serverKey = Startup.getKey();

        [Route("crearNoticiaGrupo")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public Object CrearNoticiaGrupo(Noticia noticia)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "crearNoticiaGrupo";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@codigoCurso", noticia.codigoCurso);
            cmd.Parameters.AddWithValue("@numeroGrupo", noticia.numeroGrupo);
            cmd.Parameters.AddWithValue("@tituloNoticia", noticia.tituloNoticia);
            cmd.Parameters.AddWithValue("@mensaje", noticia.mensaje);
            List<Object> respuesta = new List<Object>();
            try
            {
                cmd.ExecuteNonQuery();
                var response = new[]
                    {
                        new
                        {
                            respuesta = "200 OK",
                            error = "null"
                        }

                     };
                respuesta.Add(response);
            }
            catch (Exception e)
            {
                string[] separatingStrings = { "\r" };
                var response = new[]
                    {
                        new
                        {
                            respuesta = "error",
                            error = e.Message.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries)[0]
            }

                     };
                respuesta.Add(response);
            }
            conn.Close();
            return respuesta[0];
        }
    }
}
