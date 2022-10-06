using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextSetter : MonoBehaviour
{
	public string prefix = "";

	private Text _text;

    private void Awake()
    {
        _text = GetComponent<Text>();
    }

    public void SetValue(float value)
    {
        _text.text = prefix + value.ToString();
    }
}
