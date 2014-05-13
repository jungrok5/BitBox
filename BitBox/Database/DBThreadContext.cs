using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitBox.Log;
using BitBox.Util;

namespace BitBox.Database
{
    public class DBThreadContext : ThreadContextBase
    {
        public string ConnectionString;
        private SqlConnection m_DBConnection;

        public SqlConnection GetDBConnection() { return m_DBConnection; }

        public override void Create()
        {
            Logger.Debug("Create " + ToString());
            m_DBConnection = new SqlConnection(ConnectionString);
            m_DBConnection.Open();
        }

        public override void Dispose()
        {
            Logger.Debug("Dispose " + ToString());
            if (m_DBConnection != null)
            {
                m_DBConnection.Dispose();
                m_DBConnection = null;
            }
        }
    }
}
