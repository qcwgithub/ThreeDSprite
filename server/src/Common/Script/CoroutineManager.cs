using System;
using System.Collections;

public class EnumeratorInfo {
    public async Task<MyResponse> ie;
    public Action onFinish;
    public EnumeratorInfo parent;
}

public class CoroutineManager : IScript {
    public Server server { get; set; }
    // public Dictionary<int, EnumeratorInfo> enumerators { get { return this.server.baseData.enumerators; } }

    public void iterate(async Task<MyResponse> ie, Action onFinish)
    {
        this.iterate(new EnumeratorInfo { ie = ie, onFinish = onFinish, parent = null});
    }

    public void iterate(EnumeratorInfo info) {
        //try {
            if (!info.ie.MoveNext())
            {
                var onFinish = info.onFinish;
                info.onFinish = null;
                if (onFinish != null)
                {
                    onFinish();
                }

                var parent = info.parent;
                info.parent = null;
                if (parent != null)
                {
                    this.iterate(parent);
                }
                return;
            }

            var yieldObj = info.ie.Current as iYieldObject;
            if (yieldObj != null) {
                yieldObj.setCallback((MyResponse r) => 
                {
                    this.iterate(info);
                });
                return;
            }

            var ie = info.ie.Current as async Task<MyResponse>;
            if (ie != null)
            {
                var info2 = new EnumeratorInfo();
                info2.ie = ie;
                info2.onFinish = null;
                info2.parent = info;
                this.iterate(info2);
                return;
            }
            
            // error~!!
            throw new Exception("unknown r = await object");
        // }
        // catch (Exception ex) {

        // }
    }
}