using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTableParser<T> where T : struct
{
    public Func<string[], T> Parse;
    [SerializeField] List<T> values;
    public List<T> Values { get { return values; } }

    public DataTableParser(Func<string[], T> Parse)
    {
        this.Parse = Parse;
        values = new List<T>();
    }

    public bool Load(in string csv)
    {
        string[] lines = csv.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            T value = Parse(lines[i].Split(','));
            values.Add(value);
        }
        return true;
    }
}
