using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Logger {
	static System.IO.StreamWriter file;
	static public void Log ( object message ) {
		if (file == null )
			file = new System.IO.StreamWriter(@"/Users/eguillou/Log-Simulation.txt");
		//Debug.Log (message.ToString ());
		file.WriteLine (message.ToString ());
		file.Flush ();
	}
	static public void Flush () {
		file.Flush ();
	}
}
