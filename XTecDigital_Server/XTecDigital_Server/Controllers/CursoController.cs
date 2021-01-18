using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using XTecDigital_Server.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace XTecDigital_Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CursoController : ControllerBase
    {
        private string serverKey = Startup.getKey();


        // Controller para crear un nuevo curso a la base de datos
        [Route("crearCurso")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public Object crearCurso(Curso curso)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "crearCurso";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Codigo", curso.codigo);
            cmd.Parameters.AddWithValue("@nombre", curso.nombre);
            cmd.Parameters.AddWithValue("@carrera", curso.carrera);
            cmd.Parameters.AddWithValue("@creditos", curso.creditos);
            cmd.Parameters.AddWithValue("@habilitado", curso.habilitado);
            cmd.Parameters.AddWithValue("@cedulaAdmin", curso.cedulaAdmin);
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


        // Controller para eliminar un curso de la base de datos
        [Route("eliminarCurso")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object eliminarCurso(Curso curso)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "eliminarCurso";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Codigo", curso.codigo);
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

        // Controller para acceder a todos los cursos, habilitados o no, almacenados en la base de datos
        [Route("verCursos")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public List<Object> verCursos()
        {
            List<Object> cursos = new List<Object>();
            Curso usuarioCarrera = new Curso();
            //Connect to database
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "verCursos";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader    dr = cmd.ExecuteReader();
            try
            {
                var response = new[]
                    {
                        new
                        {
                            respuesta = "200 OK",
                            error = "null"
                        }

                     };
                cursos.Add(response);
                while (dr.Read())
                {
                    var creditos = 0;
                    var carrera = "N/A";
                    try
                    {
                        creditos = (int)dr[3];
                        carrera = dr[2].ToString();
                    }
                    catch
                    {
                        creditos = 0;
                        carrera = "N/A";
                    }
                    var jsons = new[]
                    {
                        new {
                            codigo = dr[0].ToString(),
                            nombre = dr[1].ToString(),
                            carrera = carrera,
                            creditos = creditos,
                            habilitado = dr[4],
                            idAdministrador = dr[5],
                        }

                     };
                    Console.WriteLine(jsons);
                    cursos.Add(jsons);
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
                cursos.Add(response);

            }

            List<object> retornar = new List<object>();
            for (var x = 0; x < cursos.Count; x++)
            {
                var tempList = (IList<object>)cursos[x];
                retornar.Add(tempList[0]);
            }
            conn.Close();
            return retornar;

        }


        // Controller para ver todos los cursos que estan habilitados en la base de datos
        [Route("verCursosDisponibles")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public List<Object> verCursosDisponibles()
        {
            List<Object> cursos = new List<Object>();
            Curso usuarioCarrera = new Curso();
            //Connect to database
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "verCursosDisponibles";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader dr = cmd.ExecuteReader();
            try
            {
                var response = new[]
                    {
                        new
                        {
                            respuesta = "200 OK",
                            error = "null"
                        }

                     };
                cursos.Add(response);
                while (dr.Read())
                {
                    var creditos = 0;
                    var carrera = "N/A";
                    try
                    {
                        creditos = (int)dr[3];
                        carrera = dr[2].ToString();
                    }
                    catch
                    {
                        creditos = 0;
                        carrera = "N/A";
                    }
                    var jsons = new[]
                    {
                        new {
                            codigo = dr[0].ToString(),
                            nombre = dr[1].ToString(),
                            carrera = carrera,
                            creditos = creditos,
                            habilitado = dr[4],
                            idAdministrador = dr[5],
                        }

                     };
                    Console.WriteLine(jsons);
                    cursos.Add(jsons);
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
                cursos.Add(response);

            }

            List<object> retornar = new List<object>();
            for (var x = 0; x < cursos.Count; x++)
            {
                var tempList = (IList<object>)cursos[x];
                retornar.Add(tempList[0]);
            }
            conn.Close();
            return retornar;

        }

        // Controller para deshabilitar o habilitar un curso de la base de datos. Al ser boolean, lo cambia a su opuesto
        [Route("habilitar_deshabilitarCurso")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object habilitar_deshabilitarCurso(Curso curso)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "habilitar_deshabilitarCurso";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@codigo", curso.codigo);
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

        // Controller para agregar un nuevo curso, con su numero de grupo y codigo de curso a un semestre determinado
        [Route("agregarCursoSemestre")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object agregarCursoSemestre(Curso curso)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "agregarCursoSemestre";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@codigoCurso", curso.codigoCurso);
            cmd.Parameters.AddWithValue("@anoSemestre", curso.ano);
            cmd.Parameters.AddWithValue("@periodoSemestre", curso.periodo);
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
