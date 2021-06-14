using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TopCOVID19.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TopCOVID19.Utils;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Data;

namespace TopCOVID19.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
           
            Casos casos = new Casos();

            //List<ResultModels> ListCasosRegiones = await casos.GetCasosProvinciasAsync("USA",10);
            //ViewBag.titulo1 = "PROVINCE";


            List<ResultModels> ListCasosRegiones = await casos.GetCasosPorRegioneAsync(10);
            ViewBag.titulo1 = "REGION";

            ViewBag.titulo2 = "CASES";
            ViewBag.titulo3 = "DEATHS";
            ViewBag.casosRegiones = ListCasosRegiones;

            List<RegionModels> listRegiones = await casos.GetRegionCollectionAsync();
            ViewBag.regiones = listRegiones;

            return View();
        }
    
        [HttpPost]
        public async Task<ActionResult> Provincia(string regiones)
        {
            Casos casos = new Casos();
            List<ResultModels> ListCasosRegiones = await casos.GetCasosProvinciasAsync(regiones, 10);
            ViewBag.titulo1 = "PROVINCE";

            ViewBag.titulo2 = "CASES";
            ViewBag.titulo3 = "DEATHS";
            ViewBag.casosRegiones = ListCasosRegiones;

            List<RegionModels> listRegiones = await casos.GetRegionCollectionAsync();
            ViewBag.regiones = listRegiones;

            return View();
        }

        [HttpGet]
        public async Task<FileResult> getXML()
        {
            try
            {
                Casos casos = new Casos();


                List<ResultModels> ListCasosRegiones = await casos.GetCasosPorRegioneAsync(10);
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(ListCasosRegiones);
                string str = string.Concat("{regiones:{region:", data, "}");


                XmlNode xml = JsonConvert.DeserializeXmlNode(str);
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(xml.InnerXml);


                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                XmlTextWriter writer = new XmlTextWriter(stream, System.Text.Encoding.UTF8);

                // byte[] bytes = System.Text.Encoding.UTF8.GetBytes(doc);

                xmldoc.WriteTo(writer);
                writer.Flush();
                //Response.Clear();

                Response.Clear();
                byte[] byteArray = stream.ToArray();

                return File(byteArray, "application/octet-stream", "Casos.xml");

            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message.ToString();
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes("error");
                return File(bytes, "application/octet-stream", "error.txt");
            }



        }
        [HttpGet]
        public async Task<FileResult> getJSON()
        {
            try
            {
                Casos casos = new Casos();


                List<ResultModels> ListCasosRegiones = await casos.GetCasosPorRegioneAsync(10);
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(ListCasosRegiones);
                string str = string.Concat("{casos:", data, "}");
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);


                //Response.Clear();

                return File(bytes, "application/octet-stream", "Casos.json");

            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message.ToString();
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes("error");
                return File(bytes, "application/octet-stream", "error.txt");
            }


        }
        [HttpGet]
        public async Task<FileResult> getCSV()
        {
            try
            {
                Casos casos = new Casos();


                List<ResultModels> ListCasosRegiones = await casos.GetCasosPorRegioneAsync(10);
               var data = Newtonsoft.Json.JsonConvert.SerializeObject(ListCasosRegiones);
                string str = string.Concat("{records:{record:", data,"}");


                XmlNode xml = JsonConvert.DeserializeXmlNode(str);
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(xml.InnerXml);
                XmlReader xmlReader = new XmlNodeReader(xml);
                DataSet dataSet = new DataSet();
                dataSet.ReadXml(xmlReader);
                var dataTable = dataSet.Tables[0];

                //Datatable to CSV
                var lines = new List<string>();
                string[] columnNames = dataTable.Columns.Cast<DataColumn>().
                                                  Select(column => column.ColumnName).
                                                  ToArray();
                var header = string.Join(",", columnNames);
                lines.Add(header);
                var valueLines = dataTable.AsEnumerable()
                                   .Select(row => string.Join(",", row.ItemArray));
                lines.AddRange(valueLines);
                //File.WriteAllLines(@"D:/Export.csv", lines);

                var res = string.Join("\n", lines);

                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(res);


                //Response.Clear();

                return File(bytes, "application/octet-stream", "Casos.csv");

            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message.ToString();
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes("error");
                return File(bytes, "application/octet-stream", "error.txt");
            }


        }
    }
}