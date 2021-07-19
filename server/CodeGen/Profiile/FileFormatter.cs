using System;
using System.Collections;
using System.Collections.Generic;

public class FileFormatter
{
    static string[] stab = new string[] {
    "",
    "    ",
    "        ",
    "            ",
    "                ",
    "                    ",
    "                        ",
    "                            ",
    "                                ",
    "                                    ",
    "                                        ",
    "                                            ",
    "                                                ",
    "                                                    ",
    };

    List<string> buffer = new List<string>();

    int tab = 0;
    public FileFormatter AddTab(int c = 1)
    {
        this.tab += c;
        return this;
    }
    public FileFormatter PushTab()
    {
        if (this.tab >= stab.Length) throw new Exception("tab index out of range");
        this.Push(stab[this.tab]);
        return this;
    }

    public FileFormatter Push(params string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            this.buffer.Add(args[i]);
        }
        return this;
    }
    public FileFormatter PushLine(string s = "")
    {
        this.Push(s);
        this.Push("\n");
        return this;
    }

    public string GetString()
    {
        string str = string.Join(null, this.buffer);
        this.buffer.Clear();
        return str;
    }
}