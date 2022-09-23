using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planet : pawn
{
    private bool[,] slots = new bool[3,3];
    public bool generateSettlement(int cordX, int cordY) {
        if((cordX <= 2 && cordX >=0) && (cordX <= 2 && cordX >=0) && !slots[cordX,cordY]) {
            return true;
        }
        else{return false;}
    }
}