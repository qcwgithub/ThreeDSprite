using System.Collections.Generic;
using System;

public class SCUtils : GameScriptBase {
    string[] chars = new string[] {
        "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
        "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
    };
    randomString(int length): string {
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
    clamp(int value, int min, int max) {
        if (!(value > min)) {
            return min;
        }
        if (value < max) {
            return value;
        }
        return max;
    }

    // 加权随机，给定一个长度，以及一个获取权重的函数，返回随机到的索引
    weightedRandom(int totalWeight, int length, getWeight: (int index) => number): number {
        var randomValue = Math.random() * totalWeight;
        var currentTotal = 0;
        for (int i = 0; i < length; i++) {
            currentTotal += getWeight(i);
            if (randomValue <= currentTotal) {
                return i;
            }
        }
        console.error("weightedRandom no value");
        return 0;
    }
    weightedRandomSimple(int length, getWeight: (int index) => number): number {
        var totalWeight = 0;
        for (int i = 0; i < length; i++) {
            totalWeight += getWeight(i);
        }

        return this.weightedRandom(totalWeight, length, getWeight);
    }
    public bool checkArgs(string s, params object[] others) {
        for (int i = 0; i < s.Length; i++) {
            object v = others[i];
            Type t = v.GetType();
            switch (s[i]) {
                case "b":
                    return v is bool;

                case "x": // big int
                    return this.checkArgs("i", v) || this.checkArgs("s", v);
                case "X": // big int
                    return this.checkArgs("I", v) || this.checkArgs("S", v);
                case "i":
                    if (t !== "number" || !(v >= 0) || Math.floor(v) !== v) {
                        return false;
                    }
                    break;
                case "I":
                    if (t !== "number" || !(v > 0) || Math.floor(v) !== v) {
                        return false;
                    }
                    break;
                case "f":
                    if (t !== "number") {
                        return false;
                    }
                    break;
                case "F":
                    if (t !== "number" || !(v > 0)) {
                        return false;
                    }
                    break;
                case "s":
                    if (t !== "string") {
                        return false;
                    }
                    break;
                case "S":
                    if (t !== "string" || v.length == 0) {
                        return false;
                    }
                    break;
                default:
                    return false;
            }
        }
        return true;
    }

    public bool isValidChannelType(string channelType) {
        return channelType == HermesChannels.debug ||
            channelType == HermesChannels.uuid ||
            channelType == HermesChannels.apple ||
            channelType == HermesChannels.leiting ||
            channelType == HermesChannels.ivy;
    }
}