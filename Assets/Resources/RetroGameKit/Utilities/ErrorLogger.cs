using UnityEngine;
using System.Collections.Generic;
using System;

public class ErrorLogger{

    public static void Log(string errorMessage, string tip){
        throw new Exception(
                "-= RetroGameKit Error =-\n\n" +
                errorMessage + "\n\n" +
                "Tip: " + tip + "\n\n" +
                "-----------------\n\n"
        );
    }

}
