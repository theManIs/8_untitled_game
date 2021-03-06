using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterInnateTraits", order = 1)]
public class CharacterInnateTraits : ScriptableObject
{
    [Header("Runtime Values")]
    public int TemporaryHealth = 25;
    public int RuntimeActionPoints = 0;

    [Header("Positioning")]
    public Vector3 PositionToInstantiate;

    [Header("Animation")]
    public float AnimationPlaySpeed = 1f;

    [Header("General")] 
    public string UnitName;
    public GameObject StartingInstance;
    public int MovementRange = 3;
    public int WeaponRange = 5;
    public Color BaseColor = Color.blue;
    public Color ActiveColor = Color.green;
    public Color HoverColor = Color.cyan;
    public float BaseDamage = 15;
    public AudioClip ShotSound;
    public int ActionPoints = 2;

    [Header("Experience")]
    public int Experience = 1;
    public int ExperiencePreviousLevel = 10;
    public int Level = 1;
    public int ExperienceNextLevel = 10;


    [Header("Primary Traits")]
    public int Strength = 10;
    public int Agility = 10;
    public int Dexterity = 10;
    public int Intelligence = 10;
    public int Vitality = 10;
    public int Luck = 10;

    [Header("Secondary Traits")]
    public int MeleeAttack = 0;
    public int PhysicalDefense = 0;
    public int Health = 2;
    public int CriticalChance = 0;
    public int MagicalAttack = 0;
    public int MagicalDefense = 0;
    public int Manna = 0;
    public int Accuracy = 0;
    public int Initiative = 0;
    public int CastingRate = 0;
    public int Dodging = 0;
    public int RangedAttack = 0;
    public int CriticalModifier = 0;

    [Header("Modifiers")]
    public int MeleeAttackModifier = 2;
    public int MeleeAttackEquation => Strength * MeleeAttackModifier;
    public int PhysicalDefenseModifier = 2;
    public int PhysicalDefenseEquation => Vitality / PhysicalDefenseModifier;
    public int HealthModifier = 2;
    public int HealthEquation => Vitality * HealthModifier;
    public int CriticalChanceModifier = 1;
    public int CriticalChanceEquation => Luck * CriticalChanceModifier;
    public int MagicalAttackModifier = 3;
    public int MagicalAttackEquation => Intelligence * MagicalAttackModifier;
    public int MagicalDefenseModifier = 2;
    public int MagicalDefenseEquation => Intelligence + Vitality / MagicalDefenseModifier;
    public int MannaModifier = 10;
    public int MannaEquation => Intelligence * MannaModifier;
    public int AccuracyModifier = 1;
    public int AccuracyEquation => Dexterity * AccuracyModifier;
    public int InitiativeModifier = 1;
    public int InitiativeEquation => Agility / InitiativeModifier;
    public int CastingRateModifier = 1;
    public int CastingRateEquation => Dexterity / CastingRateModifier;
    public int DodgingModifier = 1;
    public int DodgingEquation => Agility / DodgingModifier;
    public int RangedAttackModifier = 2;
    public int RangedAttackEquation => Dexterity * RangedAttackModifier;
    public int CriticalModifierModifier = 50;
    public int CriticalModifierEquation => Agility * CriticalModifierModifier;

    public int GetExperienceNextLevel(int n)
    {
        int b = 5;
        int result = 1;

        for (int i = 0; i < n; i++)
        {
            if (i % 3 == 0)
            {
                b += b / 2;
            }

            result += b;
        }

        return result;
    }
    public float CountDistance(Vector3 activePlayer, Vector3 capsule)
    {
        return Mathf.Abs(capsule.x - activePlayer.x) + Mathf.Abs(capsule.z - activePlayer.z);
    }

    public bool CheckDistance(Vector3 activePlayer, Vector3 capsule)
    {
        return CountDistance(activePlayer, capsule) <= WeaponRange;
    }

    public bool IsEnemy(CharacterInnateTraits innateTraits)
    {
        return BaseColor != innateTraits.BaseColor;
    }

    public int GetCriticalDamage()
    {
        return Convert.ToInt32(RangedAttack * CriticalModifier / 100f);
    }

    public bool IsCriticalDamage()
    {
        return Random.Range(0, 100) <= CriticalChance;
    }

    public bool DoHit(float onlyObstacleAccuracy)
    {
        return Random.Range(0, 100) <= onlyObstacleAccuracy + Accuracy;
    }

    public float CountAccuracy(float accuracy)
    {
        return accuracy + Accuracy;
    }

    public void SubAction()
    {
//        Debug.Log("SubAction");
        if (HasAction())
        {
//            Debug.Log("RuntimeActionPoints");
            RuntimeActionPoints--;
        }
    }

    public bool HasAction()
    {
        return RuntimeActionPoints > 0;
    }

    public void Recount()
    {
//        MeleeAttack = MeleeAttackEquation;
//        PhysicalDefense = PhysicalDefenseEquation;
//        Health = HealthEquation;
//        CriticalChance = CriticalChanceEquation;
//        MagicalAttack = MagicalAttackEquation;
//        MagicalDefense = MagicalDefenseEquation;
//        Manna = MannaEquation;
//        Accuracy = AccuracyEquation;
//        Initiative = InitiativeEquation;
//        CastingRate = CastingRateEquation;
//        Dodging = DodgingEquation;
//        RangedAttack = RangedAttackEquation;
//        CriticalModifier = CriticalModifierEquation;
//        ExperiencePreviousLevel = GetExperienceNextLevel(Level);
//        ExperienceNextLevel = GetExperienceNextLevel(Level);
        TemporaryHealth = Health;
        RuntimeActionPoints = ActionPoints;
    }
    
}
