using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    }
}
