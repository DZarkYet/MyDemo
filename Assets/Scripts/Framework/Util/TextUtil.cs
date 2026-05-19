using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 用于处理字符串的一些公共功能
/// </summary>
public class TextUtil
{

    #region 字符串拆分相关
    /// <summary>
    /// 拆分字符串 返回字符串数组
    /// </summary>
    /// <param name="str">被拆分的字符串</param>
    /// <param name="type">拆分字符类型: 1-; 2-, 3-% 4-: 5-空格 6-| 7-_  </param>
    /// <returns></returns>
    public static string[] SplitStr(string str, int type = 1)
    {
        if (str == null)
            return new string[0];
        string newStr = str;
        switch (type)
        {
            case 1:
                //为了避免英文符号填成了中文符号 我们先进行一个替换
                while (newStr.IndexOf("；") != -1)
                    newStr = newStr.Replace("；", ";");
                return newStr.Split(';');
            case 2:
                //为了避免英文符号填成了中文符号 我们先进行一个替换
                while (newStr.IndexOf("，") != -1)
                    newStr = newStr.Replace("，", ",");
                return newStr.Split(',');
            case 3:
                return newStr.Split('%');
            case 4:
                //为了避免英文符号填成了中文符号 我们先进行一个替换
                while (newStr.IndexOf("：") != -1)
                    newStr = newStr.Replace("：", ":");
                return newStr.Split(':');
            case 5:
                return newStr.Split(' ');
            case 6:
                return newStr.Split('|');
            case 7:
                return newStr.Split('_');
            default:
                return new string[0];
        }
    }

    /// <summary>
    /// 拆分字符串 返回整型数组
    /// </summary>
    /// <param name="str">被拆分的字符串</param>
    /// <param name="type">拆分字符类型: 1-; 2-, 3-% 4-: 5-空格 6-| 7-_  </param>
    /// <returns></returns>
    public static int[] SplitStrToIntArr(string str, int type = 1)
    {
        //得到拆分后的字符串数组
        string[] strs = SplitStr(str, type);
        if (strs.Length == 0)
            return new int[0];
        //把字符串数组转换成int数组
        return Array.ConvertAll<string, int>(strs, (str) =>
        {
            return int.Parse(str);
        });
    }

    /// <summary>
    /// 专门用来拆分多组键值对形式的数据的 以int返回
    /// </summary>
    /// <param name="str">待拆分的字符串</param>
    /// <param name="typeOne">组间分隔符 1-; 2-, 3-% 4-: 5-空格 6-| 7-_  </param>
    /// <param name="typeTwo">组内键值对分隔符 1-; 2-, 3-% 4-: 5-空格 6-| 7-_  </param>
    /// <param name="callBack">回调函数</param>
    public static void SplitStrToIntTwice(string str, int typeOne, int typeTwo, UnityAction<int, int> callBack)
    {
        string[] strs = SplitStr(str, typeOne);
        if (strs.Length == 0)
            return;
        int[] ints;
        for (int i = 0; i < strs.Length; i++)
        {
            //拆分单个道具的id和数量信息
            ints = SplitStrToIntArr(strs[i], typeTwo);
            if (ints.Length == 0)
                return;
            callBack.Invoke(ints[0], ints[1]);
        }
    }

    /// <summary>
    /// 专门用来拆分多组键值对形式的数据的 以string返回
    /// </summary>
    /// <param name="str">待拆分的字符串</param>
    /// <param name="typeOne">组间分隔符 1-; 2-, 3-% 4-: 5-空格 6-| 7-_  </param>
    /// <param name="typeTwo">组内键值对分隔符 1-; 2-, 3-% 4-: 5-空格 6-| 7-_  </param>
    /// <param name="callBack">回调函数</param>
    public static void SplitStrTwice(string str, int typeOne, int typeTwo, UnityAction<string, string> callBack)
    {
        string[] strs = SplitStr(str, typeOne);
        if (strs.Length == 0)
            return;
        string[] strs2;
        for (int i = 0; i < strs.Length; i++)
        {
            //拆分单个道具的id和数量信息
            strs2 = SplitStr(strs[i], typeTwo);
            if (strs2.Length == 0)
                return;
            callBack.Invoke(strs2[0], strs2[1]);
        }
    }
    #endregion

    #region 数字转字符串相关
    /// <summary>
    /// 得到指定长度的数值转字符串内容，如果长度不够会在前面补0，如果长度超出则会保留原始数值
    /// </summary>
    /// <param name="value">数值</param>
    /// <param name="len">长度</param>
    /// <returns></returns>
    public static string GetNumStr(int value, int len)
    {
        //ToString中传入一个Dn数字的字符串
        //代表想要将数字转换为长度为n的字符串
        //如果长度不够 会在前面补0
        return value.ToString($"D{len}");
    }

