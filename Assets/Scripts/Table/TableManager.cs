using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITableItem
{
    int Key();
}

public interface ITableManager
{
    string TableName();
    object TableData { get; }
}

static class TableHelp
{
    public static List<int> GetIntArray(this string item)
    {
        var props = new List<int>();

        if (item == null) {
            return props;
        }

        foreach (string s in item.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
        {
            int p;
            if (int.TryParse(s, out p))
            {
                props.Add(p);
            }
        }

        return props;
    }

    public static List<float> GetFloatArray(this string item) {
        var props = new List<float>();

        if (item == null) {
            return props;
        }

        foreach (string s in item.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)) {
            float p;
            if (float.TryParse(s, out p)) {
                props.Add(p);
            }
        }

        return props;
    }

    public static List<int> GetIntArray(this int item)
    {
        return new List<int>() { item };
    }

    public static List<string> GetStringArray(this string item)
    {
        if (string.IsNullOrEmpty(item)) {
            return new List<string>();
        }

        return new List<string>(item.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
    }

    public static string GetStringProp(this Dictionary<string, string> item, string key)
    {
        if (item.ContainsKey(key))
        {
            return item[key];
        }

        return string.Empty;
    }

    public static int GetIntProp(this Dictionary<string, string> item, string key)
    {
        if (item.ContainsKey(key))
        {
            int prop;
            int.TryParse(item[key], out prop);
            return prop;
        }
        return 0;
    }

    public static List<int> GetIntArray(this Dictionary<string, string> item, string key)
    {
        if (item.ContainsKey(key))
        {
            var props = new List<int>();

            foreach (string s in item[key].Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                int p;
                if (int.TryParse(s, out p))
                {
                    props.Add(p);
                }
            }

            return props;
        }
        return new List<int>();
    }

    public static List<string> GetStringArray(this Dictionary<string, string> item, string key)
    {
        if (item.ContainsKey(key))
        {
            return new List<string>(item[key].Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
        }
        return new List<string>();
    }
}

   

public abstract class TableManager<T, U> : Singleton<U>, ITableManager where T : ITableItem
{
    // abstract functions need tobe implements.
    public abstract string TableName();
    public object TableData { get { return mItemArray; } }

    // the data arrays.
    T[] mItemArray;
    protected Dictionary<int, int> mKeyItemMap = new Dictionary<int, int>();

    // constructor.
    internal TableManager()
    {
        // load from excel txt file.
        mItemArray = TableParser.Parse<T>(TableName());

        // build the key-value map.
        for (int i = 0; i < mItemArray.Length; i++)
			mKeyItemMap[mItemArray[i].Key()] = i;
    }
	
	public void ReloadTableInFile(string file)
	{
		 // reload from excel txt file.
		string fileName;
		if(file == "")
			fileName = TableName();
		else
			fileName = file + "/" + TableName();
		
		mItemArray = TableParser.Parse<T>(fileName);
		if(mItemArray != null )
		{
	        // build the key-value map.
	        for (int i = 0; i < mItemArray.Length; i++)
				mKeyItemMap[mItemArray[i].Key()] = i;
		}
	}

    // get a item base the key.
    public virtual T GetItem(int key)
    {
        int itemIndex;
        if (mKeyItemMap.TryGetValue(key, out itemIndex))
            return mItemArray[itemIndex];

        Debug.LogWarning(key + " not exists");

        return default(T);
    }

    public virtual T GetItemWithOutWarm(int key) {
        int itemIndex;
        if (mKeyItemMap.TryGetValue(key, out itemIndex))
            return mItemArray[itemIndex];
        
        return default(T);
    }
	
    // get the item array.
	public T[] GetAllItem()
	{
		return mItemArray;
	}
}

public class MultiLanguagesTables : Singleton<MultiLanguagesTables>
{
    //List<ITableManager> mTableList ;
    public MultiLanguagesTables()
    {
    }
	private void Init()
	{
//		mTableList.Add(LanguagesManager.Instance);
//		mTableList.Add(ItemBaseManager.Instance);
//		mTableList.Add(LevelSliderManager.Instance);
//		mTableList.Add(LoadingTipsManager.Instance);
//		mTableList.Add(PvpBaseManager.Instance);
//		mTableList.Add(QuestBaseManager.Instance);
//		mTableList.Add(RoleCreateNameManager.Instance);
//		mTableList.Add(RoleSkillClassManager.Instance);
//		mTableList.Add(SceneListManager.Instance);
//		mTableList.Add(SkillAttribManager.Instance);
//		mTableList.Add(UnitBaseManager.Instance);
		
	}
	public void ReloadTableInFile(string file)
	{
//        LanguagesManager.Instance.ReloadTableInFile(file);
//        ItemBaseManager.Instance.ReloadTableInFile(file);
//        LevelSliderManager.Instance.ReloadTableInFile(file);
//        LoadingTipsManager.Instance.ReloadTableInFile(file);
//        PvpBaseManager.Instance.ReloadTableInFile(file);
////		QuestBaseManager.Instance.ReloadTableInFile(file);
//        RoleCreateNameManager.Instance.ReloadTableInFile(file);
//        SceneListManager.Instance.ReloadTableInFile(file);
//        SkillAttribManager.Instance.ReloadTableInFile(file);
//        UnitBaseManager.Instance.ReloadTableInFile(file);
	}
}
