/*
Copyright 2013 Vistaprint

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

SqlClient.cs 
*/

using Automobile.Mobile.Framework.Device;
using System.Data.SqlClient;

namespace Automobile.Mobile.Framework.Data
{
    public class SqlClient : IMobileDb
    {
        private string _connectionString;

        public SqlClient(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public void Register(DeviceInfo info)
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            var sql = new SqlCommand(string.Format("exec mob_update_device_registration '{0}', '{1}', '{2}', '{3}'", info.UniqueId, info.MobileOs, info.OsVersion, info.IP), conn);
            var r = sql.ExecuteNonQuery();
        }

        public DeviceInfo GetFirstMatch(DeviceInfo info)
        {
            throw new System.NotImplementedException();
        }

        public DeviceInfo GetFirstMatch(DeviceInfo device, bool filterByAvailible)
        {
            throw new System.NotImplementedException();
        }

        public void SetAvailibility(DeviceInfo device, bool availible)
        {
            throw new System.NotImplementedException();
        }
    }
}
