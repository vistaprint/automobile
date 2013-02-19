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

MobileDb.cs 
*/

using System.Data.SQLite;
using System.Net;
using Automobile.Mobile.Framework.Device;

namespace Automobile.Registrar
{        
    public static class MobileDb
    {
        private static string _dbName;
        private static SQLiteConnection _sharedDb;

        /// <summary>
        /// Creates tables, if needed.
        /// </summary>
        /// <param name="dbName">Name of the file to use</param>
        public static void Initialize(string dbName)
        {
            Initialize(dbName, false);
        }

        /// <summary>
        /// Creates tables, if needed. Optionally creates a shared connection (for in-memory or temporary file db).
        /// </summary>
        /// <param name="dbName">Name of the file to use, or :memory: for in memory db</param>
        /// <param name="shared">To share a connection object between all calls to this class</param>
        public static void Initialize(string dbName, bool shared)
        {
            _dbName = "Data Source=" + dbName;
            var db = new SQLiteConnection(_dbName);
            db.Open();

            SQLiteCommand createTable = new SQLiteCommand("create table if not exists DeviceInfo ( MobileOs text, DeviceModel text, OsVersion text, UniqueId text, IP text, LastSeen text, Availible int)", db);
            createTable.ExecuteNonQuery();

            if(shared)
            {
                _sharedDb = db;
            }
            else
            {
                db.Close();
            }
        }

        /// <summary>
        /// Closes the shared connection if one exists
        /// </summary>
        public static void Close()
        {
            if(_sharedDb != null)
            {
                _sharedDb.Close();
            }
        }

        public static void Submit(DeviceInfo info)
        {
            var db = _sharedDb ?? new SQLiteConnection(_dbName).OpenAndReturn();

            SQLiteCommand deviceInfo = new SQLiteCommand(db);
            deviceInfo.CommandText =
                string.Format(
                    "REPLACE INTO DeviceInfo (MobileOs, DeviceModel, OsVersion, UniqueId, IP, LastSeen, Availible) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', datetime('now'), 1)",
                    info.MobileOs, info.DeviceModel, info.OsVersion, info.UniqueId, info.IP);
            deviceInfo.ExecuteNonQuery();

            if (_sharedDb == null)
            {
                db.Close();
            }
        }

        /// <summary>
        /// Returns the IP of the first registered device which matches all the info.
        /// Null values are ignored in the match
        /// </summary>
        /// <param name="info">Info to match</param>
        /// <returns>IP of the first match</returns>
        public static IPAddress GetFirstMatch(DeviceInfo info)
        {
            var db = _sharedDb ?? new SQLiteConnection(_dbName).OpenAndReturn();

            SQLiteCommand deviceInfo = new SQLiteCommand(db);
            deviceInfo.CommandText = string.Format("SELECT IP FROM DeviceInfo WHERE MobileOs = '{0}'", info.MobileOs);

            if(info.DeviceModel != null)
            {
                deviceInfo.CommandText += string.Format(" AND DeviceModel = '{0}'", info.DeviceModel);
            }
            if(info.OsVersion != null)
            {
                deviceInfo.CommandText += string.Format(" AND OsVersion = '{0}'", info.OsVersion);
            }
            if (info.UniqueId != null)
            {
                deviceInfo.CommandText += string.Format(" AND UniqueId = '{0}'", info.UniqueId);
            }
            deviceInfo.CommandText += "AND Availible = 1 limit 1";

            var ip = (string)deviceInfo.ExecuteScalar();

            if (_sharedDb == null)
            {
                db.Close();
            }

            return ip == null ? null : IPAddress.Parse(ip);
        }
    }
}