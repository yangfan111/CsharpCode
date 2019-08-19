using commons.data;
using commons.data.mysql;
using commons.util;
using Core.Utils;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace App.Shared.GameModules.GamePlay.SimpleTest
{
    public class TestPositionManager
    {
        public static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(TestPositionManager));

        private const string FIELD_MAP_ID = "map_id";
        private const string FIELD_POSITION_TYPE = "position_type";
        private const string FIELD_X = "x";
        private const string FIELD_Y = "y";
        private const string FIELD_Z = "z";
        private const string FIELD_ROTATION = "rotation";
        private const string FIELD_TIME = "time";
        private const string TABLE = "test_position";

        [NonSerialized]
        private static string _connectionString;
        public static string MysqlConnection
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    MySqlConnectionStringBuilder config = new MySqlConnectionStringBuilder();
                    XMLParser parser = new XMLParser(Application.dataPath + "/Config/Server/connection.xml");

                    config.Server = parser.GetNodeValue("//host");
                    config.Database = parser.GetNodeValue("//db-name");
                    config.UserID = parser.GetNodeValue("//user");
                    config.Password = parser.GetNodeValue("//password");
                    config.Port = uint.Parse(parser.GetNodeValue("//port"));
                    config.CharacterSet = "utf8";

                    _connectionString = config.ConnectionString;
                }

                return _connectionString;
            }
        }

        private const string MYSQL_CREATE_TABLE = @"CREATE TABLE IF NOT EXISTS `test_position`(
                `map_id` VARCHAR(40) NOT NULL,
                `position_type` VARCHAR(40) NOT NULL,
                `x` FLOAT NOT NULL,
                `y` FLOAT NOT NULL,
                `z` FLOAT NOT NULL,
                `rotation` FLOAT NOT NULL,
                `time` VARCHAR(40) NOT NULL
                    )ENGINE = InnoDB DEFAULT CHARSET = utf8";
        private const string MYSQL_GET_ALL = "select * from test_position";
        private const string MYSQL_DELETE = "DELETE FROM test_position WHERE time={0}";
        private static Dictionary<string, Dictionary<string, List<TestPosition>>> gRecordMap = new Dictionary<string, Dictionary<string, List<TestPosition>>>();
        private static List<TestPosition> gTestPositions = new List<TestPosition>();

        private static string Type { get; set; }
        private static int MapId { get; set; }

        public static int IniTable(int mapId, string type)
        {
            gRecordMap.Clear();
            gTestPositions.Clear();

            Type = type;
            MapId = mapId;

            int result = -1;
            try {
                result = MysqlUtil.Execute(MYSQL_CREATE_TABLE, MysqlConnection);
            } catch {
                Logger.DebugFormat("CreateTable failed and result is {0} !", result);
            }

            List<DataRecord> list = GetAllDataRecord();

            foreach (DataRecord record in list) {
                TestPosition testPosition = new TestPosition();
                if (record.Contains(FIELD_MAP_ID))
                    testPosition.mapId = record.GetValue(FIELD_MAP_ID);
                if (record.Contains(FIELD_POSITION_TYPE))
                    testPosition.type = record.GetValue(FIELD_POSITION_TYPE);
                if (record.Contains(FIELD_X))
                    testPosition.x = float.Parse(record.GetValue(FIELD_X));
                if (record.Contains(FIELD_Y))
                    testPosition.y = float.Parse(record.GetValue(FIELD_Y));
                if (record.Contains(FIELD_Z))
                    testPosition.z = float.Parse(record.GetValue(FIELD_Z));
                if (record.Contains(FIELD_ROTATION))
                    testPosition.rotation = float.Parse(record.GetValue(FIELD_ROTATION));
                if (record.Contains(FIELD_TIME))
                    testPosition.time = record.GetValue(FIELD_TIME);

                Dictionary<string, List<TestPosition>> typeDict = null;
                List<TestPosition> testPositions = null;
                if (!gRecordMap.TryGetValue(testPosition.mapId, out typeDict))
                {
                    typeDict = new Dictionary<string, List<TestPosition>>();
                    gRecordMap[testPosition.mapId] = typeDict;
                }

                if (!typeDict.TryGetValue(testPosition.type, out testPositions))
                {
                    testPositions = new List<TestPosition>();
                    typeDict[testPosition.type] = testPositions;
                }

                if (mapId.ToString().Equals(testPosition.mapId) &&
                    type.Equals(testPosition.type))
                {
                    gTestPositions = testPositions;
                }

                testPositions.Add(testPosition);
            }

            return result;
        }

        public static List<TestPosition> GetTestPositions()
        {
            return gTestPositions;
        }

        public static List<DataRecord> GetAllDataRecord()
        {
            List<DataRecord> list = MysqlUtil.SelectRecords(MYSQL_GET_ALL, MysqlConnection);
            return list;
        }

        public static void Add(Vector3 position, float rotation, string timeStamp)
        {
            DataRecord dataRecord = new DataRecord();
            dataRecord.AddField(FIELD_MAP_ID, MapId.ToString());
            dataRecord.AddField(FIELD_POSITION_TYPE, Type);
            dataRecord.AddField(FIELD_X, position.x.ToString());
            dataRecord.AddField(FIELD_Y, position.y.ToString());
            dataRecord.AddField(FIELD_Z, position.z.ToString());
            dataRecord.AddField(FIELD_ROTATION, rotation.ToString());
            dataRecord.AddField(FIELD_TIME, timeStamp);
            MysqlUtil.Add(dataRecord, TABLE, MysqlConnection);

            if (!Exsit(timeStamp))
            {
                TestPosition testPosition = new TestPosition();
                testPosition.mapId = MapId.ToString();
                testPosition.type = Type;
                testPosition.x = position.x;
                testPosition.y = position.y;
                testPosition.z = position.z;
                testPosition.rotation = rotation;
                testPosition.time = timeStamp;
                gTestPositions.Add(testPosition);
            }
        }

        private static bool Exsit(string timeStamp)
        {
            for (int i = 0, maxi = (null == gTestPositions ? 0 : gTestPositions.Count); i < maxi; i++)
            {
                TestPosition testPosition = gTestPositions[i];
                if (testPosition.time.Equals(timeStamp))
                    return true;
            }
            return false;
        }
        public static void Delete(string timeStamp)
        {
            TestPosition test = null;
            for (int i = 0, maxi = (null == gTestPositions ? 0 : gTestPositions.Count); i < maxi; i++)
            {
                TestPosition testPosition = gTestPositions[i];
                if (testPosition.time.Equals(timeStamp)) {
                    test = testPosition;
                    break;
                }
            }
            if (null != test)
                gTestPositions.Remove(test);

            try
            {
                MysqlUtil.Execute(string.Format(MYSQL_DELETE, timeStamp), MysqlConnection);
            }
            catch
            {
                Logger.DebugFormat("Delete failed !");
            }
        }
    }
}
