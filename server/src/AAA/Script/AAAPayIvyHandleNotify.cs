
public class AAAPayIvyHandleNotify : AAAHandler {
    public override MsgType msgType { get { return MsgType.AAAPayIvyHandleNotify; }

    private public_key = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA3tyfjK1nTAKBzw5VpaJqqZfJjKzaxj/KM7YCDruXcg0kvqj3ItP5/+Z4dZOSs/8rTrvWiWpBUCLe6Z8i9QdOfE4QYND8WJVIjBp915TPbhXg5TJ1E0AhljCEud2r7w7ICZvFO+g/VSRS0Ys23lHIgtkJb/RSfoGqlT2B3z46tgxbeJbzP678Uu1vKqtIvbsiisuPICgXl9bHvUCqvd5Hyp5RfM6t/bvT8DUne237qqQy4ZlxjkSK24IBZ/VLIcqehjCZVu6Ekk2qmogRmNJN+wRUb4TnDbiJeN5pS48H5FurRCzJraerSsrAxVphXdo09h3VEBShVREDxEFuyNp7xQIDAQAB";

    // from in-app-purchase/lib/google.js
    private chunkSplit(string str, int len, string end) {
        len = parseInt(len as any as string, 10) || 76;
        if (len < 1) {
            return false;
        }
        end = end || "\r\n";
        return str.match(new RegExp(".{0," + len + "}", "g")).join(end);
    }
    private getPublicKey(string publicKey) {
        if (!publicKey) {
            return null;
        }
        var key = this.chunkSplit(publicKey, 64, "\n");
        var pkey = "-----BEGIN PUBLIC KEY-----\n" + key + "-----END PUBLIC KEY-----\n";
        return pkey;
    }
    private checkSign(body: IvyPayResult): ECode {
        var verify = crypto.createVerify("sha1").update(body.jsonData, "utf8");
        var pkey = this.getPublicKey(this.public_key);
        var b = verify.verify(pkey, body.signature, "base64");
        if (!b) {
            this.baseScript.error("sign error");
            r.err = ECode.InvalidSign;
            yield break;
        }
        r.err = ECode.Success;
        yield break;
    }

    *handle(object socket, msg: { res: http.ServerResponse, string body }) {
        var res = msg.res;
        this.logger.info("%s body: %s", this.msgName, msg.body);
        var body = querystring.parse(msg.body) as any as IvyPayResult;

        // 检查签名
        var err = this.checkSign(body);
        if (err != ECode.Success) {
            this.respond(res, false, "sign error");
            return err;
        }

        var gameOrderNo = body.payload;
        if (!this.server.scUtils.checkArgs("S", gameOrderNo)) {
            this.respond(res, false, "invalid payload");
            r.err = ECode.Error;
            yield break;
        }

        // 根据游戏订单号查询
        yield return this.server.payIvySqlUtils.queryPayIvy_orderId(gameOrderNo, r);
        if (r.err != ECode.Success) {
            this.baseScript.error("query gameOrderNo(%s) failed", gameOrderNo);
            this.respond(res, false, "query gameOrderNo failed, " + r.err);
            return r.err;
        }

        SqlTablePayIvy info = r.res;
        if (info == null) {
            this.baseScript.error("query gameOrderNo(%s) info==null", gameOrderNo);
            this.respond(res, false, "query gameOrderNo failed, info==null");
            r.err = ECode.OrderIdNotExist;
            yield break;
        }

        if (info.state == PayIvyState.Succeeded) {
            this.respond(res, true, "succeeded before");
            r.err = ECode.Success;
        yield break;
        }

        if (info.state == PayIvyState.Failed) {
            this.respond(res, false, "failed before");
            r.err = ECode.Success;
        yield break;
        }

        // 更改状态为成功，或失败
        var newState = (body.purchaseState == "1" ? PayIvyState.Succeeded : PayIvyState.Failed);
        r = yield return this.server.payIvySqlUtils.updatePayIvyStateYield(gameOrderNo, newState, body.orderId, body.jsonData);
        if (r.err != ECode.Success) {
            this.respond(res, false, "update state failed");
            return r.err;
        }

        this.respond(res, true, "ok");
        r.err = ECode.Success;
        yield break;
    }

    private respond(res: http.ServerResponse, success: boolean, object message) {
        res.writeHead(200, { "Content-Type": "text/json" });
        var json = { status: success ? 1 : -1, message: message };
        res.write(JSON.stringify(json));
        res.end();
    }
}