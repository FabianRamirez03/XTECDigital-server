using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using XTecDigital_Server.Models;


using System.Text;
using System.Security.Cryptography;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace XTecDigital_Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SemestreController : ControllerBase
    {

        private string serverKey = Startup.getKey();

        [Route("crearSemestre")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object crearSemestre(Semestre semestre)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "crearSemestre";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ano", semestre.ano);
            cmd.Parameters.AddWithValue("@periodo", semestre.periodo);
            cmd.Parameters.AddWithValue("@cedulaAdmin", semestre.cedulaAdmin);
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

        [Route("verSemestres")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public List<Object> verSemestres()
        {
            List<Object> semestres = new List<Object>();
            //Connect to database
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "verSemestres";
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
                semestres.Add(response);
                while (dr.Read())
                {
                    var jsons = new[]
                    {
                        new {
                            ano = dr[0].ToString(),
                            periodo = dr[1].ToString(),
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


        [Route("crearSemestreExcel")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public void crearSemestreExcel()
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "crearSemestreExcel";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            List<Object> respuesta = new List<Object>();
            try
            {
                cmd.ExecuteNonQuery();
                
            }
            catch (Exception e)
            {
                string[] separatingStrings = { "\r" };
                Debug.WriteLine(e.Message.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries)[0]);
            }
            conn.Close();



            agregarUsuariosMongoExcel();
        }

        public void agregarProfeMongoExcel()
        {
            List<Object> cursos = new List<Object>();
            Curso usuarioCarrera = new Curso();
            //Connect to database
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "obtenerProfesorExcel";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader dr = cmd.ExecuteReader();
            try
            {

                while (dr.Read())
                {
                    var connectionString = "mongodb+srv://admin:admin@usuarios.ozlkz.mongodb.net/Usuarios?retryWrites=true&w=majority";
                    var mongoClient = new MongoClient(connectionString);
                    var dataBase = mongoClient.GetDatabase("Usuarios");
                    var collection = dataBase.GetCollection<BsonDocument>("profesores");
                    var document = new BsonDocument
                                                        {
                                                            { "cedula", dr[0].ToString() },
                                                            { "nombre", dr[1].ToString() },
                                                            { "apellido", dr[2].ToString() },
                                                            { "apellido1", dr[3].ToString() },
                                                            { "password", MD5Hash(dr[0].ToString()) },
                                                        };
                    collection.InsertOne(document);
                }

            }
            catch (Exception e)
            {
                string[] separatingStrings = { "\r" };
                Debug.WriteLine(e.Message.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries)[0]);
            }
            conn.Close();
        }


        public void agregarEstudianteMongoExcel()
        {
            List<Object> cursos = new List<Object>();
            Curso usuarioCarrera = new Curso();
            //Connect to database
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "obtenerEstudiantesExcel";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader dr = cmd.ExecuteReader();
            try
            {

                while (dr.Read())
                {
                    var connectionString = "mongodb+srv://admin:admin@usuarios.ozlkz.mongodb.net/Usuarios?retryWrites=true&w=majority";
                    var mongoClient = new MongoClient(connectionString);
                    var dataBase = mongoClient.GetDatabase("Usuarios");
                    var collection = dataBase.GetCollection<BsonDocument>("estudiantes");
                    var document = new BsonDocument
                                                        {
                                                            { "carnet", dr[0].ToString() },
                                                            { "nombre", dr[1].ToString() },
                                                            { "apellido", dr[2].ToString() },
                                                            { "apellido1", dr[3].ToString() },
                                                            { "correo", dr[0].ToString() + "@estudiantec.cr"},
                                                            { "password", MD5Hash(dr[0].ToString()) },
                                                        };
                    collection.InsertOne(document);
                }

            }
            catch (Exception e)
            {
                string[] separatingStrings = { "\r" };
                Debug.WriteLine(e.Message.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries)[0]);
            }
            conn.Close();

        }

        public void agregarUsuariosMongoExcel()
        {
            agregarProfeMongoExcel();
            agregarEstudianteMongoExcel();
            borrarTablaTemporal();
        }

        [Route("verCursosSemestre")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public List<Object> verCursosSemestre(Semestre semestre)
        {
            List<Object> cursos = new List<Object>();
            Curso usuarioCarrera = new Curso();
            //Connect to database
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "verCursosSemestre";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ano", semestre.ano);
            cmd.Parameters.AddWithValue("@periodo", semestre.periodo);
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
                    var jsons = new[]
                    {
                        new {
                            codigo = dr[0].ToString(),
                            numeroGrupo = (int)dr[1],
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



        public void borrarTablaTemporal()
        {
            List<Object> cursos = new List<Object>();
            Curso usuarioCarrera = new Curso();
            //Connect to database
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "borrarTablaTemporal";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader dr = cmd.ExecuteReader();
            conn.Close();
        }


        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

    }


}
