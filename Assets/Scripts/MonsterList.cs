using UnityEngine;

[System.Serializable]
public class MList
{
    public GameObject[] list;
}

public class MonsterList : MonoBehaviour 
{
    public static MonsterList instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public MList[] monsterList;//0슬라임 1슬라임 2거북이 3나무 4박쥐 5버섯

}