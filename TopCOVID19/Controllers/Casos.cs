using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using TopCOVID19.Models;

namespace TopCOVID19.Controllers
{
    public class Casos
    {
        //public List<RegionModels> regionCollection;
       
            public async System.Threading.Tasks.Task<List<ResultModels>> GetCasosProvinciasAsync(string _provincia, int _top)
        {
            JObject temp;
            string url = string.Concat("https://covid-19-statistics.p.rapidapi.com/reports?date=", (DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd")), "&iso=",_provincia);
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
            {
                { "x-rapidapi-key", "3b2b86e9cdmsh1b3a597f314a22fp19ae34jsn5ee2ff6aa3f5" },
                { "x-rapidapi-host", "covid-19-statistics.p.rapidapi.com" },
            },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                temp = JObject.Parse(body);

            }

            var res = (JArray)temp["data"];
            List<ResultModels> resultCollection = new List<ResultModels>();

            foreach (var item in res)
            {
                ResultModels casosProvincia = new ResultModels();

                casosProvincia.name = item["region"]["province"].ToString();
                casosProvincia.cases = int.Parse(item["confirmed"].ToString());
                casosProvincia.deaths = int.Parse(item["deaths"].ToString());

                resultCollection.Add(casosProvincia);
            }

            resultCollection = resultCollection.OrderByDescending(x => x.cases).Take(_top).ToList();

            return resultCollection;
        }

        public async System.Threading.Tasks.Task<List<ResultModels>> GetCasosPorRegioneAsync(int _top)
        {

            JObject temp;
            string url = string.Concat("https://covid-19-statistics.p.rapidapi.com/reports?date=", (DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd")));
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
            {
                { "x-rapidapi-key", "3b2b86e9cdmsh1b3a597f314a22fp19ae34jsn5ee2ff6aa3f5" },
                { "x-rapidapi-host", "covid-19-statistics.p.rapidapi.com" },
            },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                temp = JObject.Parse(body);

            }

            var res = (JArray)temp["data"];

            List<ProvinciaModels> provinciaCollection = GetProvinciaCollection(res);
            List<RegionModels> regionCollection = await GetRegionCollectionAsync();

 
            List<ResultModels> resultCollection = new List<ResultModels>();
            foreach (var region in regionCollection)
            {
                ResultModels resultado = new ResultModels();

                resultado.name = region.name;

                foreach (var provincia in provinciaCollection)
                {
                    if (region.iso.Equals(provincia.iso))
                    {
                        resultado.cases += int.Parse(provincia.causes);
                        resultado.deaths += int.Parse(provincia.deaths);
                    }
                }

                resultCollection.Add(resultado);
            }



            resultCollection = resultCollection.OrderByDescending(x => x.cases).Take(_top).ToList();
     

            return resultCollection;

        }

        //public List<ResultModels> GetTop10Causes(List<RegionModels> _region, List<ProvinciaModels> _provincia, int _top)
        //{
        //    List<ResultModels> resultCollection = new List<ResultModels>();

        //    foreach (var region in _region)
        //    {
        //        ResultModels resultado = new ResultModels();

        //        resultado.name = region.name;

        //        foreach (var provincia in _provincia)
        //        {
        //            if (region.iso.Equals(provincia.iso))
        //            {
        //                resultado.cases += int.Parse(provincia.causes);
        //                resultado.deaths += int.Parse(provincia.deaths);
        //            }
        //        }

        //        resultCollection.Add(resultado);
        //    }



        //    resultCollection = resultCollection.OrderByDescending(x => x.cases).Take(_top).ToList();

        //    return resultCollection;
        //}

        public List<ProvinciaModels> GetProvinciaCollection(JArray _jsonArray)
        {
            List<ProvinciaModels> provinciaCollection = new List<ProvinciaModels>();

            foreach (var item in _jsonArray)
            {
                ProvinciaModels provincia = new ProvinciaModels();
                provincia.iso = item["region"]["iso"].ToString();
                provincia.region = item["region"]["name"].ToString();
                provincia.province = item["region"]["province"].ToString();
                provincia.causes = item["confirmed"].ToString();
                provincia.deaths = item["deaths"].ToString();

                provinciaCollection.Add(provincia);
            }

            return provinciaCollection;

        }
        public  async System.Threading.Tasks.Task<List<RegionModels>> GetRegionCollectionAsync()
        {
            List<RegionModels> regionCollection; 
            var client = new HttpClient();
         
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://covid-19-statistics.p.rapidapi.com/regions"),
                Headers =
            {
                { "x-rapidapi-key", "3b2b86e9cdmsh1b3a597f314a22fp19ae34jsn5ee2ff6aa3f5" },
                { "x-rapidapi-host", "covid-19-statistics.p.rapidapi.com" },
            },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                string body = await response.Content.ReadAsStringAsync();

                JObject tmp = JObject.Parse(body);
                var res = (JArray)tmp["data"];

                regionCollection = JsonConvert.DeserializeObject<List<RegionModels>>(res.ToString());
                //Console.WriteLine(body);
            }

            return regionCollection;
        }
    }
}