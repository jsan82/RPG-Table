using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enumeration defining all available sound effect types in the game.
/// </summary>
public enum SFXType
{
    /// <summary>
    /// Sound played when UI buttons are clicked
    /// </summary>
    BUTTON_CLICK = 0,
    
    /// <summary>
    /// Sound played when a game pawn is picked up/grabbed
    /// </summary>
    PAWN_GRAB = 1,
    
    /// <summary>
    /// Sound played when a game pawn is placed/dropped
    /// </summary>
    PAWN_PLACE = 2,
    
    /// <summary>
    /// Sound played when dice are rolled
    /// </summary>
    DICE_ROLL = 3,
    
    /// <summary>
    /// Background music theme for menu screens
    /// </summary>
    MENU_THEME = 4
}