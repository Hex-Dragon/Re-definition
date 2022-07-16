using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputM {
    
    public static string keyLeft = "a", keyRight = "d", keyJump = "w", keyCrouch = "s", keyFire = "l", keyReload = "r";
    public static bool GetKeyUI(KeyType key) => key switch {
        KeyType.Left => keyLeft == "l" ? Input.GetMouseButton(0) : (keyLeft == "r" ? Input.GetMouseButton(1) : Input.GetKey(keyLeft)),
        KeyType.Right => keyRight == "l" ? Input.GetMouseButton(0) : (keyRight == "r" ? Input.GetMouseButton(1) : Input.GetKey(keyRight)),
        KeyType.Jump => keyJump == "l" ? Input.GetMouseButton(0) : (keyJump == "r" ? Input.GetMouseButton(1) : Input.GetKey(keyJump)),
        KeyType.Crouch => keyCrouch == "l" ? Input.GetMouseButton(0) : (keyCrouch == "r" ? Input.GetMouseButton(1) : Input.GetKey(keyCrouch)),
        KeyType.Fire => keyFire == "l" ? Input.GetMouseButton(0) : (keyFire == "r" ? Input.GetMouseButton(1) : Input.GetKey(keyFire)),
        _ => keyReload == "l" ? Input.GetMouseButton(0) : (keyReload == "r" ? Input.GetMouseButton(1) : Input.GetKey(keyReload)),
    };
    public static bool GetKeyEvent(KeyType key) => key switch {
        KeyType.Left => keyLeft == "l" ? Input.GetMouseButton(0) : (keyLeft == "r" ? Input.GetMouseButton(1) : Input.GetKey(keyLeft)),
        KeyType.Right => keyRight == "l" ? Input.GetMouseButton(0) : (keyRight == "r" ? Input.GetMouseButton(1) : Input.GetKey(keyRight)),
        KeyType.Jump => keyJump == "l" ? Input.GetMouseButtonDown(0) : (keyJump == "r" ? Input.GetMouseButtonDown(1) : Input.GetKeyDown(keyJump)),
        KeyType.Crouch => keyCrouch == "l" ? Input.GetMouseButton(0) : (keyCrouch == "r" ? Input.GetMouseButton(1) : Input.GetKey(keyCrouch)),
        KeyType.Fire => keyFire == "l" ? Input.GetMouseButton(0) : (keyFire == "r" ? Input.GetMouseButton(1) : Input.GetKey(keyFire)),
        _ => keyReload == "l" ? Input.GetMouseButtonDown(0) : (keyReload == "r" ? Input.GetMouseButtonDown(1) : Input.GetKeyDown(keyReload)),
    };
    public enum KeyType { Left, Right, Jump, Crouch, Fire, Reload }

}
