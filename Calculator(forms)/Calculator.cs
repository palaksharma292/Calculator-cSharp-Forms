using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculator_forms_
{
    public partial class Calculator : Form
    {
        // Create Arraylists for input expression and arithmetic operators
        public  ArrayList expression = new ArrayList();
        public  ArrayList operators = new ArrayList();
        public Calculator()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            operators.Clear();
            operators.Add("/");
            operators.Add("*");
            operators.Add("-");
            operators.Add("+");

            expression.Clear();
        }

        private void btnEnterVal_Click(object sender, EventArgs e)
        {
            string senderText = (sender as Button).Text;
            if (TextBox.Text == "0"||TextBox.Text=="ERROR")
            {
                if (senderText == ".")
                {
                    TextBox.Text = "0.";
                }
                else if(senderText=="0")
                {
                    TextBox.Text = "0";
                }
                else if (!(senderText == "*" || senderText == "/" || senderText == "+"))
                {
                    TextBox.Text = senderText;
                }
            }
            else
            {
                TextBox.Text += senderText;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            TextBox.Text = "0";
        }


        private void btnBack_Click(object sender, EventArgs e)
        {
            
            if(TextBox.Text.Length==0)
            {
                TextBox.Text = "0";
            }
            else if(TextBox.Text=="ERROR")
            {
                TextBox.Text = "0";
            }
            else
            {
                TextBox.Text = TextBox.Text.Substring(0, TextBox.Text.Length - 1);
            }    
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            string result =ProcessCommand(TextBox.Text);
            TextBox.Text = result;
        }


        // Method for processing the input string
        public string ProcessCommand(string input)
        {
            try
            {
                // Clear expression before use
                expression.Clear();
                // Extract input string and assign resultant string to expression
                expression = Extract(input);
                while (expression.Count > 1)
                {
                    // Evaluate expression then assign the result to the expression
                    expression = Evaluate(expression);
                }

                return (expression[0].ToString());
            }
            catch
            {
                return "ERROR";
            }
        }

        // Method for extracting values from the input string
        public ArrayList Extract(string expr)
        {
            try
            {
                // Checking input expression for brackets 
                if (expr.Contains("("))
                {
                    expr = Extractforbracketts(expr);
                }
                string num = "";
                for (int i = 0; i < expr.Length; i++)
                {
                    if (expr.Substring(i, 1) == " ")
                    {
                        continue;
                    }
                    int digit = 0;
                    bool canConvert = int.TryParse(expr.Substring(i, 1), out digit);
                    if (canConvert || expr.Substring(i, 1) == ".")
                    {
                        num = num + expr.Substring(i, 1);
                    }
                    // Extract negative numbers
                    else if (expr.Substring(i, 1) == "-" && num == "")
                    {
                        while (expr.Substring(i + 1, 1) == " ")
                        {
                            i++;
                        }
                        bool canConvertNext = int.TryParse(expr.Substring(i + 1, 1), out digit);
                        if (canConvertNext)
                        {
                            if (i == 0)
                            {
                                num = "-";
                            }
                            else
                            {
                                int j = i;
                                while (expr.Substring(j - 1, 1) == " ")
                                {
                                    j--; ;
                                }
                                bool canConvertPrevious = int.TryParse(expr.Substring(j - 1, 1), out digit);
                                if (!canConvertPrevious)
                                {
                                    num = "-";
                                }
                            }
                            continue;
                        }
                    }
                    // Add final value to expression
                    else
                    {
                        if (num != "")
                        {
                            expression.Add(double.Parse(num));
                        }
                        num = "";
                        expression.Add(expr.Substring(i, 1));
                    }
                }
                // Return expression with final value
                if (num != "")
                {
                    expression.Add(double.Parse(num));
                }
                return expression;
            }
            catch
            {
                TextBox.Text = "ERROR";
                return new ArrayList();
            }
        }

        // Method for handling brackets
        public  string Extractforbracketts(string expr)
        {
            while (expr.Contains(")"))
            {
                for (int i = expr.IndexOf(")"); i >= 0; i--)
                {
                    if (expr.Substring(i, 1) == "(")
                    {
                        // Extract string between brackets from the original expression
                        string section = expr.Substring(i, expr.IndexOf(")") + 1 - i);
                        ArrayList arr = new ArrayList();
                        arr = Extract(section.Substring(1, section.Length - 2));
                        // Evaluate extracted string
                        arr = Evaluate(arr);
                        // Add multiplication operator (*) to locations where num and "(" are adjacent without an operator
                        if (expr.Substring(i, 1) == "(" && i > 0)
                        {
                            int digit;
                            bool canConvertPrevious = int.TryParse(expr.Substring(i - 1, 1), out digit);
                            if (canConvertPrevious)
                            {
                                expr = expr.Insert(i, "*");
                            }
                        }
                        if (expr.IndexOf(")") < expr.Length - 1)
                        {
                            int digit;
                            bool canConvertNext = int.TryParse(expr.Substring(expr.IndexOf(")") + 1, 1), out digit);
                            if (canConvertNext)
                            {
                                expr = expr.Insert(expr.IndexOf(")") + 1, "*");
                            }
                        }
                        // Replace original string 'section' with resultant string
                        expr = expr.Replace(section, arr[0].ToString());
                        Console.WriteLine(expr);
                        arr.Clear();
                        break;
                    }
                }
            }
            return expr;
        }

        // Method for Evaluating the expression
        public  ArrayList Evaluate(ArrayList expression)
        {
            double val = 0;
            double n1 = 0, n2 = 0;

            for (int op = 0; op < operators.Count; op++)
            {
                // Check if the expression contains any operators
                while (expression.Contains(operators[op]))
                {
                    int i = expression.IndexOf(operators[op]);
                    if (i >= 1)
                    {
                        // Select values adjacent to the selected operator
                        if (expression[i - 1].GetType() == typeof(double))
                            n1 = double.Parse(expression[i - 1].ToString());
                        if (expression[i + 1].GetType() == typeof(double))
                            n2 = double.Parse(expression[i + 1].ToString());
                        // Apply operation to adjacent values based on the selected operator
                        if (operators[op].ToString() == "+")
                        {
                            val = n1 + n2;
                        }
                        else if (operators[op].ToString() == "-")
                        {
                            val = n1 - n2;
                        }
                        else if (operators[op].ToString() == "*")
                        {
                            val = n1 * n2;
                        }
                        else if (operators[op].ToString() == "/")
                        {
                            val = n1 / n2;
                        }
                        // Insert resultant value into the expression in place of the original values and operator
                        expression[i - 1] = val;
                        expression.RemoveRange(i, 2);

                    }
                }
            }
            return expression;
        }
    }
}


