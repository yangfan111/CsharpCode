using System;
using System.Security.Cryptography;
using Sharpen;
using com.graphbuilder.struc;

namespace com.graphbuilder.math
{
    /// <summary><p>Expression string parser.</summary>
    /// <remarks>
    /// <p>Expression string parser.  The parser returns an Expression object which can be evaluated.
    /// <p>The elements of an expression fall into two categories, operators and terms.  Terms include
    /// variables, functions and values.  Although values do not subclass TermNode, they are processed
    /// in a similar way during the parsing.
    /// <p>The operators are exponent (^), times (*), divide (/), plus (+) and minus (-).  The exponent has
    /// the highest precedence, then times and divide (which have an equal level of precedence) and then
    /// plus and minus (which have an equal level of precedence).  For those operators that have an equal
    /// level of precedence, the operator that comes first (reading from left to right) gets the higher
    /// level of precedence.  For example, a-b+c becomes ((a-b)+c)
    /// <p>During the parsing, the string is scanned from left to right.  Either a term or an operator will
    /// come next.  If it is a term, then it may be preceded by an optional + or - sign.  For example,
    /// For example +a/-b/+c is valid, and becomes ((a/(-b))/c).  If a term is not explicitly signed, then
    /// by default it is positive.
    /// <p>If the first character of a term is a decimal (.) or a digit (0-9) then the term marks the
    /// beginning of a value.  The first character that is found that is <b>not</b> a digit or a decimal marks the
    /// end of the value, except if the character is an 'e' or an 'E'.  In this case, the first digit immediately
    /// following can be a plus sign, minus sign or digit.  E.g. 1.23E+4.  At this point, the first character that
    /// is not a digit marks the end of the value.
    /// <p>If the first character of a term is <b>not</b> a decimal, digit, open bracket '(', a close bracket ')', a comma ',',
    /// a whitespace character ' ', '\t', '\n', or an operator, then the character marks the beginning of a variable or
    /// function name.  The first character found that is an operator, whitespace, open bracket, close bracket or comma
    /// marks the end of the name.  If the first non-whitespace character after a name is an open bracket, then it marks
    /// the beginning of the function parameters, otherwise the term is marked as a variable.
    /// <p>Functions that accept more than one parameter will have the parameters separated by commas.  E.g.
    /// f(x,y).  Since the parameters of a function are also expressions, when a comma is detected that has
    /// exactly one non-balanced open bracket to the left, a recursive call is made passing in the substring.
    /// E.g. f(g(x,y),z), a recursive call will occur passing in the substring "g(x,y)".
    /// <p>Miscellaneous Notes
    /// <ul>
    /// <li>Names cannot begin with a decimal or a digit since those characters mark the beginning of a value.</li>
    /// <li>In general, whitespace is ignored or marks the end of a name or value.</li>
    /// <li>Evaluation of values is done using the Double.parseDouble(...) method.</li>
    /// <li>Constants can be represented as functions that take no parameters.  E.g. pi() or e()</li>
    /// <li>Round brackets '(' and ')' are the only brackets that have special meaning.</li>
    /// <li>The brackets in the expression must balance otherwise a ExpressionParseException is thrown.</li>
    /// <li>An ExpressionParseException is thrown in all cases where the expression string is invalid.</li>
    /// <li>All terms must be separated by an operator.  E.g. 2x is <b>not</b> valid, but 2*x is.</li>
    /// <li>In cases where simplification is possible, simplification is <b>not</b> done.  E.g. 2^4 is <b>not</b>
    /// simplified to 16.</li>
    /// <li>Computerized scientific notation is supported for values.  E.g. 3.125e-4</li>
    /// <li>Scoped negations are not permitted. E.g. -(a) is <b>not</b> valid, but -1*(a) is.</li>
    /// </ul>
    /// </remarks>
    /// <seealso cref="Expression"/>
    public class ExpressionTree
    {
        private ExpressionTree()
        {
        }

        /// <summary>Returns an expression-tree that represents the expression string.</summary>
        /// <remarks>Returns an expression-tree that represents the expression string.  Returns null if the string is empty.</remarks>
        /// <exception cref="ExpressionParseException">If the string is invalid.</exception>
        public static Expression Parse(string s)
        {
            if (s == null)
            {
                throw new ExpressionParseException("Expression string cannot be null.", -1);
            }

            return Build(s, 0);
        }

