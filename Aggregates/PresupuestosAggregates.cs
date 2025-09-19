using cer_gateway.Models;
using Newtonsoft.Json;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace cer_gateway.Aggregates
{
    public class PresupuestosAggregates : IDefinedAggregator
    {
        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {
            var monedas = JsonConvert.DeserializeObject<GeneralResponse>
                (await responses[0].Items.DownstreamResponse().Content.ReadAsStringAsync())
                .data;

            var presupuestoOpcionesVigentes = JsonConvert.DeserializeObject<GeneralResponse>
                (await responses[1].Items.DownstreamResponse().Content.ReadAsStringAsync())
                .data;

            var centroCostos = JsonConvert.DeserializeObject<GeneralResponse>
                (await responses[2].Items.DownstreamResponse().Content.ReadAsStringAsync())
                .data;

            var presupuestosConceptos = JsonConvert.DeserializeObject<GeneralResponse>
                (await responses[3].Items.DownstreamResponse().Content.ReadAsStringAsync())
                .data;

            var proyectos = JsonConvert.DeserializeObject<GeneralResponse>
                (await responses[4].Items.DownstreamResponse().Content.ReadAsStringAsync())
                .data;

            var response = new GeneralResponse()
            {
                data = new
                {
                    monedas,
                    presupuestoOpcionesVigentes,
                    centroCostos,
                    presupuestosConceptos,
                    proyectos
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
