
public abstract partial class SpawnableSprite {

    public virtual void PressCancel() {
        this.ContextMenu.ManualUpdate();
        //this.ContextMenu.SetButtons (this.Data.Buttons);
    }
}
