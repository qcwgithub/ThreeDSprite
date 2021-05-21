public class MyResponse
{
    public MyResponse(ECode e, object r = null)
    {
        this.err = e;
        this.res = r;
    }
    public MyResponse(object r)
    {
        this.err = ECode.Success;
        this.res = r;
    }
    public ECode err;
    public object res;

    public static MyResponse noResponse = new MyResponse(ECode.NoResponse, null);
    public static MyResponse exResponse = new MyResponse(ECode.Exception, null);
    public static MyResponse badReturnResponse = new MyResponse(ECode.BadReturn, null);
    public static MyResponse wrap(object r)
    {
        if (r == null)
        {
            return noResponse;
        }
        else if (r is int)
        {
            // 如果返回一个数字，则是 ECode
            return new MyResponse((ECode)r, null);
        }
        else if (r is MyResponse)
        {
            return r as MyResponse;
        }
        // else if (r.hasOwnProperty("err") && r.hasOwnProperty("res")) {
        //     return r;
        // }
        else
        {
            return badReturnResponse;
        }
    }
    public static MyResponse create(ECode e, object r = null)
    {
        //return new MyResponse(e, r);
    }

    public static implicit operator MyResponse(ECode e) => new MyResponse(e);
}