using cer_gateway.Models;
using Newtonsoft.Json;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace cer_gateway.Aggregates
{
    public class ProveedoresAggregates : IDefinedAggregator
    {
        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {
            var tipoImpuestos = JsonConvert.DeserializeObject<GeneralResponse>
                (await responses[0].Items.DownstreamResponse().Content.ReadAsStringAsync())
                .data;

            var tipoPersonas = JsonConvert.DeserializeObject<GeneralResponse>
                (await responses[1].Items.DownstreamResponse().Content.ReadAsStringAsync())
                .data;

            var paises = JsonConvert.DeserializeObject<GeneralResponse>
                (await responses[2].Items.DownstreamResponse().Content.ReadAsStringAsync())
                .data;

            var monedas = JsonConvert.DeserializeObject<GeneralResponse>
                (await responses[3].Items.DownstreamResponse().Content.ReadAsStringAsync())
                .data;

            var tipoEmpresas = JsonConvert.DeserializeObject<GeneralResponse>
                (await responses[4].Items.DownstreamResponse().Content.ReadAsStringAsync())
                .data;

            var response = new GeneralResponse()
            {
                data = new
                {
                    tipoImpuestos,
                    tipoPersonas,
                    paises,
                    monedas,
                    tipoEmpresas
                },
                message = "Ok",
                status = 200
            };

            var contentBuilder = new StringBuilder();
            contentBuilder.Append(JsonConvert.SerializeObject(response));

            var stringContent = new StringContent(contentBuilder.ToString())
            {
                Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
            };

            return new DownstreamResponse(stringContent, HttpStatusCode.OK, new List<KeyValuePair<string, IEnumerable<string>>>(), "OK");


        }
    }
}