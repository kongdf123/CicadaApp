using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Infra.Data
{
    public class DbInitializer
    {
        private readonly SqliteConnectionFactory _factory;

        public DbInitializer(SqliteConnectionFactory factory)
        {
            _factory = factory;
        }

        public void Initialize()
        {
            using var conn = _factory.CreateConnection();

            conn.Execute(@"
                CREATE TABLE IF NOT EXISTS Telemetry (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    DeviceId TEXT,
                    Tag TEXT,
                    Value REAL,
                    Timestamp TEXT
                );
                ");
        }
    }
}
