
public class SqlLog : IScript {
    public Server server { get; set; }
    public BaseData baseData { get { return this.server.baseData; } }
    public BaseScript baseScript { get { return this.server.baseScript; } }

    private doQuery(string queryStr, object values[], int expectedAffectedRows) {
        MsgDBQuery msg = new MsgDBQuery() {
            queryStr = queryStr,
            values = values,
            expectedAffectedRows = expectedAffectedRows,
        };
        this.server.netProto.send(this.baseData.dbLogSocket, MsgType.DBQuery, msg, (MyResponse r) => {
            if (r.err != ECode.Success) {
                this.server.baseScript.error("SqlLog.doQuery failed. %s, %s", ECode[r.err], queryStr);
            }
        });
    }

    player_login(PMPlayerInfo player, boolean uploadProfile) {
        var queryStr = "INSERT INTO player_login (playerId,level,uploadProfile) VALUES (?,?,?);"
        var object values[] = [player.id, player.profile.highestHouseLevel + 1, uploadProfile];
        this.doQuery(queryStr, values, 1);
    }

    player_logout(PMPlayerInfo player) {
        var queryStr = "INSERT INTO player_logout (playerId,level) VALUES (?,?);"
        var object values[] = [player.id, player.profile.highestHouseLevel + 1];
        this.doQuery(queryStr, values, 1);
    }

    player_houseLevel(PMPlayerInfo player) {
        var queryStr = "INSERT INTO player_houseLevel (playerId,level,gameTimeS) VALUES (?,?,?);"
        var object values[] = [player.id, player.profile.highestHouseLevel + 1, Math.floor(this.server.pmScript.getRealtimeTotalGameTimeMs(player) / 1000)];
        this.doQuery(queryStr, values, 1);
    }

    player_partyLevel(PMPlayerInfo player) {
        var queryStr = "INSERT INTO player_partyLevel (playerId,level,partyLevel) VALUES (?,?,?);"
        var object values[] = [player.id, player.profile.highestHouseLevel + 1, player.profile.party.levelV2];
        this.doQuery(queryStr, values, 1);
    }

    player_changeName(PMPlayerInfo player) {
        var queryStr = "INSERT INTO player_changeName (playerId,level,userName) VALUES (?,?,?);"
        var object values[] = [player.id, player.profile.highestHouseLevel + 1, player.profile.userName];
        this.doQuery(queryStr, values, 1);
    }

    player_diamond(PMPlayerInfo player, place: DiamondPlace, int delta, int i1 = 0, int i2 = 0, string s1 = null, string s2 = null) {
        var queryStr = "INSERT INTO player_diamond (playerId,place,delta,current,level,i1,i2,s1,s2) VALUES (?,?,?,?,?,?,?,?,?);"
        var object values[] = [player.id, place, delta, player.getDiamond(), player.profile.highestHouseLevel + 1, i1, i2, s1, s2];
        this.doQuery(queryStr, values, 1);
    }

    player_badge(PMPlayerInfo player, place: BadgePlace, int delta, int i1 = 0, int i2 = 0, string s1 = null, string s2 = null) {
        var queryStr = "INSERT INTO player_badge (playerId,place,delta,current,currentC,level,i1,i2,s1,s2) VALUES (?,?,?,?,?,?,?,?,?,?);"
        var object values[] = [player.id, place, delta, player.profile.badge, player.profile.badgeC, player.profile.highestHouseLevel + 1, i1, i2, s1, s2];
        this.doQuery(queryStr, values, 1);
    }

    player_giftVoucher(PMPlayerInfo player, place: GiftVoucherPlace, int delta, int i1 = 0, int i2 = 0, string s1 = null, string s2 = null) {
        var queryStr = "INSERT INTO player_giftVoucher (playerId,place,delta,current,level,i1,i2,s1,s2) VALUES (?,?,?,?,?,?,?,?,?);"
        var object values[] = [player.id, place, delta, player.profile.giftVoucher, player.profile.highestHouseLevel + 1, i1, i2, s1, s2];
        this.doQuery(queryStr, values, 1);
    }

    account_changeChannel(int playerId, string channel1, string channelUserId1, string channel2, string channelUserId2) {
        var queryStr = "INSERT INTO account_changeChannel(playerId,channel1,channelUserId1,channel2,channelUserId2) VALUES (?,?,?,?,?);";
        var object values[] = [playerId, channel1, channelUserId1, channel2, channelUserId2];
        this.doQuery(queryStr, values, 1);
    }
}