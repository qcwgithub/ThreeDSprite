using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CLoadingPanel : MonoBehaviour
{
    public Text Message;

    public class OwnerInfo
    {
        public OwnerInfo(int leftMs, Text message)
        {
            this.leftMs = leftMs;
            this.text = "";
            this.message = message;
            this.color = Color.black;
        }

        public int leftMs;
        string text;
        Text message;
        public void setMessage(string text)
        {
            this.text = text;
            this.message.text = text;
        }
        Color color;
        public void setColor(Color color)
        {
            this.color = color;
            this.message.color = color;
        }

        public void restore()
        {
            this.message.text = this.text;
            this.message.color = this.color;
        }
    }

    private Dictionary<string, OwnerInfo> owners = new Dictionary<string, OwnerInfo>();
    public OwnerInfo show(string reason, int timeMs = 10000)
    {
        var info = this.owners[reason] = new OwnerInfo(timeMs, Message);
        this.gameObject.SetActive(true);
        return info;
    }

    public void hide(string reason)
    {
        if (!this.owners.ContainsKey(reason))
        {
            return;
        }
        this.owners.Remove(reason);
        if (this.owners.Count == 0)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            foreach (var kv in this.owners)
            {
                kv.Value.restore();
                break;
            }
        }
    }

    List<string> temp1 = new List<string>();
    List<string> temp2 = new List<string>();
    void update()
    {
        if (this.owners.Count == 0)
        {
            return;
        }

        this.temp1.Clear();
        foreach (var kv in this.owners)
        {
            this.temp1.Add(kv.Key);
        }

        this.temp2.Clear();
        int dt = (int)Time.deltaTime * 1000;
        foreach (var key in this.temp1)
        {
            var info = this.owners[key];
            if (info.leftMs == -1)
            {
                continue;
            }
            if (info.leftMs > dt)
            {
                info.leftMs -= dt;
            }
            else
            {
                this.temp2.Add(key);
            }
        }
        foreach (var key in this.temp2)
        {
            this.hide(key);
        }
    }
}
