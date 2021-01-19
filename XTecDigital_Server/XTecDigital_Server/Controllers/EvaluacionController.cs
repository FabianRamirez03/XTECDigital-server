using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XTecDigital_Server.Models;

namespace XTecDigital_Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EvaluacionController : Controller
    {
        // Llave para acceder al servidor 
        private string serverKey = Startup.getKey();

        //Crea una nueva evaluacion para un determinado grupo
        [Route("crearEvaluacion")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object crearEvaluacion(Evaluacion evaluacion)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "crearEvaluacion";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@grupal", evaluacion.grupal);
            cmd.Parameters.AddWithValue("@nombre", evaluacion.nombre);
            cmd.Parameters.AddWithValue("@porcentaje", evaluacion.porcentaje);
            cmd.Parameters.AddWithValue("@fechaInicio", evaluacion.fechaInicio);
            cmd.Parameters.AddWithValue("@fechaFin", evaluacion.fechaFin);
            cmd.Parameters.AddWithValue("@archivo", evaluacion.archivo);
            cmd.Parameters.AddWithValue("@rubro", evaluacion.rubro);
            cmd.Parameters.AddWithValue("@codigoCurso", evaluacion.codigoCurso);
            cmd.Parameters.AddWithValue("@numeroGrupo", evaluacion.numeroGrupo);
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

        // Controller para eliminar una evaluacion de un determinado grupo
        [Route("eliminarEvaluacion")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object eliminarEvaluacion(Evaluacion evaluacion)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "eliminarEvaluacion";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@nombre", evaluacion.nombre);
            cmd.Parameters.AddWithValue("@rubro", evaluacion.rubro);
            cmd.Parameters.AddWithValue("@codigoCurso", evaluacion.codigoCurso);
            cmd.Parameters.AddWithValue("@numeroGrupo", evaluacion.numeroGrupo);
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


        // Controller para ver todos los semestres existentes en la base de datos
        [Route("verNotasEstudianteGrupo")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public List<Object> verNotasEstudianteGrupo(Evaluacion evaluacion   )
        {
            List<Object> semestres = new List<Object>();
            //Connect to database
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "verNotasEstudianteGrupo";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@carnet", evaluacion.carnet);
            cmd.Parameters.AddWithValue("@codigoCurso ", evaluacion.codigoCurso);
            cmd.Parameters.AddWithValue("@numeroGrupo ", evaluacion.numeroGrupo);
            try
            {  
                SqlDataReader dr = cmd.ExecuteReader();
                var response = new[]
                    {
                        new
                        {
                            respuesta = "200 OK",
                            error = "null"
                        }

                     };
                semestres.Add(response);
                while (dr.Read())
                {
                    var jsons = new[]
                    {
                        new {
                            nombreEvaluacion = dr[0].ToString(),
                            rubro = dr[1].ToString(),
                            notaObtenida = dr[2].ToString(),
                            porcentajeObtenido = dr[3],
                            porcentajeEvaluacion = dr[4],
                            notaFinalRubro = dr[5],
                            notaFinal = dr[6],
                        }

                     };
                    Console.WriteLine(jsons);
                    semestres.Add(jsons);
                }

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
                semestres.Add(response);

            }

            List<object> retornar = new List<object>();
            for (var x = 0; x < semestres.Count; x++)
            {
                var tempList = (IList<object>)semestres[x];
                retornar.Add(tempList[0]);
            }
            conn.Close();
            return retornar;

        }



        // Controller para descargar un archivo de la base de datos
        [Route("obtenerArchivoSolucion")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public Object obtenerArchivoSolucion(Evaluacion evaluacion)
        {
            try
            {
                SqlConnection conn = new SqlConnection(serverKey);
                conn.Open();
                SqlCommand cmd;
                string insertQuery = "obtenerArchivoSolucion";
                cmd = new SqlCommand(insertQuery, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@carnet", evaluacion.carnet);
                cmd.Parameters.AddWithValue("@codigoCurso", evaluacion.codigoCurso);
                cmd.Parameters.AddWithValue("@numeroGrupo", evaluacion.numeroGrupo);
                cmd.Parameters.AddWithValue("@rubro", evaluacion.rubro);
                cmd.Parameters.AddWithValue("@nombreEvaluacion", evaluacion.nombreEvaluacion);
                SqlDataReader dr = cmd.ExecuteReader();
                var archivo = "";
                var nombre = "";
                while (dr.Read())
                {
                    archivo = dr[0].ToString();
                    nombre = dr[1].ToString();
                }
                var imageStream = new MemoryStream();
                // var bytes = Encoding.ASCII.GetBytes(defaultAvatarAsBase64);
                var bytes = Convert.FromBase64String(archivo);
                imageStream = new MemoryStream(bytes);

                var result = new FileStreamResult(imageStream, "APPLICATION/octet-stream");

                result.FileDownloadName = nombre;
                return result;
            }
            catch (Exception e)
            {
                return e;
            }
        }



        // Controller para ver todos los semestres existentes en la base de datos
        [Route("verEvaluacionesRubro")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public List<Object> verEvaluacionesRubro(Evaluacion evaluacion)
        {
            List<Object> semestres = new List<Object>();
            //Connect to database
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "verEvaluacionesRubro";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@codigoCurso ", evaluacion.codigoCurso);
            cmd.Parameters.AddWithValue("@numeroGrupo ", evaluacion.numeroGrupo);
            cmd.Parameters.AddWithValue("@rubro", evaluacion.rubro);
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                var response = new[]
                    {
                        new
                        {
                            respuesta = "200 OK",
                            error = "null"
                        }

                     };
                semestres.Add(response);
                while (dr.Read())
                {
                    var jsons = new[]
                    {
                        new {
                            nombre = dr[0].ToString(),
                            fechaFin = dr[1].ToString(),
                            porcentaje = dr[2],
                            archivo = dr[3],
                            cantPersonas = dr[4],
                        }

                     };
                    Console.WriteLine(jsons);
                    semestres.Add(jsons);
                }

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
                semestres.Add(response);

            }

            List<object> retornar = new List<object>();
            for (var x = 0; x < semestres.Count; x++)
            {
                var tempList = (IList<object>)semestres[x];
                retornar.Add(tempList[0]);
            }
            conn.Close();
            return retornar;

        }

    }
}
