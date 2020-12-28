using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimingParameters
{
    //threshold percentage which determines hit/miss
    public static float threshold = 0.2f;

    //value which determines the smallest hittable timeUnit (e.g. 4 = Fourths, 8 = Eights, etc.)
    public static int smallestUnit = 4;
}
