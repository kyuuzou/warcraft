using System;
using UnityEngine;

[System.Serializable]
public class MapTypeTexture {

    [SerializeField]
    private Texture dungeonTexture;

    [SerializeField]
    private Texture forestTexture;

    [SerializeField]
    private Texture swampTexture;

    public Texture GetTexture(MapType mapType) {
        Texture texture = null;

        switch (mapType) {
            case MapType.Dungeon:
                texture = this.dungeonTexture;
                break;

            case MapType.Forest:
                texture = this.forestTexture;
                break;

            case MapType.Swamp:
                texture = this.swampTexture;
                break;

            default:
                throw new NotSupportedException($"Received unexpected value: {mapType}");
        }

        return texture;
    }
}
