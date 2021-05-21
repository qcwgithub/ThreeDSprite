// 对应数据表 hermes_player.payivy
public class SqlTablePayIvy {
    public string orderId;
    public int playerId;
    public int id;
    public string productId;
    public int quantity;
    public string fen;
    public PayIvyState state;
    public int got; // 0未领取，1已领取
    public int createTime;
    public int notifyTime;
    public int gotTime;
}