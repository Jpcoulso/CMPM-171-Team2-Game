using UnityEngine;

// ScriptableObject that defines a unit class and its 4 abilities.
// Create new classes via Assets > Create > Unit Class Data in the Unity editor.
// To add a new class: just create another SO asset and fill in the fields.
[CreateAssetMenu(fileName = "NewUnitClass", menuName = "Unit Class Data")]
public class UnitClassData : ScriptableObject
{
    [Header("Class Info")]
    public string className = "Default";
    public Sprite classIcon; // drag unit portrait here in Inspector

    [Header("Q Ability")]
    public string qName = "Ability Q";
    [Range(1f, 10f)] public float qCooldown = 3f;
    public Sprite qIcon; // drag ability icon here

    [Header("W Ability")]
    public string wName = "Ability W";
    [Range(1f, 10f)] public float wCooldown = 5f;
    public Sprite wIcon;

    [Header("E Ability")]
    public string eAbilityName = "Ability E";
    [Range(1f, 10f)] public float eCooldown = 7f;
    public Sprite eIcon;

    [Header("R Ability (Ultimate)")]
    public string rName = "Ability R";
    public float rCooldown = 60f;
    public Sprite rIcon;

    // Alias for consistency — the field is named eAbilityName to avoid
    // clashing with Unity's built-in 'name' property in the inspector.
    public string eName => eAbilityName;
}
