using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XTecDigital_Server.Models
{
    public class Noticia
    {
        public Noticia()
        {
        }
        public string codigoCurso { get; set; }
        public int numeroGrupo { get; set; }
        public string tituloNoticia { get; set; }
        public string tituloViejo { get; set; }
        public string mensaje { get; set; }
        public string cedulaAutor { get; set; }
    }
}
