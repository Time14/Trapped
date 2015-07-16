using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// R stands for Rebindable
public static class RInput
{

	static string defaultKeyFile = "Default.key";
	static string keyFile = "Keys.key";

	//Buttons
	static Dictionary<string, KeyCode> buttonsPrim = new Dictionary<string, KeyCode>();
	static Dictionary<string, KeyCode> buttonsSec = new Dictionary<string, KeyCode>();

	//Axies
	static Dictionary<string, KeyCode> positive = new Dictionary<string, KeyCode>();
	static Dictionary<string, KeyCode> negative = new Dictionary<string, KeyCode>();

	//Array of dictionaries
	static Dictionary<string, KeyCode>[] keybindings = new Dictionary<string, KeyCode>[] {
		buttonsPrim,
		buttonsSec,
		positive,
		negative
	};

	static RInput()
	{
	}

	public static void AddButton(string name, KeyCode button1, KeyCode button2 = KeyCode.None)
	{
		if (buttonsPrim.ContainsKey(name))
		{
			buttonsPrim [name] = button1;
			buttonsSec [name] = button2;
		}
		else
		{
			buttonsPrim.Add(name, button1);
			buttonsSec.Add(name, button2);
		}
	}

	public static void AddAxis(string name, KeyCode keyIDPos, KeyCode keyIDNeg)
	{
		if (positive.ContainsKey(name))
		{
			positive [name] = keyIDPos;
			negative [name] = keyIDNeg;
		}
		else
		{
			positive.Add(name, keyIDPos);
			negative.Add(name, keyIDNeg);
		}
	}

	/*
	public static bool CheckAvailability(KeyCode key, bool setToNone = false)
	{
		foreach (Dictionary<string, KeyCode> d in keybindings)
		{
			foreach (KeyValuePair<string, KeyCode> pair in d)
			{
				if (pair.Value == key && pair.Value != KeyCode.None)
				{
					if (setToNone)
					{
						d [pair.Key] = KeyCode.None;
					} else
					{
						return false;
					}
				}
			}
		}
		return true;
	}
	*/

	public static bool GetButtonDown(string name)
	{
		KeyCode p;
		buttonsPrim.TryGetValue(name, out p);
		KeyCode s;
		buttonsSec.TryGetValue(name, out s);
		return Input.GetKeyDown(p) || Input.GetKeyDown(s);
	}

	public static bool GetButtonUp(string name)
	{
		KeyCode p;
		buttonsPrim.TryGetValue(name, out p);
		KeyCode s;
		buttonsSec.TryGetValue(name, out s);
		return Input.GetKeyUp(p) || Input.GetKeyUp(s);
	}

	public static bool GetButton(string name)
	{
		KeyCode p;
		buttonsPrim.TryGetValue(name, out p);
		KeyCode s;
		buttonsSec.TryGetValue(name, out s);
		return Input.GetKey(p) || Input.GetKey(s);
	}

	public static float GetAxisRaw(string name)
	{
		float dir = 0;
		KeyCode k;
		positive.TryGetValue(name, out k);
		dir += Input.GetKey(k) ? 1 : 0;
		negative.TryGetValue(name, out k);
		dir -= Input.GetKey(k) ? 1 : 0;
		return dir;
	}

	/*
	 * Save system
	 * ex:
	 * 1 Horizontal D A
	 * 1 : Is an axis
	 * Horizontal : name
	 * D : Posetive key / primary key
	 * A : Negative key / secondary key (Not requierd)
	 */

	public static void LoadDefaultKeys()
	{
		Load(defaultKeyFile);
	}

	public static void LoadKeys()
	{
		if (Load(keyFile))
			return;

		if (Load(defaultKeyFile))
			return;

		GenerateDefaultKeys();
		if (Load(defaultKeyFile))
			return;

	}

	public static bool Load(string path)
	{
		try
		{
			StreamReader sr = new StreamReader(path);
			string line;
			line = sr.ReadLine();
			do
			{
				string[] commands = line.Split(' ');
				if (commands [0] == "1")
				{
					RInput.AddAxis(commands [1], (KeyCode)System.Enum.Parse(typeof(KeyCode), commands [2]), (commands.Length == 4) ? (KeyCode)System.Enum.Parse(typeof(KeyCode), commands [3]) : KeyCode.None);
				} else
				{
					RInput.AddButton(commands [1], (KeyCode)System.Enum.Parse(typeof(KeyCode), commands [2]), (commands.Length == 4) ? (KeyCode)System.Enum.Parse(typeof(KeyCode), commands [3]) : KeyCode.None);
				}
				line = sr.ReadLine();
			} while (line != null);
			sr.Close();
			return true;
		} catch (System.Exception e)
		{
			Debug.LogError("Failed to load key file : " + path);
			return false;
		}
	}

	private static void GenerateDefaultKeys()
	{
		string defaultKeys = "0 LampToggle E None\n0 Walk Mouse0 None\n0 Sound Q None\n0 Jump Space None\n0 Run LeftShift None\n0 Interact R None\n0 Grapple Alpha2 None\n0 Roll Alpha1 None\n0 Quit Escape None\n1 Horizontal D A\n1 Vertical W S\n";
		StreamWriter sw = new StreamWriter(defaultKeyFile);
		sw.Write(defaultKeys);
		sw.Close();
	}

	public static void SaveKeys()
	{
		Save(keyFile);
	}

	public static void Save(string path)
	{
		StreamWriter sw = new StreamWriter(path);
		for (int i = 0; i < keybindings.Length; i+=2)
		{
			foreach (KeyValuePair<string, KeyCode> pair in keybindings[i])
			{
				string line = "";
				line += (i == 0) ? "0" : "1";
				line += " ";
				line += pair.Key;
				line += " ";
				line += pair.Value;
				line += " ";
				line += keybindings [i + 1] [pair.Key];
				sw.WriteLine(line);
			}
		}
		sw.Close();
	}
}
