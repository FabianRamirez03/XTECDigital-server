using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using XTecDigital_Server.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using MongoDB.Driver;
using MongoDB.Bson;

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
            cmd.Parameters.AddWithValue("@cedulaAutor", noticia.cedulaAutor);
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

        [Route("eliminarNoticia")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public Object eliminarNoticia(Noticia noticia)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "eliminarNoticia";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@codigoCurso", noticia.codigoCurso);
            cmd.Parameters.AddWithValue("@numeroGrupo", noticia.numeroGrupo);
            cmd.Parameters.AddWithValue("@tituloNoticia", noticia.tituloNoticia);
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




        [Route("editarNoticiaGrupo")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public Object editarNoticiaGrupo(Noticia noticia)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "editarNoticiaGrupo";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@codigoCurso", noticia.codigoCurso);
            cmd.Parameters.AddWithValue("@numeroGrupo", noticia.numeroGrupo);
            cmd.Parameters.AddWithValue("@tituloNoticia", noticia.tituloViejo);
            cmd.Parameters.AddWithValue("@tituloNuevo", noticia.tituloNoticia);
            cmd.Parameters.AddWithValue("@mensajeNuevo", noticia.mensaje);
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



        [Route("verNoticiasProfesor")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public List<Object> verNoticiasProfesor(Noticia noticia)
        {
            List<Object> semestres = new List<Object>();
            //Connect to database
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "verNoticiasProfesor";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@cedula", noticia.cedulaAutor);
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
                            titulo = dr[0].ToString(),
                            mensaje = dr[1].ToString(),
                            fecha = dr[2].ToString(),
                            nombre = dr[3],
                            numeroGrupo = dr[4],
                            codigoCurso = dr[5].ToString()
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


        [Route("verTodasNoticiasEstudiante")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public List<Object> verTodasNoticiasEstudiante(Noticia noticia)
        {
            List<Object> semestres = new List<Object>();
            //Connect to database
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "verTodasNoticiasEstudiante";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@carnet", noticia.carnet);
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
                    var connectionString = "mongodb+srv://admin:admin@usuarios.ozlkz.mongodb.net/Usuarios?retryWrites=true&w=majority";
                    var mongoClient = new MongoClient(connectionString);
                    var dataBase = mongoClient.GetDatabase("Usuarios");
                    var collection = dataBase.GetCollection<BsonDocument>("profesores");
                    var filter = Builders<BsonDocument>.Filter.Eq("cedula", dr[3].ToString());
                    var document = collection.Find(filter).FirstOrDefault();
                    var jsons = new[]
                    {
                        new {
                            titulo = dr[0].ToString(),
                            mensaje = dr[1].ToString(),
                            fecha = dr[2].ToString(),
                            cedulaAutor = dr[3].ToString(),
                            nombre = dr[4],
                            codigoCurso = dr[5].ToString(),
                            numeroGrupo = dr[6],
                            nombreProfesor = document.GetValue("nombre").AsString + " " + document.GetValue("apellido").AsString
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
