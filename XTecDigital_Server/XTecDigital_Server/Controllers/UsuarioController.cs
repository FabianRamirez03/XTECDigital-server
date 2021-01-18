using System;
using System.Linq;
using XTecDigital_Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Data.SqlClient;
using System.Collections.Generic;
using MongoDB.Bson.IO;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace XTecDigital_Server.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        // Llave para acceder a la base de datos 
        private string serverKey = Startup.getKey();


        // Controller para validar si un usuario existe en la base de datos
        // Determina el tipo de usuario que es
        [Route("validarUser")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public List<Object> validarUsuario(Usuario usuario)
        {
            var connectionString = "mongodb+srv://admin:admin@usuarios.ozlkz.mongodb.net/Usuarios?retryWrites=true&w=majority";
            var mongoClient = new MongoClient(connectionString);
            var dataBase = mongoClient.GetDatabase("Usuarios");
            var collection = dataBase.GetCollection<BsonDocument>("estudiantes");
            var filter1 = Builders<BsonDocument>.Filter.Eq("carnet", usuario.carnet);
            var filter2 = Builders<BsonDocument>.Filter.Eq("password", usuario.password);
            var projection = Builders<BsonDocument>.Projection.Exclude("_id");
            var document = collection.Find(filter1 & filter2).Project(projection).FirstOrDefault();
            List<Object> respuesta = new List<Object>();
            if (document != null)
            {
                var response = new[]
                    {
                        new
                        {
                            respuesta = "200 OK",
                            error = "null",
                            rol = "estudiante"
                        },
                     };
                var estudiante = new[]
                        {
                        new
                        {
                            carnet = document.GetValue("carnet").AsString,
                            nombre = document.GetValue("nombre").AsString,
                            apellido = document.GetValue("apellido").AsString,
                            apellido1 = document.GetValue("apellido1").AsString,
                        },
                     };
                respuesta.Add(response);
                respuesta.Add(estudiante);
                return respuesta;
            }
            else
            {
                return buscarProfesor(usuario);
            }
            
        }

        // Controller para buscar si el usuario es un profesor
        private List<Object> buscarProfesor(Usuario usuario)
        {
            var connectionString = "mongodb+srv://admin:admin@usuarios.ozlkz.mongodb.net/Usuarios?retryWrites=true&w=majority";
            var mongoClient = new MongoClient(connectionString);
            var dataBase = mongoClient.GetDatabase("Usuarios");
            var collection = dataBase.GetCollection<BsonDocument>("profesores");
            var filter1 = Builders<BsonDocument>.Filter.Eq("cedula", usuario.carnet);
            var filter2 = Builders<BsonDocument>.Filter.Eq("password", usuario.password);
            var projection = Builders<BsonDocument>.Projection.Exclude("_id");
            var document = collection.Find(filter1 & filter2).Project(projection).FirstOrDefault();
            List<Object> respuesta = new List<Object>();
            if (document != null)
            {
                var response = new[]
                    {
                        new
                        {
                            respuesta = "200 OK",
                            error = "null",
                            rol = "profesor"
                        },
                     };
                var profesor = new[]
                        {
                        new
                        {
                            carnet = document.GetValue("cedula").AsString,
                            nombre = document.GetValue("nombre").AsString,
                            apellido = document.GetValue("apellido").AsString,
                            apellido1 = document.GetValue("apellido1").AsString,
                        },
                     };
                respuesta.Add(response);
                respuesta.Add(profesor);
                return respuesta;
            }
            else
            {
                return buscarAdmin(usuario);
            }
        }

        // Controller para comprobar si el usuario es un administrador
        private List<object> buscarAdmin(Usuario usuario)
        {
            var connectionString = "mongodb+srv://admin:admin@usuarios.ozlkz.mongodb.net/Usuarios?retryWrites=true&w=majority";
            var mongoClient = new MongoClient(connectionString);
            var dataBase = mongoClient.GetDatabase("Usuarios");
            var collection = dataBase.GetCollection<BsonDocument>("admin");
            var filter1 = Builders<BsonDocument>.Filter.Eq("cedula", usuario.carnet);
            var filter2 = Builders<BsonDocument>.Filter.Eq("password", usuario.password);
            var projection = Builders<BsonDocument>.Projection.Exclude("_id");
            var document = collection.Find(filter1 & filter2).Project(projection).FirstOrDefault();
            List<Object> respuesta = new List<Object>();
            if (document != null)
            {
                var response = new[]
                    {
                        new
                        {
                            respuesta = "200 OK",
                            error = "null",
                            rol = "admin"
                        },
                     };
                var admin = new[]
                        {
                        new
                        {
                            cedula = document.GetValue("cedula").AsString,
                        },
                     };
                respuesta.Add(response);
                respuesta.Add(admin);
                return respuesta;
            }
            else
            {
                var response = new[]
                   {
                        new
                        {
                            respuesta = "200 OK",
                            error = "No existe el usuario"
                        },
                     };
                respuesta.Add(response);
                return respuesta;
            }
        }

        // Controller para agregar un nuevo estudiante a la base de datos
        [Route("agregarEstudiante")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object agregarEstudiante(Usuario usuario)
        {
            var connectionString = "mongodb+srv://admin:admin@usuarios.ozlkz.mongodb.net/Usuarios?retryWrites=true&w=majority";
            var mongoClient = new MongoClient(connectionString);
            var dataBase = mongoClient.GetDatabase("Usuarios");
            var collection = dataBase.GetCollection<BsonDocument>("estudiantes");
            var document = new BsonDocument
            {
                { "carnet", usuario.carnet },
                { "nombre", usuario.nombre },
                { "email", usuario.email },
                { "telefono", usuario.telefono },
                { "password", usuario.password },
                { "rol", usuario.rol },
            };
            collection.InsertOne(document);
            return agregarEstudianteSQL(usuario);
        }

        // Controller para agregar un nuevo profesor a la base de datos
        [Route("agregarProfesor")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object agregarProfesor(Usuario usuario)
        {
            var connectionString = "mongodb+srv://admin:admin@usuarios.ozlkz.mongodb.net/Usuarios?retryWrites=true&w=majority";
            var mongoClient = new MongoClient(connectionString);
            var dataBase = mongoClient.GetDatabase("Usuarios");
            var collection = dataBase.GetCollection<BsonDocument>("profesores");
            var document = new BsonDocument
            {
                { "cedula", usuario.carnet },
                { "nombre", usuario.nombre },
                { "email", usuario.email },
                { "telefono", usuario.telefono },
                { "password", usuario.password },
                { "rol", usuario.rol },
            };
            collection.InsertOne(document);
            return agregarProfesorSQL(usuario);
        }

        // Controller para agregar un nuevo administrador a la base de datos
        [Route("agregarAdmin")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object agregarAdmin(Usuario usuario)
        {
            var connectionString = "mongodb+srv://admin:admin@usuarios.ozlkz.mongodb.net/Usuarios?retryWrites=true&w=majority";
            var mongoClient = new MongoClient(connectionString);
            var dataBase = mongoClient.GetDatabase("Usuarios");
            var collection = dataBase.GetCollection<BsonDocument>("admin");
            var document = new BsonDocument
            {
                { "cedula", usuario.carnet },
                { "rol", usuario.rol },
            };
            collection.InsertOne(document);
            return agregarAdminSQL(usuario);
        }

        // Controller para agregar el carnet a la base de datos de SQL server
        [Route("agregarEstudianteSQL")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object agregarEstudianteSQL(Usuario usuario)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "agregarEstudiante";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@carnet", usuario.carnet);
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

        // Controller para agregar la cedula de un profesor a la base de datos de SQL server
        [Route("agregarProfesorSQL")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object agregarProfesorSQL(Usuario usuario)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "agregarProfesor";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@cedula", usuario.cedula);
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


        // Controller para agregar las credenciales de un administrador a la base de datos de SQL server
        [Route("agregarAdminSQL")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object agregarAdminSQL(Usuario usuario)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "agregarAdmin";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@cedula", usuario.cedula);
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

        // Controller para asignar un profesor especifico a un grupo
        [Route("asignarProfesorGrupo")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object asignarProfesorGrupo(Usuario usuario)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "asignarProfesorGrupo";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@codigoCurso", usuario.codigoCurso);
            cmd.Parameters.AddWithValue("@numeroGrupo", usuario.numeroGrupo);
            cmd.Parameters.AddWithValue("@ano", usuario.ano);
            cmd.Parameters.AddWithValue("@periodo", usuario.periodo);   
            cmd.Parameters.AddWithValue("@cedulaProfesor", usuario.cedulaProfesor);
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

        // Controller para eliminar un profesor de un grupo determinado
        [Route("eliminarProfesorGrupo")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public void eliminarProfesorGrupo(Usuario usuario)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "eliminarProfesorGrupo";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@codigoCurso", usuario.codigoCurso);
            cmd.Parameters.AddWithValue("@numeroGrupo", usuario.numeroGrupo);
            cmd.Parameters.AddWithValue("@cedulaProfesor", usuario.cedulaProfesor);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        // Controller para agregar un estudiante a un grupo determinado
        [Route("agregarEstudiantesGrupo")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object agregarEstudiantesGrupo(Usuario usuario)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "agregarEstudiantesGrupo";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@carnet", usuario.carnet);
            cmd.Parameters.AddWithValue("@codigoCurso", usuario.codigoCurso);
            cmd.Parameters.AddWithValue("@numeroGrupo", usuario.numeroGrupo);
            cmd.Parameters.AddWithValue("@ano", usuario.ano);
            cmd.Parameters.AddWithValue("@periodo", usuario.periodo);

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


        // Controller para ver todos los cursos que están asignados a un estudiante específico
        [Route("verCursosEstudiante")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public List<Object> verCursosEstudiante(Usuario usuario)
        {
            List<Object> cursos = new List<Object>();
            Curso usuarioCarrera = new Curso();
            //Connect to database
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "verCursosEstudiante";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@carnet", usuario.carnet);
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
                cursos.Add(response);
                while (dr.Read())
                {

                    var jsons = new[]
                    {
                        new {
                            grupo = (int)dr[0],
                            nombre = dr[1].ToString(),
                            carrera = dr[2].ToString(),
                            creditos = dr[3].ToString(),
                            ano = dr[4].ToString(),
                            periodo = dr[5].ToString(),
                            codigo = dr[6].ToString(),
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


        // Controller para ver todos los cursos a los cuales está asignado un profesor 
        [Route("verCursosProfesor")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public List<Object> verCursosProfesor(Usuario usuario)
        {
            List<Object> cursos = new List<Object>();
            Curso usuarioCarrera = new Curso();
            //Connect to database
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "verCursosProfesor";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@cedula", usuario.cedula);
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
                cursos.Add(response);
                while (dr.Read())
                {

                    var jsons = new[]
                    {
                        new {
                            nombre = dr[0].ToString(),
                            grupo = (int)dr[1],
                            ano = (int)dr[2],
                            periodo = dr[3].ToString(),
                            codigo = dr[4].ToString(),
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

        // Controller para conseguir todos los esrudiantes de la base de datos de Mongo
        [Route("getNombreEstudiantes")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public List<Object> getEstudiantes()
        {
            var connectionString = "mongodb+srv://admin:admin@usuarios.ozlkz.mongodb.net/Usuarios?retryWrites=true&w=majority";
            var mongoClient = new MongoClient(connectionString);
            var dataBase = mongoClient.GetDatabase("Usuarios");
            var collection = dataBase.GetCollection<BsonDocument>("estudiantes");
            var projection = Builders<BsonDocument>.Projection.Exclude("_id");
            var documents = collection.Find(new BsonDocument()).ToList();
            List<Object> respuesta = new List<Object>();
            if (documents != null)
            {
                var response = 
                    new
                        {
                            respuesta = "200 OK",
                            error = "null",
                            rol = "estudiante"
                        };
                respuesta.Add(response);
                
                foreach (BsonDocument doc in documents)
                {
                    var estudiante =

                        new
                        {
                            nombre = doc.GetValue("nombre").AsString + " " + doc.GetValue("apellido").AsString + " " + doc.GetValue("apellido").AsString,
                            carnet = doc.GetValue("carnet").AsString
                        };
                    respuesta.Add(estudiante);
                }
                
                return respuesta;
            }
            else
            {
                var response = new[]
                    {
                        new
                        {
                            respuesta = "200 OK",
                            error = "No hay estudiantes",
                        },
                     };
                respuesta.Add(response);
                return respuesta;
            }

        }


        // Controller para obtener todos los profesores de la base de datos de MOngo
        [Route("getNombreProfesores")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public List<Object> getProfesores()
        {
            var connectionString = "mongodb+srv://admin:admin@usuarios.ozlkz.mongodb.net/Usuarios?retryWrites=true&w=majority";
            var mongoClient = new MongoClient(connectionString);
            var dataBase = mongoClient.GetDatabase("Usuarios");
            var collection = dataBase.GetCollection<BsonDocument>("profesores");
            var projection = Builders<BsonDocument>.Projection.Exclude("_id");
            var documents = collection.Find(new BsonDocument()).ToList();
            List<Object> respuesta = new List<Object>();
            if (documents != null)
            {
                var response =
                    new
                    {
                        respuesta = "200 OK",
                        error = "null",
                        rol = "estudiante"
                    };
                respuesta.Add(response);

                foreach (BsonDocument doc in documents)
                {
                    var estudiante =

                        new
                        {
                            nombre = doc.GetValue("nombre").AsString + " " + doc.GetValue("apellido").AsString + " " + doc.GetValue("apellido").AsString,
                            cedula = doc.GetValue("cedula").AsString
                        };
                    respuesta.Add(estudiante);
                }

                return respuesta;
            }
            else
            {
                var response = new[]
                    {
                        new
                        {
                            respuesta = "200 OK",
                            error = "No hay profesores",
                        },
                     };
                respuesta.Add(response);
                return respuesta;
            }

        }




    }
}
