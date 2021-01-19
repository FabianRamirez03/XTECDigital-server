using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XTecDigital_Server.Models
{
    public class Evaluacion
    {
        public Evaluacion()
        {
        }
        public int idEvaluacion { get; set; }
        public bool grupal { get; set; }
        public string nombre { get; set; }
        public float porcentaje { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
        public string archivo { get; set; }
        public int idRubro { get; set; }
        public string rubro { get; set; }
        public string codigoCurso { get; set; }
        public int numeroGrupo { get; set; }
        public string carnet { get; set; }
        public string nombreEvaluacion { get; set; }
        public string nombArch { get; set; }
        public string tipoArch { get; set; }
    }
}

