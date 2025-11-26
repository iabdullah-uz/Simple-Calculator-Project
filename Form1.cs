using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Calculator
{
    public partial class CalculatorForm : Form
    {
        private StringBuilder inputBuilder = new StringBuilder();
        private DataTable table = new DataTable();

        public CalculatorForm()
        {
            InitializeComponent();
        }

        private void Buttons_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            string btnText = clickedButton.Text;

            if (btnText == "AC")
            {
                ClearAll();
            }
            else if (btnText == "C")
            {
                Backspace();
            }
            else if (btnText == "=")
            {
                CalculateResult();
            }
            else
            {
                AddInput(btnText);
            }
        }

        private void ClearAll()
        {
            inputBuilder.Clear();
            UpdateDisplay();
        }

        private void Backspace()
        {
            if (inputBuilder.Length > 0)
            {
                inputBuilder.Remove(inputBuilder.Length - 1, 1);
                UpdateDisplay();
            }
        }

        private void CalculateResult()
        {
            try
            {
                if (inputBuilder.Length == 0)
                    return;

                string expression = inputBuilder.ToString()
                    .Replace("x", "*")
                    .Replace("÷", "/");

                var result = table.Compute(expression, null);
                inputBuilder.Clear();
                inputBuilder.Append(result);
                UpdateDisplay();
            }
            catch
            {
                label1.Text = "Error";
                inputBuilder.Clear();
            }
        }

        private void AddInput(string input)
        {
            if (string.IsNullOrEmpty(input))
                return;

            char lastChar = inputBuilder.Length > 0 ? inputBuilder[inputBuilder.Length - 1] : '\0';

            if (IsOperator(input))
            {
                // Prevent starting with an operator other than minus
                if (inputBuilder.Length == 0 && input != "-")
                    return;

                // Prevent two operators in a row
                if (inputBuilder.Length == 0)
                {
                    // First char is minus is allowed
                    if (input == "-")
                    {
                        inputBuilder.Append(input);
                        UpdateDisplay();
                    }
                    return;
                }

                if (IsOperator(lastChar.ToString()))
                {
                    // Replace last operator with the new one (allows correction)
                    inputBuilder[inputBuilder.Length - 1] = input[0];
                    UpdateDisplay();
                    return;
                }
            }
            else if (input == ".")
            {
                if (!CanAddDecimal())
                    return;
            }

            // For digits or allowed operators, just append
            inputBuilder.Append(input);
            UpdateDisplay();
        }

        private bool IsOperator(string s)
        {
            return s == "+" || s == "-" || s == "x" || s == "÷" || s == "*" || s == "/";
        }

        private bool CanAddDecimal()
        {
            // Check current number segment for a decimal point already present
            // We'll scan backward until we hit an operator or start of string
            int i = inputBuilder.Length - 1;
            while (i >= 0 && !IsOperator(inputBuilder[i].ToString()))
            {
                if (inputBuilder[i] == '.')
                    return false;
                i--;
            }
            return true;
        }

        private void UpdateDisplay()
        {
            label1.Text = inputBuilder.ToString();
        }
    }
}
