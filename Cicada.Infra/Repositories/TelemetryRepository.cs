using Cicada.Domain.Entities;
using Cicada.Domain.Interfaces;
using Cicada.Infra.Data;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Infra.Repositories
{
    public class TelemetryRepository : ITelemetryRepository
    {
        private readonly SqliteConnectionFactory _factory;

        public TelemetryRepository(SqliteConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task SaveAsync(Telemetry data)
        {
            using var conn = _factory.CreateConnection();

            var sql = @"
                INSERT INTO Telemetry (DeviceId, Tag, Value, Timestamp)
                VALUES (@DeviceId, @Tag, @Value, @Timestamp);
                ";

            await conn.ExecuteAsync(sql, data);
        }
    }
}
