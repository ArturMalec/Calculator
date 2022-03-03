using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculatorManager : MonoBehaviour
{
    [SerializeField] InputField _MainInput;
    [SerializeField] Text _CalculationsText;
    [SerializeField] Image _ErrorImage;
    [SerializeField] List<CalcButton> _CalcButtons;

    public enum calcModes 
    {
        none = 0, 
        plus = 1, 
        minus = 2, 
        multiply = 3, 
        division = 4
    }

    #region PrivateVariables

    private calcModes calculationMode;
    private CalcButton calcButton;
    private string input;
    private double firstNumber;
    private double secondNumber;
    private bool isOperation = false;
    private bool isAfterEquation = false;
    private bool operationMode = false;
    private bool isReadyForResult = false;
    private bool isCommaUsed = false;
    private bool isTrimmingBlocked = false;
    private bool isPercentUsed = false;
    private bool isSpecialOperationUsed = false;

    #endregion

    public void GetInputAndPushToInputField(int id)
    {
        calcButton = _CalcButtons[id];
        input = calcButton.Sign;

        if (calcButton.InputAllowedButton)
        {
            if ((_MainInput.text == 0.ToString() && input != ",") || isOperation)
            {
                if (_MainInput.text != 0.ToString() && input == ",")
                {
                    _MainInput.text = 0.ToString() + input;
                    isCommaUsed = true;
                }
                else
                {
                    _MainInput.text = input;
                }

                
                isOperation = false;
            }
            else
            {
                if (input == "," && !isCommaUsed)
                {
                    _MainInput.text += input;
                    isCommaUsed = true;
                }
                else if (input != ",")
                {
                    _MainInput.text += input;
                }
               
            }
        }
    }

    /// <summary>
    /// Plus, Minus, Division, Multiply
    /// </summary>
    public void BasicOperations()
    {
        isCommaUsed = false;
        if (operationMode)
        {

            if (!isReadyForResult || isOperation)
            {
                calculationMode = calcButton.CalcMode;             
                _CalculationsText.text = firstNumber.ToString() + " " + GetSign(calculationMode);              
            }
            else
            {               
                isOperation = true;
                secondNumber = double.Parse(_MainInput.text);
                double result = Calculations(firstNumber, secondNumber, calculationMode);
                calculationMode = calcButton.CalcMode;
                if (IsDivisionByZero(result))
                {
                    _MainInput.text = "0 division!";
                    _CalculationsText.text = "";
                }
                else
                {
                    _MainInput.text = result.ToString();
                    _CalculationsText.text = result.ToString() + " " + GetSign(calculationMode);
                }
                firstNumber = result;
            }          
        }
        else
        {
            
            _CalculationsText.text = "";
            isReadyForResult = false;
            if (isAfterEquation)
            {
                _CalculationsText.text = "";
                isAfterEquation = false;
            }
            if (calcButton.CalcMode != calcModes.none)
            {
                calculationMode = calcButton.CalcMode;
            }
            isOperation = true;
            firstNumber = double.Parse(_MainInput.text);
            _CalculationsText.text += firstNumber.ToString() + " " + GetSign(calculationMode);
            _MainInput.text = firstNumber.ToString();
            operationMode = true;
            StartCoroutine(WaitForDigit());
        }
        isPercentUsed = false;
        isSpecialOperationUsed = false;
    }

    IEnumerator WaitForDigit()
    {
        isTrimmingBlocked = true;
        yield return new WaitUntil(() => calcButton.InputAllowedButton);
        isReadyForResult = true;
        isTrimmingBlocked = false;
    }

    public void Equation()
    {
        if (isAfterEquation)
        {
            double num = double.Parse(_MainInput.text);
            double result = Calculations(double.Parse(_MainInput.text), secondNumber, calculationMode);
            _CalculationsText.text = num.ToString() + " " + GetSign(calculationMode) + " " + secondNumber.ToString() + " =";
            if (IsDivisionByZero(result))
                _MainInput.text = "0 division!";
            else
                _MainInput.text = result.ToString();
        }
        else
        {
            secondNumber = double.Parse(_MainInput.text);
            double result = Calculations(firstNumber, secondNumber, calculationMode);

            if (calculationMode == calcModes.none)
            {
                _MainInput.text = secondNumber.ToString();
                _CalculationsText.text = " " + secondNumber.ToString() + " =";
                isAfterEquation = false;
            }
            else
            {
                if (IsDivisionByZero(result))
                {
                    _MainInput.text = "0 division!";
                    _CalculationsText.text = "";
                }
                else
                {
                    _MainInput.text = result.ToString();
                    if (!isPercentUsed && !isTrimmingBlocked)
                        _CalculationsText.text += " " + secondNumber.ToString() + " =";
                    else
                        _CalculationsText.text +=  " =";
                }
                
                isAfterEquation = true;
            }
        }
        operationMode = false;
        isSpecialOperationUsed = false;
        isPercentUsed = false;
    }

    public void ClearInput()
    {
        _MainInput.text = 0.ToString();
        _CalculationsText.text = "";
        isOperation = false;
        operationMode = false;
        isAfterEquation = false;
        isCommaUsed = false;
        isPercentUsed = false;
        isTrimmingBlocked = false;
        isSpecialOperationUsed = false;
        calculationMode = calcModes.none;
        firstNumber = 0;
        secondNumber = 0;
        BlockButtonsAfterFalseDivision(false);
    }

    public void ClearInputDigit()
    {
        _MainInput.text = 0.ToString();
    }

    public void TrimLastCharacter()
    {
        if (!isTrimmingBlocked)
        {
            if (!isAfterEquation)
            {
                if (_MainInput.text[_MainInput.text.Length - 1] == ',')
                {
                    isCommaUsed = false;
                }
                _MainInput.text = _MainInput.text.Remove(_MainInput.text.Length - 1);
                if (_MainInput.text == "-")
                {
                    _MainInput.text = 0.ToString();
                }
            }
            else
            {
                _CalculationsText.text = "";
            }          
        }
       
        if (string.IsNullOrEmpty(_MainInput.text))
        {
            _MainInput.text = 0.ToString();
        }
    }

    public void PowerOperation()
    {
        double num = double.Parse(_MainInput.text);
        double finalNumber = Mathf.Pow(float.Parse(_MainInput.text), 2);
        _MainInput.text = finalNumber.ToString();

        if (!operationMode || isSpecialOperationUsed)
            _CalculationsText.text = "sqr(" + num.ToString() + ")";
        else
            _CalculationsText.text += " sqr(" + num.ToString() + ")";

        isTrimmingBlocked = true;
        isSpecialOperationUsed = true;
    }

    public void SqrRootOperation()
    {
        double num = double.Parse(_MainInput.text);

        if (num >= 0)
        {
            double finalNumber = Mathf.Sqrt(float.Parse(_MainInput.text));
            _MainInput.text = finalNumber.ToString();

            if (!operationMode || isSpecialOperationUsed)
                _CalculationsText.text = "√" + num.ToString();
            else
                _CalculationsText.text += " √" + num.ToString();
        }
        else
        {
            _MainInput.text = "Bad input";
            BlockButtonsAfterFalseDivision(true);
        }
        
        isTrimmingBlocked = true;
        isSpecialOperationUsed = true;
    }

    public void OneByNumberOperation()
    {
        double num = double.Parse(_MainInput.text);
        double finalNumber = 1 / double.Parse(_MainInput.text);

        if (IsDivisionByZero(finalNumber))
        {
            _MainInput.text = "0 division!";
        }
        else
        {
            _MainInput.text = finalNumber.ToString();

            if (!operationMode || isSpecialOperationUsed)
                _CalculationsText.text = "1/(" + num.ToString() + ")";
            else
                _CalculationsText.text += " 1/(" + num.ToString() + ")";
        }

        isTrimmingBlocked = true;
        isSpecialOperationUsed = true;
    }

    public void PercentOperation()
    {
        double finalNumber;

        if (calculationMode == calcModes.none)
        {
            finalNumber = 0d;
            _MainInput.text = finalNumber.ToString();
            _CalculationsText.text = finalNumber.ToString();
        }
        else if (calculationMode == calcModes.plus || calculationMode == calcModes.minus)
        {
            finalNumber = firstNumber * (double.Parse(_MainInput.text) / 100);
            _MainInput.text = finalNumber.ToString();
            _CalculationsText.text = firstNumber + " " + GetSign(calculationMode) + " " + finalNumber.ToString();
        }
        else if (calculationMode == calcModes.multiply || calculationMode == calcModes.division)
        {
            finalNumber = double.Parse(_MainInput.text);
            finalNumber /= 100;
            _MainInput.text = finalNumber.ToString();
            _CalculationsText.text = firstNumber + " " + GetSign(calculationMode) + " " + finalNumber.ToString();
        }

        isPercentUsed = true;
        isTrimmingBlocked = true;
    }

    public void NegateOperation()
    {
        if (_MainInput.text[_MainInput.text.Length-1] == ',')
        {
            if (!_MainInput.text.Contains("-"))
                _MainInput.text = "-" + _MainInput.text;
            else
                _MainInput.text = _MainInput.text.Replace("-", "");
        }
        else
        {
            double number = double.Parse(_MainInput.text);
            number *= -1;
            _MainInput.text = number.ToString();
        }
    }

    private bool IsDivisionByZero(double result)
    {
        if (double.IsInfinity(result))
        {
            BlockButtonsAfterFalseDivision(true);
            return true;
        }
        
        return false;
    }

    private void BlockButtonsAfterFalseDivision(bool state)
    {
        for (int i = 0; i < _CalcButtons.Count; i++)
        {
            if (_CalcButtons[i].Sign != "C")
            {
                _CalcButtons[i].DisableButton(state);
            }
        }

        if (state)
            _ErrorImage.enabled = true;
        else
            _ErrorImage.enabled = false;
    }

    private double Calculations(double first, double second, calcModes mode)
    {
        switch (mode)
        {
            case calcModes.division:
                return first / second;
            case calcModes.multiply:
                return first * second;
            case calcModes.minus:
                return first - second;
            case calcModes.plus:
                return first + second;
            default:
                return -1;
        }
    }

    private string GetSign(calcModes mode)
    {
        switch (mode)
        {
            case calcModes.division:
                return "÷";
            case calcModes.multiply:
                return "x";
            case calcModes.minus:
                return "-";
            case calcModes.plus:
                return "+";
            default:
                return "e";
        }
    }






}
