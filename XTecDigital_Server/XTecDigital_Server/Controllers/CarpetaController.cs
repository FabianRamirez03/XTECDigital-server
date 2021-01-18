using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using XTecDigital_Server.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;

namespace XTecDigital_Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CarpetaController : Controller
    {
        
        // String para ingresar a la base de datos
        private string serverKey = Startup.getKey();


        // Controller para crear nuevas carpetas en la gestion de documentos
        [Route("crearCarpeta")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public Object crearCarpeta(Carpeta semestre)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "crearCarpeta";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@nombre", semestre.nombre);
            cmd.Parameters.AddWithValue("@codigoCurso", semestre.codigoCurso);
            cmd.Parameters.AddWithValue("@numeroGrupo", semestre.numeroGrupo);
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


        // Controller para Eliminar carpetas en la gestion de documentos
        [Route("eliminarCarpeta")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object eliminarCarpeta(Carpeta semestre)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "eliminarCarpeta";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@nombre", semestre.nombre);
            cmd.Parameters.AddWithValue("@codigoCurso", semestre.codigoCurso);
            cmd.Parameters.AddWithValue("@numeroGrupo", semestre.numeroGrupo);
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
