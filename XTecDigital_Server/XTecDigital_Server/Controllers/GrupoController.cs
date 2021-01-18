using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using XTecDigital_Server.Models;

namespace XTecDigital_Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GrupoController : Controller
    {
        // Llave para acceder a la base de datos
        private string serverKey = Startup.getKey();


        // Crea un nuevo grupo de un determinado curso en la base de datos
        [Route("crearGrupo")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object crearGrupo(Grupo grupo)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "crearGrupo";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@codigoCurso", grupo.codigoCurso);
            cmd.Parameters.AddWithValue("@numeroGrupo", grupo.numeroGrupo);
            cmd.Parameters.AddWithValue("@ano", grupo.ano);
            cmd.Parameters.AddWithValue("@periodo", grupo.periodo);
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


        // Controller para eliminar un determinado grupo de la base de datos
        [Route("eliminarGrupo")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public void eliminarGrupo(Evaluacion evaluacion)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "eliminarGrupo";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@nombre", evaluacion.nombre);
            cmd.Parameters.AddWithValue("@rubro", evaluacion.rubro);
            cmd.Parameters.AddWithValue("@codigoCurso", evaluacion.codigoCurso);
            cmd.ExecuteNonQuery();
            conn.Close();
        }


        // Controller para asignar un determinado profesor a un grupo específico
        [Route("asignarProfesorGrupo")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object asignarProfesorGrupo(Grupo grupo)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "asignarProfesorGrupo";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@codigoCurso", grupo.codigoCurso);
            cmd.Parameters.AddWithValue("@numeroGrupo", grupo.numeroGrupo);
            cmd.Parameters.AddWithValue("@cedulaProfesor", grupo.cedulaProfesor);
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



        // Controller para eliminar un profesor determinado de un curso específico
        [Route("eliminarProfesorGrupo")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object eliminarProfesorGrupo(Grupo grupo)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "eliminarProfesorGrupo";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@codigoCurso", grupo.codigoCurso);
            cmd.Parameters.AddWithValue("@numeroGrupo", grupo.numeroGrupo);
            cmd.Parameters.AddWithValue("@cedulaProfesor", grupo.cedulaProfesor);
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



        // Controller para agregar nuevos estudiantes a un grupo determinado
        [Route("agregarEstudiantesGrupo")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object agregarEstudiantesGrupo(Grupo grupo)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "agregarEstudiantesGrupo";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@carnet", grupo.carnet);
            cmd.Parameters.AddWithValue("@codigoCurso", grupo.codigoCurso);
            cmd.Parameters.AddWithValue("@numeroGrupo", grupo.numeroGrupo);
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


        // Controller para ver todos los estudiantes inscritos en un grupo de un curso específico
        [Route("verEstudiantesGrupo")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public List<Object> verEstudiantesGrupo(Grupo grupo)
        {
            List<Object> estudiantes = new List<Object>();
            Curso usuarioCarrera = new Curso();
            //Connect to database
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "verEstudiantesGrupo";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ano", grupo.ano);
            cmd.Parameters.AddWithValue("@periodo", grupo.periodo);
            cmd.Parameters.AddWithValue("@grupo", grupo.grupo);
            cmd.Parameters.AddWithValue("@codigoCurso", grupo.codigoCurso);
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
                estudiantes.Add(response);
                while (dr.Read())
                {
                    var connectionString = "mongodb+srv://admin:admin@usuarios.ozlkz.mongodb.net/Usuarios?retryWrites=true&w=majority";
                    var mongoClient = new MongoClient(connectionString);
                    var dataBase = mongoClient.GetDatabase("Usuarios");
                    var collection = dataBase.GetCollection<BsonDocument>("estudiantes");
                    var filter1 = Builders<BsonDocument>.Filter.Eq("carnet", dr[0].ToString());
                    var projection = Builders<BsonDocument>.Projection.Exclude("_id");
                    var document = collection.Find(filter1).Project(projection).FirstOrDefault();
                    var jsons = new[]
                    {
                        new {
                            carnet = dr[0].ToString(),
                            nombre = document.GetValue("nombre").AsString + " " + document.GetValue("apellido").AsString + " " + document.GetValue("apellido1").AsString,
                        }

                     };
                    Console.WriteLine(jsons);
                    estudiantes.Add(jsons);
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
                estudiantes.Add(response);

            }

            List<object> retornar = new List<object>();
            for (var x = 0; x < estudiantes.Count; x++)
            {
                var tempList = (IList<object>)estudiantes[x];
                retornar.Add(tempList[0]);
            }
            conn.Close();
            return retornar;

        }


        // Controller para todos los profesores asignados en un grupo específico
        [Route("verProfesorSemestre")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public List<Object> verProfesorSemestre(Grupo grupo)
        {
            List<Object> profesores = new List<Object>();
            Curso usuarioCarrera = new Curso();
            //Connect to database
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "verProfesorSemestre";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ano", grupo.ano);
            cmd.Parameters.AddWithValue("@periodo", grupo.periodo);
            cmd.Parameters.AddWithValue("@grupo", grupo.grupo);
            cmd.Parameters.AddWithValue("@codigoCurso", grupo.codigoCurso);
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
                profesores.Add(response);
                while (dr.Read())
                {
                    var connectionString = "mongodb+srv://admin:admin@usuarios.ozlkz.mongodb.net/Usuarios?retryWrites=true&w=majority";
                    var mongoClient = new MongoClient(connectionString);
                    var dataBase = mongoClient.GetDatabase("Usuarios");
                    var collection = dataBase.GetCollection<BsonDocument>("profesores");
                    var filter1 = Builders<BsonDocument>.Filter.Eq("cedula", dr[0].ToString());
                    var projection = Builders<BsonDocument>.Projection.Exclude("_id");
                    var document = collection.Find(filter1).Project(projection).FirstOrDefault();
                    var jsons = new[]
                    {
                        new {
                            cedula = dr[0].ToString(),
                            nombre = document.GetValue("nombre").AsString + " " + document.GetValue("apellido").AsString,
                        }

                     };
                    Console.WriteLine(jsons);
                    profesores.Add(jsons);
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
                profesores.Add(response);

            }

            List<object> retornar = new List<object>();
            for (var x = 0; x < profesores.Count; x++)
            {
                var tempList = (IList<object>)profesores[x];
                retornar.Add(tempList[0]);
            }
            conn.Close();
            return retornar;
        }




        // Controller para ver el reporte de notas de un grupo específico
        [Route("verNotasGrupo")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public List<Object> verNotasGrupo(Grupo grupo)
        {
            List<Object> semestres = new List<Object>();
            //Connect to database
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "verNotasGrupo";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@codigoCurso", grupo.codigoCurso);
            cmd.Parameters.AddWithValue("@numeroGrupo", grupo.numeroGrupo);
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
                    var collection = dataBase.GetCollection<BsonDocument>("estudiantes");
                    var filter = Builders<BsonDocument>.Filter.Eq("carnet", dr[0].ToString());
                    var document = collection.Find(filter).FirstOrDefault();
                    var jsons = new[]
                    {
                        new {
                            carnet = dr[0].ToString(),
                            nombreEstudiante = document.GetValue("nombre").AsString + " " + document.GetValue("apellido").AsString + " " + document.GetValue("apellido1").AsString,
                            nombreEvaluacion = dr[1].ToString(),
                            rubro = dr[2].ToString(),
                            notaObtenida = dr[3].ToString(),
                            porcentajeObtenido = dr[4],
                            porcentajeEvaluacion = dr[5].ToString(),
                            notaFinalCurso = dr[6]
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
