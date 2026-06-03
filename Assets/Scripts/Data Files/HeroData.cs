using System.Collections.Generic;
using UnityEngine;

//Outline for what a HeroData.asset looks like, values will be filled in when creating a specific hero.
// Use: right click in Assets folder -> Create -> Scriptable Objects -> HeroData
// name it something like Paladin.asset
//  open file and fill in specifc values for the Paladin character in the inspector
[CreateAssetMenu(fileName = "NewHero", menuName = "RPG/HeroData")]
public class HeroData : ScriptableObject
{
    [Header("Identity")]
    public string heroName;
    public Sprite portrait;
    public GameObject heroPrefab; // drag the hero prefab here
    public Color spriteTint = Color.white; // tint the sprite to differentiate classes
    public bool isRanged;
    public bool isHealer;

    [Header("Base Stats")]
    public float maxHealth;
    public float attackDamage;
    public float attackRange;
    public float attackRate;
    public float moveSpeed;
    public float armor;
    public GameObject projectilePrefab;

    [Header("Abilities")]
    public List<AbilityData> abilities;
    [Header("Q Ability")]
    public string qName = "Ability Q";
    [Range(1f, 10f)] public float qCooldown = 3f;
    public Sprite qIcon; // drag ability icon here

}
