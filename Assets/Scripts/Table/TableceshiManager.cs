using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class Tableceshi : ITableItem {

    public int Id;
    public int HH;
    public string Name;


    public int Key() { return Id; }
}

public class TableceshiManager : TableManager<Tableceshi, TableceshiManager>
{
    public override string TableName() { return "Tableceshi"; }
}