    /// <summary>
    /// 让指定浮点数保留小数点后n位
    /// </summary>
    /// <param name="value">具体的浮点数</param>
    /// <param name="len">保留小数点后n位</param>
    /// <returns></returns>
    public static string GetDecimalStr(float value, int len)
    {
        //ToString中传入一个Fn数字的字符串
        //代表想要保留小数点后几位小数
        return value.ToString($"F{len}");
    }

    /// <summary>
    /// 将较大较长的数转换为字符串
    /// </summary>
    /// <param name="num">具体数值</param>
    /// <returns>n亿n千万 n万n千</returns>
    public static string GetBigDataToStr(int num)
    {
        //如果大于1亿 显示n亿n千万
        if(num >= 100000000)
        {
            return BigDataChange(num, 100000000, "亿", "千万");
        }
        //如果大于1万 显示n万n千
        else if(num >= 10000)
        {
            return BigDataChange(num, 10000, "万", "千");
        }
        //都不满足 显示数字本身
        else
            return num.ToString();
    }

    /// <summary>
    /// 大数据转换成对应的字符串拼接
    /// </summary>
    /// <param name="num">数值</param>
    /// <param name="company">分割单位 可以填 100000000 10000</param>
    /// <param name="bigCompany">大单位 亿 万</param>
    /// <param name="littleCompany">小单位 万 千</param>
    /// <returns></returns>
    private static string BigDataChange(int num,int company,string bigCompany,string littleCompany)
    {
        resultStr.Clear();
        //有几亿 几万
        resultStr.Append(num / company);
        resultStr.Append(bigCompany);
        //看有几千万 几千
        int tmpNum = num % company;
        tmpNum /= (company / 10);
        //算出来不为0
        if (tmpNum != 0)
        {
            resultStr.Append(tmpNum);
            resultStr.Append(littleCompany);
        }
        return resultStr.ToString();
    }

    #endregion

    #region 时间转换相关
    private static StringBuilder resultStr = new StringBuilder("");

    /// <summary>
    /// 秒转时分秒格式 其中时分秒可以自己传
    /// </summary>
    /// <param name="s">秒数</param>
    /// <param name="igZero">是否忽略0</param>
    /// <param name="isKeepLen">是否保留至少两位</param>
    /// <param name="hourStr">小时拼接字符</param>
    /// <param name="minuteStr">分钟拼接字符</param>
    /// <param name="secondStr">秒拼接字符</param>
    /// <returns></returns>
    public static string SecontToHMS(int s, bool igZero = false, bool isKeepLen = false, 
        string hourStr = "时", string minuteStr = "分", string secondStr = "秒")
    {
        //时间不会有负数 如果发现是负数 直接归零
        if (s < 0) 
            s = 0;
        //计算小时
        int hour = s / 3600;
        //计算分钟
        //除去小时后的剩余秒
        int second = s % 3600;
        //剩余秒转为分钟数
        int minute = second / 60;
        //计算秒
        second = s % 60;
        //拼接
        resultStr.Clear();
        //如果小时不为0 或者 不忽略0
        if(hour!=0 || !igZero)
        {
            resultStr.Append(isKeepLen ? GetNumStr(hour, 2) : hour);//具体几个小时
            resultStr.Append(hourStr);//时
        }
        //如果分钟不为0 或者 不忽略0 或者 小时不为0
        if (minute != 0 || !igZero || hour != 0) 
        {
            resultStr.Append(isKeepLen ? GetNumStr(minute, 2) : minute);//具体几分钟
            resultStr.Append(minuteStr);//分
        }
        //如果分钟不为0 或者 不忽略0 或者 小时和分钟不为0
        if (second != 0 || !igZero || hour != 0 || minute != 0) 
        {
            resultStr.Append(isKeepLen ? GetNumStr(second, 2) : second);//具体是多少秒
            resultStr.Append(secondStr);//秒
        }

        //如果传入的参数是0秒时
        if(resultStr.Length == 0)
        {
            resultStr.Append(0);
            resultStr.Append(secondStr);
        }

        return resultStr.ToString();
    }

    /// <summary>
    /// 秒转00:00:00格式
    /// </summary>
    /// <param name="s"></param>
    /// <param name="igZero"></param>
    /// <returns></returns>
    public static string SecondToHMS2(int s, bool igZero = false)
    {
        return SecontToHMS(s, igZero, true, ":", ":", "");
    }


    #endregion

}
