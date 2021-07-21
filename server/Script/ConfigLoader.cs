using System;
using System.IO;
using System.Text;
using System.Xml;
using Data;

namespace Script
{
    public class _LocConfig_
    {
        public string host;
    }

    public class _VersionConfig_
    {
        public string android;
        public string ios;
    }

    public class ConfigLoader
    {
        Purpose purpose;
        public void Load(Purpose purpose)
        {
            this.purpose = purpose;
        }

        T loadHomeJson<T>(string f)
        {
            string personalPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return JsonUtils.parse<T>(File.ReadAllText(personalPath + "/config/" + f, Encoding.UTF8));
        }

        string loadConfigText(string f)
        {
            return File.ReadAllText("./config/" + f, Encoding.UTF8);
        }
        XmlElement parseConfigXml(string text)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(text);
            return doc.DocumentElement;
        }

        T loadPurposeConfigJson<T>(string f)
        {
            return JsonUtils.parse<T>(File.ReadAllText("./Purposes/" + purpose + "/" + f, Encoding.UTF8));
        }

        public string loadGameText(string f)
        {
            return File.ReadAllText("./Assets_Configs/" + f, Encoding.UTF8);
        }

        public string loadImportedText(string f)
        {
            return File.ReadAllText("./Assets_Resources_Imported/" + f, Encoding.UTF8);
        }

        T loadGameJson<T>(string f)
        {
            return JsonUtils.parse<T>(File.ReadAllText("./Assets_Configs/" + f, Encoding.UTF8));
        }

        SqlConfig initSqlConfig(string name)
        {
            string _purpose = purpose.ToString().ToLower();

            return new SqlConfig
            {
                connectionLimit = 10,
                user = $"user_{_purpose}_{name}",
                password = $"gbits*{_purpose}*{name}*user*2020",
                database = $"{_purpose}_{name}",
            };
        }

        _VersionConfig_ _versionConfig;
        public _VersionConfig_ VersionConfig
        {
            get
            {
                if (_versionConfig == null) _versionConfig = this.loadPurposeConfigJson<_VersionConfig_>("version.json");
                return _versionConfig;
            }
        }

        ThisMachineConfig _thisMachineConfig;
        public ThisMachineConfig ThisMachineConfig
        {
            get
            {
                if (_thisMachineConfig == null) _thisMachineConfig = loadHomeJson<ThisMachineConfig>("thisMachineConfig.json");
                return _thisMachineConfig;
            }
        }

        _LocConfig_ _locConfig;
        public _LocConfig_ LocConfig
        {
            get
            {
                if (_locConfig == null) _locConfig = loadPurposeConfigJson<_LocConfig_>("locConfig.json");
                return _locConfig;
            }
        }

        SqlConfig _accountSqlConfig;
        public SqlConfig AccountSqlConfig
        {
            get
            {
                if (_accountSqlConfig == null) _accountSqlConfig = this.initSqlConfig("account");
                return _accountSqlConfig;
            }
        }

        SqlConfig _playerSqlConfig;
        public SqlConfig PlayerSqlConfig
        {
            get
            {
                if (_playerSqlConfig == null) _playerSqlConfig = this.initSqlConfig("player");
                return _playerSqlConfig;
            }
        }

        SqlConfig _logSqlConfig;
        public SqlConfig LogSqlConfig
        {
            get
            {
                if (_logSqlConfig == null) _logSqlConfig = this.initSqlConfig("log");
                return _logSqlConfig;
            }
        }

        XmlElement _log4netConfigXml;
        public XmlElement Log4netConfigXml
        {
            get
            {
                if (_log4netConfigXml == null)
                {
                    string xmlText = loadConfigText("log4netConfig.xml");
                    _log4netConfigXml = parseConfigXml(xmlText);
                }
                return _log4netConfigXml;
            }
        }

        public void loadMap(IBattleConfigs configs, int mapId)
        {
            BattleScript.loadMap(configs, mapId,
                mapId => this.loadImportedText("Egzd/map" + mapId + ".tmx.json"),
                tileset => this.loadImportedText("Egzd/" + tileset + ".json"));
        }
    }
}