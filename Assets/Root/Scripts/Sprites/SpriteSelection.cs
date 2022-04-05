using UnityEngine;
using System.Collections;

public class SpriteSelection : MonoBehaviour {

    [SerializeField]
    private Transform[] bounds;
    
    [SerializeField]
    private Material greenOutline;
    
    [SerializeField]
    private Material greyOutline;
    
    [SerializeField]
    private Material redOutline;

    private Material overrideMaterial;

    private bool selected = false;
    private bool visible = false;
    private bool invalid = false;
    
    //Ignore visibility and selection toggles
    private bool locked = false;
    
    private void InitializeBound (int index, Vector3 localScale, float multiplier, Vector3 offset) {
        Transform bound = this.bounds[index];

        bound.localScale = localScale;
        
        Vector3 localPosition = bound.localPosition;
        localPosition *= multiplier;
        localPosition += offset;
        bound.localPosition = localPosition;
    }
    
    public void InitializeSelection (Transform parent, Vector2Int tileSize, Material material = null) {
        this.transform.parent = parent;
        this.transform.SetLocalPosition (0.0f, 0.0f, -10.0f);
        
        Vector3 horizontalScale = new Vector3 (tileSize.x, 1.0f, 1.0f);
        Vector3 verticalScale = new Vector3 (1.0f, tileSize.y, 1.0f);
        Vector3 diagonalScale = new Vector3 (
            Mathf.Sqrt (tileSize.x * tileSize.x + tileSize.y * tileSize.y) - 0.1f,
            1.0f,
            1.0f
        );
        
        this.InitializeBound (0, horizontalScale, tileSize.y, new Vector3 ( 0.0f, -1.0f, 0.0f));
        this.InitializeBound (1, verticalScale,   tileSize.x, new Vector3 (-1.0f,  0.0f, 0.0f));
        this.InitializeBound (2, horizontalScale, tileSize.y, new Vector3 ( 0.0f,  1.0f, 0.0f));
        this.InitializeBound (3, verticalScale,   tileSize.x, new Vector3 ( 1.0f,  0.0f, 0.0f));
        this.InitializeBound (4, diagonalScale,   1.0f, Vector3.zero);
        this.InitializeBound (5, diagonalScale,   1.0f, Vector3.zero);

        if (material != null) {
            this.overrideMaterial = Material.Instantiate<Material> (material);
        }
    }

    public bool IsVisible () {
        return this.visible;
    }

    public void SetColor (Color color) {
        this.overrideMaterial.color = color;
    }

    public void SetInvalid (bool invalid) {
        if (invalid && ! this.invalid) {
            foreach (Transform bound in this.bounds) {
                bound.GetComponent<Renderer>().material = this.redOutline;
            }
        } else if (!invalid && this.invalid) {
            foreach (Transform bound in this.bounds) {
                bound.GetComponent<Renderer>().material = this.greyOutline;
            }
        }
        
        this.bounds[4].GetComponent<Renderer>().enabled = invalid;
        this.bounds[5].GetComponent<Renderer>().enabled = invalid;
        
        this.invalid = invalid;
    }
    
    public void SetLocked (bool locked) {
        this.locked = locked;
    }
    
    public void SetSelected (bool selected, bool overrideLock = false) {
        if (! this.locked || overrideLock) {
            this.selected = selected;
        }
    }
    
    public void SetVisible (bool visible, bool overrideLock = false) {
        if (! this.locked || overrideLock) {
            this.visible = visible;
            
            Material material;

            if (this.overrideMaterial == null) {
                material = this.selected ? this.greenOutline : this.greyOutline;
            } else {
                material = this.overrideMaterial;
            }

            int maximum = 4;

            if (! visible) {
                maximum = this.bounds.Length;
            }

            for (int i = 0; i < maximum; i++) {
                Transform bound = this.bounds[i];
                
                bound.GetComponent<Renderer>().material = material;
                bound.GetComponent<Renderer>().enabled = visible;
            }
        }
    }
}
