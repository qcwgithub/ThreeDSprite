
public class OnReloadScript : Handler {
    public override MsgType msgType { get { return MsgType.ReloadScript; } }

    public string findFromRequireCache(string file) {
        for (var k in require.cache) {
            if (!k.endsWith(file)) continue;
            if (k.indexOf("node_modules") >= 0) continue;
            return k;
        }
        return null;
    }

    // files 格式 AAAGetSummary.js，都是有前缀的，所以不怕重复
    handle(object socket, MsgReloadScript msg) {
        this.logger.warn("******** OnReloadScript ********");

        //// check
        for (int i = 0; i < msg.jsFiles.length; i++) {
            var file = msg.jsFiles[i];
            ReloadType type = msg.types[i];
            var varName = msg.varNames[i];

            // if (!fs.existsSync(file)) {
            //     this.logger.error("file not exist: " + file);
            //     return MyResponse.create(ECode.Error);
            // }

            if (type != "serverScript" && type != "handler" && type != "gameScript") {
                this.baseScript.error("wrong type");
                return MyResponse.create(ECode.Error);
            }

            if ((type == "serverScript" || type == "gameScript") && !(varName in this.server)) {
                this.baseScript.error("!(varName in this.server)");
                return MyResponse.create(ECode.Error);
            }
        }

        for (int i = 0; i < msg.jsFiles.length; i++) {
            var file = msg.jsFiles[i];
            var type = msg.types[i];
            var varName = msg.varNames[i];

            // 1 remove from require.cache
            var fullPath = this.findFromRequireCache(file);
            if (fullPath == null) {
                this.baseScript.error("file not in require.cache: " + file);
                continue;
            }

            // 2 require file
            delete require.cache[fullPath];
            var f = require(fullPath);
            if (f == null || f.default == null || typeof f.default !== "function") {
                this.baseScript.error("f == null or f.default is not function");
                continue;
            }

            if (type == "serverScript") {
                IScript newScript = new f.default();
                newScript.server = this.server;
                (this.server as any)[varName] = newScript;

                this.logger.info("reload serverScript, varName: " + varName);
            }
            else if (type == "handler") {
                Handler newHandler = new f.default();
                newHandler.server = this.server;

                // 3 replace server.dispatcher.handlers
                // 只允许原来已经存在的
                if (!this.server.baseData.handlers.has(newHandler.msgType)) {
                    this.baseScript.error("msgType == -1");
                    continue;
                }

                // execute
                this.dispatcher.addHandler(newHandler);

                this.logger.info("reload handler, msgType: " + MsgType[newHandler.msgType]);
            }
            else if (type == "gameScript") {
                GameScriptBase newScript = new f.default();
                newScript.init(this.server.pmData, this.server);
                (this.server as any)[varName] = newScript;

                this.logger.info("reload gameScript, varName: " + varName);
            }
        }

        return MyResponse.create(ECode.Success);
    }
}