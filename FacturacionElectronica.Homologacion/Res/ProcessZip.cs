using System;
using System.IO;
using System.Linq;
using Ionic.Zip;

namespace FacturacionElectronica.Homologacion.Res
{
    internal static class ProcessZip
    {
        /// <summary>
        /// Extrae los Archivos dentro del Array[](Zip)
        /// </summary>
        /// <param name="arrayZip">bytes of content zip</param>
        /// <param name="directoryOutput">Directorio a extraer archivos</param>
        /// <returns>Stream que contiene el archivo XML-CDR </returns>
        /// <exception cref="FileNotFoundException">El archivo xml CDR de respuesta no fue encontrado</exception>
        public static Stream ExtractFile(byte[] arrayZip, string directoryOutput)
        {
            using (var zipContent= new MemoryStream(arrayZip))
            {
                using (var zip = ZipFile.Read(zipContent))
                {
                    zip.ExtractAll(Path.GetDirectoryName(directoryOutput), ExtractExistingFileAction.OverwriteSilently);
                    var cdr = zip.Entries.FirstOrDefault(f => f.FileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase));
                    if(cdr == null)
                        throw new FileNotFoundException("El archivo xml CDR de respuesta no fue encontrado");
                    var stream = new MemoryStream();
                    cdr.Extract(stream);
                    stream.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                    return stream;
                }
            }

        }
        /// <summary>
        /// Comprime un archivo, y guarda el zip en el mismo directorio con el mismo nombre.
        /// </summary>
        /// <param name="pstrRutaFile">Archivo a comprimir</param>
        /// <returns>bytes of zip</returns>
        public static byte[] CompressFile(string pstrRutaFile)
        {
            using (var zip = new ZipFile())
            {
                zip.AddFile(pstrRutaFile, string.Empty);
                return ConvertToArray(zip);
            }
        }

        /// <summary>
        /// Comprime un archivo, y guarda el zip en el mismo directorio con el mismo nombre.
        /// </summary>
        /// <param name="filename">filename</param>
        /// <param name="content">Content of file</param>
        /// <returns>bytes of zip</returns>
        public static byte[] CompressFile(string filename, byte[] content)
        {
            using (var zip = new ZipFile())
            {
                zip.AddEntry(filename, content);
                return ConvertToArray(zip);
            }
        }

        private static byte[] ConvertToArray(ZipFile zip)
        {
            using (var mem = new MemoryStream())
            {
                zip.Save(mem);
                return mem.ToArray();
            }
        }
    }
}
