using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataSO", menuName = "ScriptableObject/PlayerData", order = 0)]
public class PlayerDataSO : ScriptableObject
{
    [Header("ª˘¥°…Ë÷√")]
    public string characterName = "Player";
    public float maxHP = 300f;
    public float normalSpeed = 5f;
    public float sprintSpeed = 8f;
    public float attackPower = 10f;
    public float jumpForce = 8f;

    [Header("∑≠πˆ/…¡±‹")]
    public float dodgeDuration = 0.5f;
    public float dodgeInvincibleTime = 0.3f;
    public float dodgeDistance = 2f;

    [Header("¡¨ª˜≈‰÷√")]
    public int maxComboCount = 5;
    public int comboWindow = 1;
    public int swordWindow = 3;

}
