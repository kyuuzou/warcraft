using System.Collections.Generic;

public interface ISpawnableTrait {

    bool Active { get; }

    void Activate();

    void Deactivate();

    void FilterButtons(ref List<GameButtonType> buttons);
}
