/*
using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

public static class Debug {
    
    public static void DrawLine (Vector3 start, Vector3 end) {
        UnityEngine.Debug.DrawLine (start, end);
    }
    
    public static void DrawLine (Vector3 start, Vector3 end, Color color) {
        UnityEngine.Debug.DrawLine (start, end, color);
    }
    
    public static void Log (params object[] objects) {
        UnityEngine.Debug.Log (Debug.ParseOutput (objects));
    }
    
    public static void LogError (params object[] objects) {
        UnityEngine.Debug.LogError (Debug.ParseOutput (objects));
    }
    
    public static void LogWarning (params object[] objects) {
        UnityEngine.Debug.LogWarning (Debug.ParseOutput (objects));
    }
    
    private static string ParseOutput (params object[] objects) {
        string output = string.Join (
            ":",
            Array.ConvertAll (objects, @object => (@object == null) ? "null" : @object.ToString())
        );
        
        //"GetFrame (2)" so it skips ParseOutput and whatever Utils method invoked it.
        StackFrame frame = new StackTrace (true).GetFrame (2);
        MethodBase method = frame.GetMethod ();
        
        return string.Format (
            "{0}<b><{1}.{2}:{3}></b> {4}",
            Time.timeSinceLevelLoad,
            method.ReflectedType.Name,
            method.Name,
            frame.GetFileLineNumber (),
            output
        );
    }
}
*/