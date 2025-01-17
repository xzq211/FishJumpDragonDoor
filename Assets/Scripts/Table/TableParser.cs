﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using UnityEngine;

public static class TableParser {
    static void ParsePropertyValue<T>(T obj, FieldInfo fieldInfo, string valueStr) {
        System.Object value = valueStr;
        if (fieldInfo.FieldType.IsEnum)
            value = Enum.Parse(fieldInfo.FieldType, valueStr);
        else {
            if (fieldInfo.FieldType == typeof(int))
                value = int.Parse(valueStr);
            else if (fieldInfo.FieldType == typeof(byte))
                value = byte.Parse(valueStr);
            else if (fieldInfo.FieldType == typeof(SmartInt))
                value = new SmartInt(int.Parse(valueStr));
            else if (fieldInfo.FieldType == typeof(float))
                value = float.Parse(valueStr);
            else if (fieldInfo.FieldType == typeof(double))
                value = double.Parse(valueStr);
            else if (fieldInfo.FieldType == typeof(List<int>))
                value = valueStr.GetIntArray();
            else if (fieldInfo.FieldType == typeof(List<string>))
                value = valueStr.GetStringArray();
            else {
                if (valueStr.Contains("\"\""))
                    valueStr = valueStr.Replace("\"\"", "\"");

                // process the excel string.
                if (valueStr.Length > 2 && valueStr[0] == '\"' && valueStr[valueStr.Length - 1] == '\"')
                    valueStr = valueStr.Substring(1, valueStr.Length - 2);

                value = valueStr;
            }
        }

        if (value == null)
            return;

        fieldInfo.SetValue(obj, value);
    }

    static T ParseObject<T>(string[] lines, int idx, Dictionary<int, FieldInfo> propertyInfos) {
        T obj = Activator.CreateInstance<T>();
        string line = lines[idx];
        string[] values = line.Split('\t');
        foreach (KeyValuePair<int, FieldInfo> pair in propertyInfos) {
            if (pair.Key >= values.Length)
                continue;

            string value = values[pair.Key];
            if (string.IsNullOrEmpty(value))
                continue;

            try {
                ParsePropertyValue(obj, pair.Value, value);
            } catch (Exception ex) {
                Debug.LogError(string.Format("ParseError: Row={0} Column={1} Name={2} Want={3} Get={4}",
                    idx + 1,
                    pair.Key + 1,
                    pair.Value.Name,
                    pair.Value.FieldType.Name,
                    value));
                throw ex;
            }
        }
        return obj;
    }

    static Dictionary<int, FieldInfo> GetPropertyInfos<T>(string memberLine) {
        Type objType = typeof(T);

        string[] members = memberLine.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        Dictionary<int, FieldInfo> propertyInfos = new Dictionary<int, FieldInfo>();
        for (int i = 0; i < members.Length; i++) {
            FieldInfo fieldInfo = objType.GetField(members[i]);
            if (fieldInfo == null)
                continue;
            propertyInfos[i] = fieldInfo;
        }

        return propertyInfos;
    }

    // parse a data array from the table data.
    //static T[] LoadTable<T>(string name)
    //{
    //    TextAsset textAsset = UpdateHelper.Instance.GetAsset("Tables.unity3d", name);
    //    MemoryStream stream = new MemoryStream(textAsset.bytes);
    //    return ProtoBuf.Serializer.Deserialize<T[]>(stream);
    //}

    public static T[] Parse<T>(string name) {
        //    #if !UNITY_EDITOR
        //        // we load from the server tables.
        //        if (MainScript.Instance != null && 
        //            MainScript.Instance.CheckUpdate && 
        //            UpdateHelper.Instance != null)
        //            return LoadTable<T>(name);
        //    #endif

        // here we load the text asset.
#if !UNITY_UPDATE_LOAD
        TextAsset textAsset = (TextAsset)Resources.Load(name);
#else
		object u3dObject = resource.XDownloadManager.DownLoadResource(name,false);
		TextAsset textAsset = u3dObject as TextAsset;
#endif
        if (textAsset == null) {
            Debug.LogWarning("无法加载表格文件：" + name);
            return null;
        }

        // try parse the table lines.


        string[] lines = textAsset.text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 3) {
            Debug.LogError("表格文件行数错误，【1】属性名称【2】变量名称【3-...】值：" + name);
            return null;
        }

        // fetch all of the field infos.
        Dictionary<int, FieldInfo> propertyInfos = GetPropertyInfos<T>(lines[1]);

        // parse it one by one.
        T[] array = new T[lines.Length - 2];
        for (int i = 0; i < lines.Length - 2; i++)
            array[i] = ParseObject<T>(lines, i + 2, propertyInfos);

        return array;
    }
}
