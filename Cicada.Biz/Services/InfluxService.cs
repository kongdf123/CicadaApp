using Cicada.Domain.Entities;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using InfluxDB.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Biz.Services
{
    public class InfluxService
    {
        private readonly InfluxDBClient _client;
        private readonly string _bucket = "cicada";
        private readonly string _org = "your-org";

        public InfluxService()
        {
            _client = InfluxDBClientFactory.Create(
                "http://localhost:8086",
                "your-token");
        }

        public async Task WriteAsync(Telemetry data)
        {
            var writeApi = _client.GetWriteApiAsync();

            var point = PointData
                .Measurement("telemetry")
                .Tag("device", data.DeviceId)
                .Tag("tag", data.Tag)
                .Field("value", data.Value)
                .Timestamp(data.Timestamp, WritePrecision.Ms);

            await writeApi.WritePointAsync(point, _bucket, _org);
        }
    }
}
