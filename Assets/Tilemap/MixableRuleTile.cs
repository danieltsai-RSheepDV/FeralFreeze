using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class MixableRuleTile : RuleTile<MixableRuleTile.Neighbor> {
    
    public TileBase[] mixables;

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.This: return CheckMixable(tile);
            case Neighbor.NotThis: return !CheckMixable(tile);
        }
        return base.RuleMatch(neighbor, tile);
    }

    bool CheckMixable(TileBase tile)
    {
        return tile != null && (tile == this || mixables.Contains(tile));
    }
}