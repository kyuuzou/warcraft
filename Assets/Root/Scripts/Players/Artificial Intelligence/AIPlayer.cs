using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player {

    //private Map map;

    protected override void Start () {
        //this.map = ServiceLocator.Instance.Map;

        this.StartCoroutine (this.Think ());
    }

    private IEnumerator Think () {
        yield return new WaitForSeconds (1.0f);
        List<Unit> attackers = this.Factions [0].GetUnits ();
        List<UnitGroup> movingGroups = new List<UnitGroup> ();

        int unitCount = attackers.Count;

        if (unitCount == 0) {
            yield break;
        }

        Direction direction = Direction.South;

        foreach (Unit attacker in attackers) {
            if (movingGroups.Contains (attacker.Group)) {
                continue;
            }

            movingGroups.Add (attacker.Group);

            if (attacker.Group == null) {
                continue;
            }

            /*
            Unit squadLeader = attacker.Group.Units[0];
            MapTile targetTile = null;

            for (int i = 0; i < 4; i ++) {
                direction = (direction + 2).RoundClamp ();

                if (squadLeader.Tile.GetNeighbour (direction).IsTraversable (MovementType.Land, squadLeader)) {
                    targetTile = squadLeader.Tile.GetNeighbour (direction);
                    break;
                }
            }

            if (targetTile != null) {
                squadLeader.Move (targetTile);
            }*/
        }
    }
}
