using System;
using System.Collections.Generic;
using UnityEngine;

public enum PathfindingAlgorithmType {
    None,
    AStar
}

public static class PathfindingAlgorithmExtension {

    private static Dictionary<PathfindingAlgorithmType, Type> classByType = new Dictionary<PathfindingAlgorithmType, Type> () {
        { PathfindingAlgorithmType.None, null },
        { PathfindingAlgorithmType.AStar, typeof (AStar) }
    };

    public static PathfindingAlgorithm AddToGameObject (
        this PathfindingAlgorithmType algorithm,
        GameObject gameObject
    ) {
        Type type = PathfindingAlgorithmExtension.classByType [algorithm];
        return gameObject.AddComponent (type) as PathfindingAlgorithm;
    }
}