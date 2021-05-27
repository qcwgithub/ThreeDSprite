public class MyResponse
{
    public MyResponse(ECode e, object r = null)
    {
        this.err = e;
        this.res = r;
    }
    public ECode err;
    public object res;

    public static MyResponse noResponse = new MyResponse(ECode.NoResponse);
    public static MyResponse exResponse = new MyResponse(ECode.Exception);
    public static MyResponse badReturnResponse = new MyResponse(ECode.BadReturn);

    public static implicit operator MyResponse(ECode e) => new MyResponse(e);
}