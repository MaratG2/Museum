using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class State 
{
    public static bool Frozen = false;
    public static void SetCursorLock(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = isLocked ? false : true;
    }
    public static void SetCursorLock()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    public static void View(bool f)
    {
        if(f)
        {
            Frozen = true;
            SetCursorLock(false);
            Cursor.visible = true;
        }
        else
        {
            Frozen = false;
            SetCursorLock(true);
            Cursor.visible = false;
        }
    }
    public static void View()
    {
        
            Frozen = !Frozen;
            SetCursorLock();     
        
    }
}
