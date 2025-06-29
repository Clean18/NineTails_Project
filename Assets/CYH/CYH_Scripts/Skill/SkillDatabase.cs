using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDatabase : MonoBehaviour
{
    [SerializeField] private GameObject[] allSkills; 
    private Dictionary<string, GameObject> skillDic;
}