        private static Expression Build(string s, int indexErrorOffset)
        {
            // do not remove (required condition for functions with no parameters, e.g. Pi())
            if (s.Trim().Length == 0)
            {
                return null;
            }

            Stack s1 = Stack.Allocate();
            // contains expression nodes
            Stack s2 = Stack.Allocate();
            try
            {
                // contains open brackets ( and operators ^,*,/,+,-
                bool term = true;
                // indicates a term should come next, not an operator
                bool signed = false;
                // indicates if the current term has been signed
                bool negate = false;
                // indicates if the sign of the current term is negated
                for (int i = 0; i < s.Length; i++)
                {
                    char c = s[i];
                    if (c == ' ' || c == '\t' || c == '\n')
                    {
                        continue;
                    }

                    if (term)
                    {
                        if (c == '(')
                        {
                            if (negate)
                            {
                                throw new ExpressionParseException("Open bracket found after negate.", i);
                            }

                            s2.Push("(");
                        }
                        else
                        {
                            if (!signed && (c == '+' || c == '-'))
                            {
                                signed = true;
                                if (c == '-')
                                {
                                    negate = true;
                                }
                            }
                            else
                            {
                                // by default negate is false
                                if (c >= '0' && c <= '9' || c == '.')
                                {
                                    int j = i + 1;
                                    while (j < s.Length)
                                    {
                                        c = s[j];
                                        if (c >= '0' && c <= '9' || c == '.')
                                        {
                                            j++;
                                        }
                                        else
                                        {
                                            // code to account for "computerized scientific notation"
                                            if (c == 'e' || c == 'E')
                                            {
                                                j++;
                                                if (j < s.Length)
                                                {
                                                    c = s[j];
                                                    if (c != '+' && c != '-' && (c < '0' || c > '9'))
                                                    {
                                                        throw new ExpressionParseException(
                                                            "Expected digit, plus sign or minus sign but found: " +
                                                            c.ToString(), j + indexErrorOffset);
                                                    }

                                                    j++;
                                                }

                                                while (j < s.Length)
                                                {
                                                    c = s[j];
                                                    if (c < '0' || c > '9')
                                                    {
                                                        break;
                                                    }

                                                    j++;
                                                }

                                                break;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    double d = 0;
                                    string _d = Sharpen.Runtime.Substring(s, i, j);
                                    try
                                    {
                                        d = double.Parse(_d);
                                    }
                                    catch
                                    {
                                        throw new ExpressionParseException("Improperly formatted value: " + _d,
                                            i + indexErrorOffset);
                                    }

                                    if (negate)
                                    {
                                        d = -d;
                                    }

                                    s1.Push(new ValNode(d));
                                    i = j - 1;
                                    negate = false;
                                    term = false;
                                    signed = false;
                                }
                                else
                                {
                                    if (c != ',' && c != ')' && c != '^' && c != '*' && c != '/' && c != '+' &&
                                        c != '-')
                                    {
                                        int j = i + 1;
                                        while (j < s.Length)
                                        {
                                            c = s[j];
                                            if (c != ',' && c != ' ' && c != '\t' && c != '\n' && c != '(' &&
                                                c != ')' && c != '^' && c != '*' && c != '/' && c != '+' && c != '-')
                                            {
                                                j++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        if (j < s.Length)
                                        {
                                            int k = j;
                                            while (c == ' ' || c == '\t' || c == '\n')
                                            {
                                                k++;
                                                if (k == s.Length)
                                                {
                                                    break;
                                                }

                                                c = s[k];
                                            }

                                            if (c == '(')
                                            {
                                                FuncNode fn = new FuncNode(Sharpen.Runtime.Substring(s, i, j), negate);
                                                int b = 1;
                                                int kOld = k + 1;
                                                while (b != 0)
                                                {
                                                    k++;
                                                    if (k >= s.Length)
                                                    {
                                                        throw new ExpressionParseException(
                                                            "Missing function close bracket.", i + indexErrorOffset);
                                                    }

                                                    c = s[k];
                                                    if (c == ')')
                                                    {
                                                        b--;
                                                    }
                                                    else
                                                    {
                                                        if (c == '(')
                                                        {
                                                            b++;
                                                        }
                                                        else
                                                        {
                                                            if (c == ',' && b == 1)
                                                            {
                                                                Expression o = Build(
                                                                    Sharpen.Runtime.Substring(s, kOld, k), kOld);
                                                                if (o == null)
                                                                {
                                                                    throw new ExpressionParseException(
                                                                        "Incomplete function.",
                                                                        kOld + indexErrorOffset);
                                                                }

                                                                fn.Add(o);
                                                                kOld = k + 1;
                                                            }
                                                        }
                                                    }
                                                }

                                                Expression o_1 = Build(Sharpen.Runtime.Substring(s, kOld, k), kOld);
                                                if (o_1 == null)
                                                {
                                                    if (fn.NumChildren() > 0)
                                                    {
                                                        throw new ExpressionParseException("Incomplete function.",
                                                            kOld + indexErrorOffset);
                                                    }
                                                }
                                                else
                                                {
                                                    fn.Add(o_1);
                                                }

                                                s1.Push(fn);
                                                i = k;
                                            }
                                            else
                                            {
                                                s1.Push(new VarNode(Sharpen.Runtime.Substring(s, i, j), negate));
                                                i = k - 1;
                                            }
                                        }
                                        else
                                        {
                                            s1.Push(new VarNode(Sharpen.Runtime.Substring(s, i, j), negate));
                                            i = j - 1;
                                        }

                                        negate = false;
                                        term = false;
                                        signed = false;
                                    }
                                    else
                                    {
                                        throw new ExpressionParseException("Unexpected character: " + c.ToString(),
                                            i + indexErrorOffset);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (c == ')')
                        {
                            Stack s3 = Stack.Allocate();
                            Stack s4 = Stack.Allocate();
                            try
                            {
                                while (true)
                                {
                                    if (s2.IsEmpty())
                                    {
                                        throw new ExpressionParseException("Missing open bracket.",
                                            i + indexErrorOffset);
                                    }

                                    object o = s2.Pop();
                                    if (o.Equals("("))
                                    {
                                        break;
                                    }
							s3.AddToTail(s1.Pop());
							s4.AddToTail(o);
                                }
						s3.AddToTail(s1.Pop());
                                s1.Push(Build(s3, s4));
                            }
                            finally
                            {
                                s3.ReleaseReference();
                                s4.ReleaseReference();
                            }
                        }
                        else
                        {
                            if (c == '^' || c == '*' || c == '/' || c == '+' || c == '-')
                            {
                                term = true;
                                s2.Push(c.ToString());
                            }
                            else
                            {
                                throw new ExpressionParseException(
                                    "Expected operator or close bracket but found: " + c.ToString(),
                                    i + indexErrorOffset);
                            }
                        }
                    }
                }

                if (s1.Size() != s2.Size() + 1)
                {
                    throw new ExpressionParseException("Incomplete expression.", indexErrorOffset + s.Length);
                }

                return Build(s1, s2);
            }
            finally
            {
                s1.ReleaseReference();
                s2.ReleaseReference();
            }
        }

        private static Expression Build(Stack s1, Stack s2)
        {
            Stack s3 = Stack.Allocate();
            // contains expression nodes
            Stack s4 = Stack.Allocate();
            try
            {
                while (!s2.IsEmpty())
                {
				object o = s2.RemoveTail();
				object o1 = s1.RemoveTail();
				object o2 = s1.RemoveTail();
                    if (o.Equals("^"))
                    {
					s1.AddToTail(new PowNode((Expression)o1, (Expression)o2));
                    }
                    else
                    {
					s1.AddToTail(o2);
                        s4.Push(o);
                        s3.Push(o1);
                    }
                }

                s3.Push(s1.Pop());
                while (!s4.IsEmpty())
                {
				object o = s4.RemoveTail();
				object o1 = s3.RemoveTail();
				object o2 = s3.RemoveTail();
                    if (o.Equals("*"))
                    {
					s3.AddToTail(new MultNode((Expression)o1, (Expression)o2));
                    }
                    else
                    {
                        if (o.Equals("/"))
                        {
						s3.AddToTail(new DivNode((Expression)o1, (Expression)o2));
                        }
                        else
                        {
						s3.AddToTail(o2);
                            s2.Push(o);
                            s1.Push(o1);
                        }
                    }
                }

                s1.Push(s3.Pop());
                while (!s2.IsEmpty())
                {
				object o = s2.RemoveTail();
				object o1 = s1.RemoveTail();
				object o2 = s1.RemoveTail();
                    if (o.Equals("+"))
                    {
					s1.AddToTail(new AddNode((Expression)o1, (Expression)o2));
                    }
                    else
                    {
                        if (o.Equals("-"))
                        {
						s1.AddToTail(new SubNode((Expression)o1, (Expression)o2));
                        }
                        else
                        {
                            // should never happen
                            throw new ExpressionParseException("Unknown operator: " + o, -1);
                        }
                    }
                }

                return (Expression) s1.Pop();
            }
            finally
            {
                s3.ReleaseReference();
                s4.ReleaseReference();
            }
        }
    }
}