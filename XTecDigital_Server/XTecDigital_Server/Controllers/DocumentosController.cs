using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Syncfusion.EJ2.FileManager.Base;
using XTecDigital_Server.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace XTecDigital_Server.Controllers
{
    [Route("[controller]")]
   // [EnableCors("AllowAllOrigins")]
    [ApiController]
    public class DocumentosController : ControllerBase
    {
        // llave para acceder a la base de datos
        private string serverKey = Startup.getKey();

        // Controller para crear un nuevo documento en la base de datos
        [Route("crearDocumentos")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object crearDocumentos(Documento documento)
        {
            if (!documento.admin)
            {
                Response.Clear();
                Response.StatusCode = 520;
                Response.Headers.Add("status", "No tiene permiso para eliminar este documento");
                return Response;
            }
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "crearDocumentos";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            var prueba = Convert.FromBase64String(documento.archivo);
            string hex = BitConverter.ToString(prueba);
            hex = hex.Replace("-", "");
            hex = "0x" + hex;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@nombreDocumento", documento.nombre);
            cmd.Parameters.AddWithValue("@archivo", documento.archivo);
            cmd.Parameters.AddWithValue("@tamano", documento.tamano);
            cmd.Parameters.AddWithValue("@nombreCarpeta", documento.nombreCarpeta);
            cmd.Parameters.AddWithValue("@idGrupo", documento.idGrupo);
            cmd.Parameters.AddWithValue("@tipoArchivo", documento.tipoArchivo);
            cmd.ExecuteNonQuery();
            conn.Close();
            return 0;

        }


        // Controller para elimianar un documento de la base de datos
        [Route("eliminarDocumentos")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public void eliminarDocumentos(Documento documento)
        {
            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            string insertQuery = "eliminarDocumentos";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@nombre", documento.nombre);
            cmd.Parameters.AddWithValue("@nombreCarpeta", documento.nombreCarpeta);
            cmd.Parameters.AddWithValue("@idGrupo", documento.idGrupo);
            try
            {
                cmd.ExecuteNonQuery();
                Debug.WriteLine("Documento creado exitosamente");
            }
            catch
            {
                Debug.WriteLine("Error al crear carpeta");
            }
            conn.Close();
        }

        // Controller para descargar un archivo de la base de datos
        [Route("Download")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public Object Descargar([FromForm] string downloadInput)
        {
            Debug.WriteLine(downloadInput);
            Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            FileManager args = JsonConvert.DeserializeObject<FileManager>(downloadInput);
            FileManagerResponse createResponse = new FileManagerResponse();
            args.path = args.path.Replace("/", "");
            try
            {
                SqlConnection conn = new SqlConnection(serverKey);
                conn.Open();
                SqlCommand cmd;
                string insertQuery = "verDocumentosEspecifico";
                cmd = new SqlCommand(insertQuery, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@nombreCarpeta", args.path);
                cmd.Parameters.AddWithValue("@codigoCurso", args.Curso);
                cmd.Parameters.AddWithValue("@numeroGrupo", args.Grupo);
                cmd.Parameters.AddWithValue("@nombreDocumento", args.names[0]);
                SqlDataReader dr = cmd.ExecuteReader();
                var defaultAvatarAsBase64 = "";
                var nombre = "";
                while (dr.Read())
                {
                    defaultAvatarAsBase64 = dr[2].ToString();
                    nombre = dr[1].ToString() + "." + dr[3].ToString();
                }
                var imageStream = new MemoryStream();
                // var bytes = Encoding.ASCII.GetBytes(defaultAvatarAsBase64);
                var bytes = Convert.FromBase64String(defaultAvatarAsBase64);
                imageStream = new MemoryStream(bytes);

                var result = new FileStreamResult(imageStream, "APPLICATION/octet-stream");

                result.FileDownloadName = nombre;
                return result;
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.StatusCode = 400;
                Response.Headers.Add("status", e.Message);
                ErrorDetails er = new ErrorDetails();
                er.Message = e.Message.ToString();
                er.Code = "420";
                createResponse.Error = er;
                return createResponse;

            }
        }

        // Controller para subir un nuevo archivo a la base de datos 
        [Route("Upload")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object Subir([FromForm] string Curso, [FromForm] Boolean admin,[FromForm] IList<IFormFile> uploadFiles, [FromForm] string action, [FromForm] string path, [FromForm] string Grupo)
        {
            path = path.Replace("/", "");

            Debug.WriteLine("Subir Archivo");

            if (!admin)
            {
                Response.Clear();
                Response.StatusCode = 520;
                Response.Headers.Add("status", "No tiene permiso para eliminar este documento");
                return Response;
            }

            var file = uploadFiles[0];
            string filename = file.FileName;

            var ms = new MemoryStream();
            file.CopyTo(ms);
            var fileBytes = ms.ToArray();
            string s = Convert.ToBase64String(fileBytes);
            string extension = Path.GetExtension(file.FileName);

            Debug.WriteLine(path);
            Debug.WriteLine(Curso);
            Debug.WriteLine(filename);
            Debug.WriteLine(extension);

            SqlConnection conn = new SqlConnection(serverKey);
            conn.Open();
            SqlCommand cmd;
            string insertQuery = "crearDocumentos";
            cmd = new SqlCommand(insertQuery, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@nombreDocumento", filename);
            cmd.Parameters.AddWithValue("@archivo", s);
            cmd.Parameters.AddWithValue("@tamano", fileBytes.Length);
            cmd.Parameters.AddWithValue("@nombreCarpeta", path);
            cmd.Parameters.AddWithValue("@codigoCurso", Curso);
            cmd.Parameters.AddWithValue("@numeroGrupo", Grupo);
            cmd.Parameters.AddWithValue("@tipoArchivo", extension);
            SqlDataReader dr = cmd.ExecuteReader();
            var defaultAvatarAsBase64 = "";
            var nombre = "";
            while (dr.Read())
            {
                defaultAvatarAsBase64 = dr[2].ToString();
                nombre = dr[1].ToString() + "." + dr[3].ToString();
            }
            return Content("");
        }

        [Route("FileOperations")]
        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public object FileOperations([FromBody] FileManager args)
        {
            Debug.WriteLine("Read Files");
            Debug.WriteLine(HttpContext.Session.GetString("Ubic"));
            if (args.Action == "delete" || args.Action == "rename")
            {
                Debug.WriteLine("Pensar que hacer");
            }
            switch (args.Action)
            {
                case "read":
                    return args.read(serverKey);
                    
                case "delete":
                    // deletes the selected file(s) or folder(s) from the given path.
                    return args.delete(serverKey,Response);
                case "create":
                    return args.create(serverKey, Response);
                    /*
                case "copy":
                    // copies the selected file(s) or folder(s) from a path and then pastes them into a given target path.
                    return this.operation.ToCamelCase(this.operation.Copy(args.Path, args.TargetPath, args.Names, args.RenameFiles, args.TargetData));
                case "move":
                    // cuts the selected file(s) or folder(s) from a path and then pastes them into a given target path.
                    return this.operation.ToCamelCase(this.operation.Move(args.Path, args.TargetPath, args.Names, args.RenameFiles, args.TargetData));
                case "details":
                    // gets the details of the selected file(s) or folder(s).
                    return this.operation.ToCamelCase(this.operation.Details(args.Path, args.Names, args.Data));
                case "create":
                    // creates a new folder in a given path.
                    return this.operation.ToCamelCase(this.operation.Create(args.Path, args.Name));
                case "search":
                    // gets the list of file(s) or folder(s) from a given path based on the searched key string.
                    return this.operation.ToCamelCase(this.operation.Search(args.Path, args.SearchString, args.ShowHiddenItems, args.CaseSensitive));
                case "rename":
                    // renames a file or folder.
                    return this.operation.ToCamelCase(this.operation.Rename(args.Path, args.Name, args.NewName));*/
            }
            return null;
        }


        // Controller para generar la vista previa de una imagen visualmente
        [Route("GetImage")]
        [EnableCors("AnotherPolicy")]
        [HttpGet]
        public Object GetImage([FromQuery(Name = "path")] string path, [FromQuery(Name = "Curso")] string curso, [FromQuery(Name = "grupo")] int grupo, [FromQuery(Name = "Ubic")] string ubic)
        {
            Debug.WriteLine("GETIMAGE");
            Debug.WriteLine(path);
            Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            FileManager args = new FileManager();
            FileManagerResponse createResponse = new FileManagerResponse();
            args.path = path.Replace("/", "");
            args.Ubic = ubic.Replace("/", "");
            args.Curso = curso;
            args.Grupo = grupo;
            Debug.WriteLine(args.Curso);
            try
            {
                SqlConnection conn = new SqlConnection(serverKey);
                conn.Open();
                SqlCommand cmd;
                string insertQuery = "verDocumentosEspecifico";
                cmd = new SqlCommand(insertQuery, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@nombreCarpeta", args.Ubic );
                cmd.Parameters.AddWithValue("@codigoCurso", args.Curso);
                cmd.Parameters.AddWithValue("@numeroGrupo", args.Grupo);
                cmd.Parameters.AddWithValue("@nombreDocumento", args.path);
                SqlDataReader dr = cmd.ExecuteReader();
                var defaultAvatarAsBase64 = "";
                var nombre = "";
                while (dr.Read())
                {
                    defaultAvatarAsBase64 = dr[2].ToString();
                    nombre = dr[1].ToString() + "." + dr[3].ToString();
                }
                var imageStream = new MemoryStream();
                // var bytes = Encoding.ASCII.GetBytes(defaultAvatarAsBase64);
                var bytes = Convert.FromBase64String(defaultAvatarAsBase64);
                imageStream = new MemoryStream(bytes);

                var result = new FileStreamResult(imageStream, "APPLICATION/octet-stream");
                return result;
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.StatusCode = 400;
                Response.Headers.Add("status", e.Message);
                ErrorDetails er = new ErrorDetails();
                er.Message = e.Message.ToString();
                er.Code = "420";
                createResponse.Error = er;
                return createResponse;

            }
        }

    }
}
