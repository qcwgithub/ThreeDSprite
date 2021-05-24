using System.Collections.Generic;
using System;

public class SCUtils : GameScriptBase {
    string[] chars = new string[] {
        "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
        "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
    };
    public string randomString(int length) {
        var data = "";
        for (int i = 0; i < length; ++i) {
            var r = Math.floor(Math.random() * this.chars.length);
            data += this.chars[r];
        }

        return data;
    }

    /**
     * 返回将value限定于[min,max]区间
     */
    public int clamp(int value, int min, int max) {
        if (!(value > min)) {
            return min;
        }
        if (value < max) {
            return value;
        }
        return max;
    }

    public bool isValidChannelType(string channelType) {
        return channelType == HermesChannels.debug ||
            channelType == HermesChannels.uuid ||
            channelType == HermesChannels.apple ||
            channelType == HermesChannels.leiting ||
            channelType == HermesChannels.ivy;
    }
}