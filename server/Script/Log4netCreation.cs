using System;
using System.Collections.Generic;
using System.Xml;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace Script
{
    public class Log4netCreation
    {
        ILoggerRepository log4netRepo;
        public ILog getLogger(string name)
        {
            return LogManager.GetLogger(this.log4netRepo.Name, name);
        }

        public void Create(List<string> loggerNamesToAdd, ConfigLoader configLoader)
        {
            XmlElement xmlRoot = configLoader.Log4netConfigXml;

            // find templates
            XmlNode consoleAppender = null;
            XmlNode fileAppenderTemplate = null;
            XmlNode loggerTemplate = null;

            XmlNode node = xmlRoot.FirstChild;
            while (node != null)
            {
                if (consoleAppender == null && node.Name == "appender" && node.Attributes["name"].Value == "console")
                {
                    consoleAppender = node;
                }
                if (fileAppenderTemplate == null && node.Name == "appender" && node.Attributes["name"].Value == "file")
                {
                    fileAppenderTemplate = node;
                }
                if (loggerTemplate == null && node.Name == "logger")
                {
                    loggerTemplate = node;
                }
                node = node.NextSibling;
            }

            if (consoleAppender == null || fileAppenderTemplate == null || loggerTemplate == null)
            {
                throw new Exception("init log4net failed 1");
            }
#if RELEASE
            consoleAppender.ParentNode.RemoveChild(consoleAppender);
            node = loggerTemplate.FirstChild;
            while (node != null)
            {
                if (node.Name == "appender-ref" &&  node.Attributes["ref"].Value == "console")
                {
                    node.ParentNode.RemoveChild(node);
                    break;
                }
                node = node.NextSibling;
            }
#endif

            // 
            foreach (string loggerName in loggerNamesToAdd)
            {
                // 1
                XmlNode fileAppender = fileAppenderTemplate.Clone();
                fileAppenderTemplate.ParentNode.AppendChild(fileAppender);

                fileAppender.Attributes["name"].Value = "file_" + loggerName;
                bool success = false;
                node = fileAppender.FirstChild;
                while (node != null)
                {
                    if (node.Name == "file")
                    {
                        success = true;
                        node.Attributes["value"].Value = "./logs/" + loggerName;
                        break;
                    }
                    node = node.NextSibling;
                }
                if (!success)
                {
                    throw new Exception("init log4net failed 2");
                }

                // 2
                XmlNode logger = loggerTemplate.Clone();
                loggerTemplate.ParentNode.AppendChild(logger);

                logger.Attributes["name"].Value = loggerName;
                success = false;
                node = logger.FirstChild;
                while (node != null)
                {
                    if (node.Name == "appender-ref" && node.Attributes["ref"].Value == "file")
                    {
                        success = true;
                        node.Attributes["ref"].Value = "file_" + loggerName;
                        break;
                    }
                    node = node.NextSibling;
                }
                if (!success)
                {
                    throw new Exception("init log4net failed 3");
                }
            }

            fileAppenderTemplate.ParentNode.RemoveChild(fileAppenderTemplate);
            loggerTemplate.ParentNode.RemoveChild(loggerTemplate);

            log4netRepo = LogManager.CreateRepository("my_log4net_repo");
            XmlConfigurator.Configure(log4netRepo, xmlRoot);
        }
    }
}