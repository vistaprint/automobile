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

SQLiteClient.cs 
*/
using System;
using System.Data.SQLite;
using Automobile.Mobile.Framework;
using Automobile.Mobile.Framework.Data;

namespace Automobile.Registrar
{

    public class SQLiteClient : IMobileDb
    {
        private static string _dbName;
        private static SQLiteConnection _sharedDb;

        /// <summary>
        /// Creates tables, if needed.
        /// </summary>
        /// <param name="dbName">Name of the file to use</param>
        public SQLiteClient(string dbName) : this(dbName, dbName == ":memory:") {}

        /// <summary>
        /// Creates tables, if needed. Optionally creates a shared connection (for in-memory or temporary file db).
        /// </summary>
        /// <param name="dbName">Name of the file to use, or :memory: for in memory db</param>
        /// <param name="shared">To share a connection object between all calls to this class</param>
        public SQLiteClient(string dbName, bool shared)
        {
            _dbName = "Data Source=" + dbName;
            var db = new SQLiteConnection(_dbName);
            db.Open();

            using(SQLiteCommand createTable = new SQLiteCommand("create table if not exists DeviceInfo ( MobileOs text, DeviceModel text, OsVersion text, UniqueId text, IP text, LastSeen text, Availible int, UNIQUE(UniqueId))", db))
            {
                createTable.ExecuteNonQuery();
            }

            if (shared)
            {
                _sharedDb = db;
            }
            else
            {
                db.Close();
                GC.Collect();
            }
        }

        /// <summary>
        /// Closes the shared connection if one exists
        /// </summary>
        public void Dispose()
        {
            if(_sharedDb != null)
            {
                _sharedDb.Close();
            }
        }

        public void Register(DeviceInfo info)
        {
            var db = _sharedDb ?? new SQLiteConnection(_dbName).OpenAndReturn();

            using (SQLiteCommand deviceInfo = new SQLiteCommand(db))
            {
                deviceInfo.CommandText =
                    string.Format(
                        "REPLACE INTO DeviceInfo (MobileOs, DeviceModel, OsVersion, UniqueId, IP, LastSeen, Availible) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', datetime('now'), 1)",
                        info.MobileOs, info.DeviceModel, info.OsVersion, info.UniqueId, info.IP);
                deviceInfo.ExecuteNonQuery();
            }

            if (_sharedDb == null)
            {
                db.Close();
                GC.Collect();
            }
        }

        public void SetAvailibility(DeviceInfo device, bool availible)
        {
            var db = _sharedDb ?? new SQLiteConnection(_dbName).OpenAndReturn();

            using(SQLiteCommand availUpdate = new SQLiteCommand(db))
            {
                availUpdate.CommandText = string.Format("UPDATE DeviceInfo SET Availible = {0} WHERE UniqueId = '{1}'", availible ? 1 : 0, device.UniqueId);
                availUpdate.ExecuteNonQuery();
            }

            if (_sharedDb == null)
            {
                db.Close();
                GC.Collect();
            }
        }

        public DeviceInfo GetFirstMatch(DeviceInfo device)
        {
            return GetFirstMatch(device, true);
        }

        /// <summary>
        /// Returns the IP of the first registered device which matches all the info.
        /// Null values are ignored in the match
        /// </summary>
        /// <param name="info">Info to match</param>
        /// <param name="filterByAvailible"> </param>
        /// <returns>IP of the first match</returns>
        public DeviceInfo GetFirstMatch(DeviceInfo device, bool filterByAvailible)
        {
            var db = _sharedDb ?? new SQLiteConnection(_dbName).OpenAndReturn();
            DeviceInfo match;
            using(SQLiteCommand deviceInfo = new SQLiteCommand(db))
            {
                deviceInfo.CommandText ="SELECT MobileOs, DeviceModel, OsVersion, UniqueId, IP FROM DeviceInfo WHERE";
                bool first = true;

                if(device.MobileOs != null)
                {
                    deviceInfo.CommandText += string.Format(" MobileOs = '{0}'", device.MobileOs);
                    first = false;
                }
                if(device.DeviceModel != null)
                {
                    deviceInfo.CommandText += string.Format("{0} DeviceModel = '{1}'", first ? "" : " AND", device.DeviceModel);
                    first = false;
                }
                if(device.OsVersion != null)
                {
                    deviceInfo.CommandText += string.Format("{0} OsVersion = '{1}'", first ? "" : " AND", device.OsVersion);
                    first = false;
                }
                if (device.UniqueId != null)
                {
                    deviceInfo.CommandText += string.Format("{0} UniqueId = '{1}'", first ? "" : " AND", device.UniqueId);
                    first = false;
                }
                if(filterByAvailible)
                {
                    deviceInfo.CommandText += string.Format("{0} Availible = 1", first ? "" : " AND");
                }
                deviceInfo.CommandText += " limit 1";

                using (var reader = deviceInfo.ExecuteReader())
                {
                    match = !reader.HasRows
                                ? null
                                : new DeviceInfo
                                  {
                                      DeviceModel = (string) reader["DeviceModel"],
                                      MobileOs =
                                          (MobileOs) Enum.Parse(typeof (MobileOs), (string) reader["MobileOs"]),
                                      IP = (string) reader["IP"],
                                      OsVersion = (string) reader["OsVersion"],
                                      UniqueId = (string) reader["UniqueId"]
                                  };
                }
            }

            if (_sharedDb == null)
            {
                db.Close();
                GC.Collect();
            }

            return match;
        }
    }
}