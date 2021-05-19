using System.Collections.Generic;

public class PMSqlUtils : SqlUtils {
    *selectPlayerYield(int playerId) {
        MsgDBQuery msg = new MsgDBQuery {
            queryStr = "SELECT * FROM player WHERE id=?;",
            values = new List<object>  { playerId }
        };
        MyResponse r = yield this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg);
        return r;
    }

    private string createInsertQueryStr(PMPlayerInfo player, string[] fields, object[] values) {
        var L = fields.Length;
        if (L == 0) {
            return null;
        }
        if (L != values.Length) {
            this.server.baseScript.error("createInsertQueryStr fields.length != values.length");
            return null;
        }

        List<string> buffer = new List<string>();
        buffer.Add("INSERT INTO player (id,");//) VALUES (" + player.id + ",");
        for (int i = 0; i < L; i++) {
            buffer.Add(fields[i]);
            if (i < L - 1) {
                buffer.Add(",");
            }
        }
        buffer.Add(") VALUES (" + player.id + ",");
        for (int i = 0; i < L; i++) {
            buffer.Add("?");
            if (i < L - 1) {
                buffer.Add(",");
            }
        }
        buffer.Add(")");

        var queryStr = buffer.join("");
        return queryStr;
    }

    private string createUpdateQueryStr(PMPlayerInfo player, string[] fields, object[] values) {
        var L = fields.Length;
        if (L == 0) {
            return null;
        }
        if (L != values.Length) {
            this.server.baseScript.error("saveFieldBatch fields.length != values.length");
            return null;
        }

        List<string> buffer = new List<string>();
        buffer.Add("UPDATE player SET ");
        for (int i = 0; i < L; i++) {
            buffer.Add(fields[i] + " = ?");
            if (i < L - 1) {
                buffer.Add(",");
            }
        }
        buffer.Add(" WHERE id=" + player.id);

        var queryStr = buffer.join("");
        return queryStr;
    }

    private void saveFieldBatch(PMPlayerInfo player, string[] fields, object[] values) {
        var queryStr = this.createUpdateQueryStr(player, fields, values);
        if (queryStr == null) {
            return;
        }
        MsgDBQuery msg = {
            queryStr: queryStr,
            values: values,
            expectedAffectedRows: 1,
        };
        this.server.netProto.send(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg, (r: MyResponse) => {
            if (r.err != ECode.Success) {
                this.server.baseScript.error("saveFieldBatch failed. " + queryStr);
            }
        });
    }

    *saveFieldBatchYield(PMPlayerInfo player, string fields[], object values[]) {
        var queryStr = this.createUpdateQueryStr(player, fields, values);
        if (queryStr == null) {
            return MyResponse.create(ECode.Error);
        }

        MsgDBQuery msg = {
            queryStr: queryStr,
            values: values,
            expectedAffectedRows: 1,
        };
        MyResponse r = yield this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg);
        return r;
    }

    // 仅用于新玩家
    *insertPlayerYield(PMPlayerInfo player) {
        var obj = new PMSqlHelpObject();
        obj.player = player;
        obj.fields = [];
        obj.values = [];

        //#region autoInsertPlayer >>>>>>>>自动生成区域开始
        //// a
        obj.push_activity(); //0/51
        obj.push_adBox(); //1/51
        obj.push_adReward(); //2/51
        obj.push_auth(); //3/51

        //// b
        obj.push_badge(); //4/51
        obj.push_badgeC(); //5/51
        obj.push_badgeAct(); //6/51
        obj.push_bank(); //7/51
        obj.push_boutiqueInfos(); //8/51

        //// c
        obj.push_consumer(); //12/51
        obj.push_currentBonus(); //13/51

        //// d
        obj.push_dailyReward(); //14/51
        obj.push_diamond(); //15/51

        //// f
        obj.push_freeBox(); //17/51

        //// g
        obj.push_giftVoucher(); //18/51

        //// h
        obj.push_highestHouseLevel(); //19/51
        obj.push_houses(); //20/51

        //// k
        obj.push_kingstreet(); //22/51
        obj.push_kingstreetExtend(); //23/51

        //// l
        obj.push_landcells(); //24/51
        obj.push_loginReward(); //25/51

        //// m
        obj.push_money(); //26/51

        //// n
        obj.push_numbers(); //28/51

        //// o
        obj.push_offlineBonus(); //29/51

        //// p
        obj.push_party(); //30/51
        obj.push_portrait(); //31/51
        obj.push_purchaseHouseRecords(); //32/51
        obj.push_piggy(); //33/51

        //// s
        obj.push_ship(); //34/51
        obj.push_shopItemDatas(); //35/51
        obj.push_shoppingMallInfo(); //36/51
        obj.push_statistics(); //37/51
        obj.push_star(); //38/51
        obj.push_skinVoucher(); //39/51
        obj.push_subscribe(); //40/51
        obj.push_skin(); //41/51

        //// t
        obj.push_task(); //42/51
        obj.push_totalGameTimeMs(); //43/51
        obj.push_totalLoginTimes(); //44/51
        obj.push_town(); //45/51
        obj.push_turnTable(); //46/51
        obj.push_tutorial(); //47/51

        //// u
        obj.push_userID(); //48/51
        obj.push_userName(); //49/51

        //#endregion autoInsertPlayer <<<<<<<<自动生成区域结束

        // 注意索引被后面使用
        obj.fields.push("createTime");
        obj.values.push(this.server.gameScript.getTime());
        var object valueTypes = {};
        valueTypes[obj.fields.length - 1] = MyDBValueType.DateTime;

        var queryStr = this.createInsertQueryStr(player, obj.fields, obj.values);
        if (queryStr == null) {
            return MyResponse.create(ECode.Error);
        }

        MsgDBQuery msg = {
            queryStr: queryStr,
            values: obj.values,
            valueTypes: valueTypes,
            expectedAffectedRows: 1,
        };

        MyResponse r = yield this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg);
        return r;
    }

    *insertPayiOSYield(int playerId, string env, int id, string productId, string bundleId, int quantity, string transactionId, string originalTransactionId, int purchaseDateMs, int expiresDateMs) {
        var queryStr = "INSERT INTO payios (playerId,env,id,productId,bundleId,quantity,transactionId,originalTransactionId,purchaseDate,expiresDate) VALUES (?,?,?,?,?,?,?,?,?,?)";
        var object values[] = [playerId, env, id, productId, bundleId, quantity, transactionId, originalTransactionId, purchaseDateMs, expiresDateMs];
        MsgDBQuery msg = {
            queryStr: queryStr,
            values: values,
            expectedAffectedRows: 1,
        };
        msg.valueTypes = {};
        msg.valueTypes[values.length - 2] = MyDBValueType.DateTime;
        msg.valueTypes[values.length - 1] = MyDBValueType.DateTime;
        MyResponse r = yield this.server.baseScript.sendYield(this.server.baseData.dbPlayerSocket, MsgType.DBQuery, msg);
        return r;
    }

    private newHelpObject(PMPlayerInfo player) {
        var obj = new PMSqlHelpObject();
        obj.player = player;
        obj.fields = [];
        obj.values = [];
        return obj;
    }

    save(PMPlayerInfo player, fun: (obj: PMSqlHelpObject) => void) {
        var obj = this.newHelpObject(player);
        fun(obj);
        this.saveFieldBatch(player, obj.fields, obj.values);
    }

    *saveYield(PMPlayerInfo player, fun: (obj: PMSqlHelpObject) => void) {
        var obj = this.newHelpObject(player);
        fun(obj);
        return yield this.saveFieldBatchYield(player, obj.fields, obj.values);
    }

    beginSave(PMPlayerInfo player): PMSqlHelpObject {
        var obj = this.newHelpObject(player);
        return obj;
    }
    endSave(obj: PMSqlHelpObject) {
        this.saveFieldBatch(obj.player, obj.fields, obj.values);
    }
    *endSaveYield(obj: PMSqlHelpObject) {
        return yield this.saveFieldBatchYield(obj.player, obj.fields, obj.values);
    }
}