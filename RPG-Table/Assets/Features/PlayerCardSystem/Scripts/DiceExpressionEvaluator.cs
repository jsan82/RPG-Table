using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;

public class DiceExpressionEvaluator : MonoBehaviour
{
    [Header("Expression Settings")]
    [Tooltip("Example: (@str + 2d6) * @lvl")]
    
    [Header("Debug Options")]
    public bool evaluateOnStart = true;
    public bool showPostfixNotation = true;
    public bool detailedDiceLogging = true;
    
    //operators precedence
    private readonly Dictionary<char, int> precedence = new Dictionary<char, int> 
    {
        {'+', 2}, {'-', 2}, {'*', 3}, {'/', 3}, {'^', 4}, {'d', 5}
    };

    private static DiceExpressionEvaluator _instance;
    
    public static DiceExpressionEvaluator Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("DiceEvaluator");
                _instance = go.AddComponent<DiceExpressionEvaluator>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public class DiceRoll
    {
        public int DiceCount;
        public int DiceSides;
        public List<int> Rolls;
        public int Total => Rolls.Sum();

        
        public DiceRoll(int count, int sides)
        {
            DiceCount = count;
            DiceSides = sides;
            Rolls = new List<int>();
        }

        
        public override string ToString()
        {
            return $"{DiceCount}d{DiceSides}: {string.Join(" + ", Rolls)} = {Total}";
        }
    }


    //Method to evaluate the expression
    public int EvaluateAndLog(string infixExpression)
    {
        try
        {   
            string postfix = InfixToPostfix(infixExpression);
            if (showPostfixNotation) Debug.Log($"Postfix: {postfix}");
            
            //list to store dice rolls for detailed logging
            List<DiceRoll> diceRolls = new List<DiceRoll>();
            int result = EvaluatePostfix(postfix, diceRolls);
            
            //Log the detailed dice rolls if enabled
            if (detailedDiceLogging && diceRolls.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("=== Detailed Dice Rolls ===");
                foreach (var roll in diceRolls)
                {
                    sb.AppendLine(roll.ToString());
                }
                sb.AppendLine($"Final result: {result}");
                Debug.Log(sb.ToString());
            }
            
            Debug.Log($"Result of '{infixExpression}': {result}");
            return result;
        }
        catch (Exception e)
        {
            Debug.LogError($"Evaluation error: {e.Message}");
            return 0;
        }
    }

//Method to 
    private string InfixToPostfix(string infix)
    {
        
        var output = new Queue<string>();
        var operators = new Stack<char>();
        
        for (int i = 0; i < infix.Length; i++)
        {
            char c = infix[i];
            
            if( char.IsWhiteSpace(c)){

               continue; // Skip whitespace 
            }
            else if (c == '@')// Handle attribute references 
            {
                string attribute = "";
                i++; // Skip @
                while (i < infix.Length && char.IsLetter(infix[i]))
                {   

                    attribute += infix[i];
                    i++;
                }
                output.Enqueue("@" + attribute);
                i--; // Adjust index after loop
            }
            else if (char.IsDigit(c))// Handle numbers
            {              
                string number = "";
                while (i < infix.Length && char.IsDigit(infix[i]))
                {
                    number += infix[i];
                    i++;
                }
                output.Enqueue(number);
                i--;
            }// handle operators
            else if (c == 'd' && (i == 0 || infix[i-1] == 'd' || IsOperator(infix[i-1]) || infix[i-1] == '(' || infix[i-1] == '@'|| infix[i-1] == ' ' )) 
            {
                operators.Push(c);
            }
            else if (IsOperator(c))
            {
                while (operators.Count > 0 && operators.Peek() != '(' &&
                       ((precedence[c] < precedence[operators.Peek()]) ||
                        (precedence[c] == precedence[operators.Peek()] && IsLeftAssociative(c))))
                {
                    output.Enqueue(operators.Pop().ToString());
                }
                operators.Push(c);
            }
            else if (c == '(')
            {
                operators.Push(c);
            }
            else if (c == ')')
            {
                while (operators.Count > 0 && operators.Peek() != '(')
                {
                    output.Enqueue(operators.Pop().ToString());
                }
                
                if (operators.Count == 0) throw new ArgumentException("Mismatched parentheses");
                operators.Pop();
            }
        }
        // Handle any remaining operators
        while (operators.Count > 0)
        {
            if (operators.Peek() == '(') throw new ArgumentException("Mismatched parentheses");
            output.Enqueue(operators.Pop().ToString());
        }
        
        return string.Join(" ", output);
    }

    //Method to evaluate the postfix expression
    private int EvaluatePostfix(string postfix, List<DiceRoll> diceRolls)
    {
        var stack = new Stack<int>();
        var tokens = postfix.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        System.Random random = new System.Random();
        
        // Loop through each token in the postfix expression
        foreach (string token in tokens)
        {
            if (token.StartsWith("@"))// Handle attribute reference
            {
                string attributeName = token.Substring(1);
                int attributeValue = GetAttributeValue(attributeName);
                stack.Push(attributeValue);
            }
            else if (int.TryParse(token, out int number)) //hanlde numbers 
            {
                stack.Push(number);
            }
            else if (token == "d") //handle dice rolls
            {
                if (stack.Count < 2) throw new ArgumentException("Invalid dice expression");
                int sides = stack.Pop();
                int count = stack.Pop();
                
                if (count < 1 || sides < 1) throw new ArgumentException("Dice values must be positive");
                
                DiceRoll diceRoll = new DiceRoll(count, sides);
                for (int i = 0; i < count; i++)
                {
                    int roll = random.Next(1, sides + 1);
                    diceRoll.Rolls.Add(roll);
                }
                diceRolls.Add(diceRoll);
                stack.Push(diceRoll.Total);
            }
            else if (token.Length == 1 && IsOperator(token[0])) // handle operators
            {
                if (stack.Count < 2) throw new ArgumentException("Invalid expression");
                int b = stack.Pop();
                int a = stack.Pop();
                
                switch (token[0])
                {
                    case '+': stack.Push(a + b); break;
                    case '-': stack.Push(a - b); break;
                    case '*': stack.Push(a * b); break;
                    case '/': stack.Push(a / b); break;
                    case '^': stack.Push((int)Math.Pow(a, b)); break;
                    default: throw new ArgumentException($"Unknown operator: {token}");
                }
            }
            else
            {
                throw new ArgumentException($"Invalid token: {token}");
            }
        }
        
        if (stack.Count != 1) throw new ArgumentException("Invalid expression");
        return stack.Pop();
    }
    
    //Method to get the attribute value from the objectID
    private int GetAttributeValue(string objectID)
    {
        if (string.IsNullOrEmpty(objectID))
        {
            Debug.LogError("No objectID provided for attribute reference");
            return 0;
        }

        GameObject obj = ObjectID.GetObjectByID(objectID);
        if (obj == null)
        {
            Debug.LogError($"Object with ID {objectID} not found");
            return 0;
        }
        
        return obj.GetComponent<TMP_InputField>().text == "" ? 1 : int.Parse(obj.GetComponent<TMP_InputField>().text);

        

        Debug.LogError($"No CharacterAttributes component found on {objectID}");
        return 0;
    }

    //Method to check if the character is an operator
    private bool IsOperator(char c) => precedence.ContainsKey(c);
    
    //Method to check if the operator is left associative
    private bool IsLeftAssociative(char op) => op != '^' && op != 'd';
}
