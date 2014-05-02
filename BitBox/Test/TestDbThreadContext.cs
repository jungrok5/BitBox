using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitBox.Log;
using BitBox.Util;

namespace BitBox.Test
{
    public class TestDbThreadContext : ThreadContextBase
    {
        private SqlConnection m_DB;

        public SqlConnection GetDB() { return m_DB; }

        public override void Create()
        {
            Logger.Debug("Create " + ToString());
            m_DB = new SqlConnection("Data Source=x;Initial Catalog=x;Persist Security Info=True;User ID=x;Password=x");
            m_DB.Open();
        }

        public override void Dispose()
        {
            Logger.Debug("Dispose " + ToString());
            if (m_DB != null)
            {
                m_DB.Dispose();
                m_DB = null;
            }
        }
    }
}
