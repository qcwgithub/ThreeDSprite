public class LoginAAAFailAdvice
{
    public bool retry;
    public int retryMs;
    public bool fallbackToUuid;
    public bool clearInterfaceCache;
    public bool logout;
}

public class LoginPMFailAdvice
{
    public bool retryPM;
    public int retryPMMs;
    public bool retryAAA;
    public int retryAAAMs;
    public bool logout;
}

public class LoginBMFailAdvice
{
    public bool retryBM;
    public int retryBMMs;
    public bool logout;
}