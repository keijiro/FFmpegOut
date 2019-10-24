using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class SetTimeNow : MonoBehaviour
{
    public Text timeText;

    void Update()
    {
        timeText.text = DateTime.UtcNow.ToString(
            "HH:mm:ss.fff",
            CultureInfo.InvariantCulture );
    }
}
