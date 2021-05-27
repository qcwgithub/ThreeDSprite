public class ProfileSubscribe {
    public bool subscribing;
    public int purchaseTime;
    public int expireTime;
    public int lastMs;

    static ProfileSubscribe ensure(ProfileSubscribe obj) {
        if (obj == null) {
            obj = new ProfileSubscribe();
        }
        return obj;
    }
}