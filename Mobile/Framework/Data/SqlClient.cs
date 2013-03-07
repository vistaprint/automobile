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

using System;
using System.Data.SqlClient;

namespace Automobile.Mobile.Framework.Data
{
    public class SqlClient : IMobileDb
    {
        private readonly string _connectionString;

        public SqlClient(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Dispose()
        {
            // Nothing to do here for sql
        }

        public void Register(DeviceInfo device)
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            var sql = new SqlCommand(string.Format("exec mob_update_device_registration '{0}', '{1}', '{2}', '{3}'", device.UniqueId, device.MobileOs, device.OsVersion, device.IP), conn);
            sql.ExecuteNonQuery();
        }

        public DeviceInfo GetFirstMatch(DeviceInfo device)
        {
            return GetFirstMatch(device, true);
        }

        public DeviceInfo GetFirstMatch(DeviceInfo device, bool filterByAvailible)
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            var sql = new SqlCommand(string.Format("exec mob_get_first_match '{0}', '{1}', '{2}', '{3}', {4}", device.UniqueId, device.MobileOs, device.OsVersion, device.IP, filterByAvailible ? 1 : 0), conn);
            var reader = sql.ExecuteReader();

            return new DeviceInfo
                       {
                           UniqueId = (string) reader["device_id"],
                           MobileOs = (MobileOs) Enum.Parse(typeof(MobileOs), (string)reader["mobile_od"]),
                           OsVersion =  (string) reader["os_version"],
                           IP = (string) reader["ip"]
                       };
        }

        public void SetAvailibility(DeviceInfo device, bool availible)
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            var sql = new SqlCommand(string.Format("exec mob_set_availibility '{0}', {1}", device.UniqueId, availible ? 1 : 0), conn);
            sql.ExecuteNonQuery();
        }
    }
}
