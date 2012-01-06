using System;
using System.Collections.Generic;
using System.Text;

namespace Shiro.Interpreter
{
    public class Scanner
    {
        public string[] keywords = { "if", "to", "for", "in", "else", "print", "while", "try", "catch", "where", "class",
                                        "finally", "throw", "select", "option", "break", "twice", "thrice",
                                        "convert", "def", "return", "use", "run", "new", "is", "then" };

        protected string[] operators = { "+", "-", "*", "/", "^", "=", "!", "%", "&", "|", "?", "~", "@", ":" };
        protected string[] conditionalOperators = { ">", "<", "==", "!=", "<>", ">=", "<=" };
        protected string[] opsToRecombine = { "==", "!=", "<>", "<=", ">=", ":=", "=?", "??" };
        protected string[] compoundAssigners = { "+=", "-=", "*=", "/=", "&=", "|=" };
        protected string[] symbols = { ":", ".", ",", "(", ")", "[", "]", "{", "}", ";" };

        protected bool IsAName(string valIn)
        {
            string val = valIn.ToLower();
            char first = val[0];

            if (valIn == "true" || valIn == "false")
                return false;

            if ((first >= 'a' && first <= 'z') || (first == '_'))
                for (int i = 1; i < val.Length; i++)
                {
                    if ((val[i] >= 'a' && val[i] <= 'z') ||
                            (val[i] >= '0' && val[i] <= '9') ||
                            (val[i] == '_'))
                        continue;
                    else
                        return false;
                }
            else
                return false;

            return true;
        }

        protected List<Token> AssignTokenProperties(List<Token> tokens)
        {
            List<Token> ret = new List<Token>();

            List<string> workKW = new List<string>(keywords);
            List<string> workOp = new List<string>(operators);
            List<string> workCOp = new List<string>(conditionalOperators);
            List<string> workSym = new List<string>(symbols);

            int tokenScope = 0;

            foreach (Token constToke in tokens)
            {
                Token token = constToke;

                if (token.token == "{" && token.tt != TokenType.Value)
                    tokenScope++;
                else if (token.token == "}" && token.tt != TokenType.Value)
                    tokenScope--;

                //Assign a tt
                if (token.vt == ValueType.String)
                    token.tt = TokenType.Value;
                else if (workKW.Contains(token.token))
                    token.tt = TokenType.Keyword;
                else if (workOp.Contains(token.token))
                    token.tt = TokenType.Operator;
                else if (workCOp.Contains(token.token))
                    token.tt = TokenType.CondOp;
                else if (workSym.Contains(token.token))
                    token.tt = TokenType.Symbol;
                else if (IsAName(token.token))
                    token.tt = TokenType.Name;
                else
                {
                    double l;
                    long l2;
                    token.tt = TokenType.Value;

                    //Assign VT
                    if (double.TryParse(token.token, out l))
                        token.vt = ValueType.Number;
                    else if (long.TryParse(token.token, out l2))
                        token.vt = ValueType.Number;
                    else if (token.token == "true" || token.token == "false")
                        token.vt = ValueType.Bool;
                    else
                        token.vt = ValueType.String;
                }

                token.scope = tokenScope;
                ret.Add(token);
            }

            return ret;
        }

        protected List<Token> CombineDualOperators(List<Token> tokens)
        {
            List<Token> ret = new List<Token>();
            List<string> recOps = new List<string>(opsToRecombine);
            List<string> compAs = new List<string>(compoundAssigners);

            for (int i = 0; i < tokens.Count; i++)
            {
                if ((tokens[i].tt == TokenType.CondOp) || (tokens[i].tt == TokenType.Operator) || (tokens[i].token == ":"))     //Last condition is for :=
                {
                    if (tokens.Count <= i + 1)
                    {
                        ret.Add(tokens[i]);
                        continue;
                    }
                    string checkThis = tokens[i].token + tokens[i + 1].token;
                    if (recOps.Contains(checkThis))
                    {
                        i += 1;
                        Token newToke = new Token();
                        newToke.token = checkThis;
                        newToke.tt = TokenType.CondOp;
                        ret.Add(newToke);
                        continue;
                    }
                    else if (compAs.Contains(checkThis))
                    {
                        // x _= y  scans to x = x _ y
                        string theOperator = checkThis[0].ToString();
                        
                        i += 1;
                        Token operatorToken = new Token();
                        operatorToken.token = theOperator;
                        operatorToken.tt = TokenType.Operator;
                        Token nameToken = ret[ret.Count - 1].Clone();
                        Token eqToken = tokens[i].Clone();

                        ret.Add(eqToken);
                        ret.Add(nameToken);
                        ret.Add(operatorToken);
                    }
                    else
                    {
                        ret.Add(tokens[i]);
                        continue;
                    }
                }
                else if (tokens[i].token == "." && tokens.Count >= (i+2) && tokens[i + 1].token == "." && tokens[i + 2].token == ".")
                {
                    // ... construct (ie: def myFunction(...))
                    i += 2;
                    Token newToke = new Token();
                    newToke.token = "...";
                    newToke.tt = TokenType.Symbol;
                    ret.Add(newToke);
                    continue;
                }
                else
                {
                    ret.Add(tokens[i]);
                    continue;
                }
            }

            return ret;
        }

        public List<Token> Scan(List<Token> tokens)
        {
            List<Token> ret;

            ret = AssignTokenProperties(tokens);
            ret = CombineDualOperators(ret);

            return ret;
        }
    }
}
