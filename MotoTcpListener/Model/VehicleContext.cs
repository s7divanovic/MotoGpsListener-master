using System.Data.Entity;
using System.Data.Common;

namespace MotoTcpListener.Model
{
	public class VehicleContext : DbContext
    {
        public DbSet<Tracking> Tracking { get; set; }

        public VehicleContext() : base("connStr")
        {

        }

        public VehicleContext(DbConnection existingConnection, bool contextOwnsConnection)
			: base(existingConnection, contextOwnsConnection)
		{

        }
    }
}
