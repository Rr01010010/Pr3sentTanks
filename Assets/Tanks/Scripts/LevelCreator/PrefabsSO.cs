using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "TanksPrefabs",menuName = "ScriptableObjects/TanksPrefabs")]
public class PrefabsSO : ScriptableObject
{
    public List<Transform> Prefabs = new List<Transform>();
}
