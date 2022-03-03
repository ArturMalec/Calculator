using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalcButton : MonoBehaviour
{
    [SerializeField] Text _Sign;
    [SerializeField] bool _InputAllowedButton;
    [SerializeField] Image _Mask;
    [SerializeField] CalculatorManager.calcModes _CalcMode;

    public string Sign
    {
        get
        {
            return _Sign.text;
        }
    }

    public bool InputAllowedButton
    {
        get
        {
            return _InputAllowedButton;
        }
    }

    public CalculatorManager.calcModes CalcMode
    {
        get
        {
            return _CalcMode;
        }
    }

    public void DisableButton(bool state)
    {
        if (state)
        {
            GetComponent<Button>().interactable = false;
            _Mask.enabled = true;
        }
        else
        {
            GetComponent<Button>().interactable = true;
            _Mask.enabled = false;
        }
    }
}
