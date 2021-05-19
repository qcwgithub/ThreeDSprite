
// https://wiki.g-bits.com/pages/viewpage.action?pageId=559682334
public class AAALoginResponseData {
    string sid;            // 游戏账号
    int registerTime;   // 游戏账号注册时间，时间戳格式
    int isGuest;        // 游客标识 0：非游客 1：游客
    string idCard;         // 身份证md5，未实名时用sid md5代替
    int age;            // 年龄（未实名认证、游客年龄值为0）
    int auth;           // 实名认证防沉迷状态 0：已实名认证未成年 1：已实名认证成年人 2：未实名认证    
}