using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player {

    //private Map map;

    protected override void Start() {
        //this.map = ServiceLocator.Instance.Map;

        this.StartCoroutine(this.Think());
    }

    private IEnumerator Think() {
        yield return new WaitForSeconds(1.0f);
        List<Unit> attackers = this.Factions[0].GetUnits();
        List<SpawnableSpriteGroup> movingGroups = new List<SpawnableSpriteGroup>();

        int unitCount = attackers.Count;

        if (unitCount == 0) {
            yield break;
        }

        Direction direction = Direction.South;

        foreach (Unit attacker in attackers) {
            if (movingGroups.Contains(attacker.Group)) {
                continue;
            }

            movingGroups.Add(attacker.Group);

            if (attacker.Group == null) {
                continue;
            }
        }
    }
}
