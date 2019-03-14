/*
 * LL(1) Grammar for the Simple Expression Language
 * 
 *      Prog  ::= Exp "EOF"
 *      Exp   ::= Term ("+" Term)*
 *      Term  ::= Pow ("*" Pow)*
 *      Pow   ::= Fact ("^" Pow)?
 *      Fact  ::= "int" | "(" Exp ")"
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public enum TokenCategory {
    INT, PLUS, TIMES, POW, PAR_OPEN, PAR_CLOSED, EOF, BAD_TOKEN
}

public class Token {
    public TokenCategory Category { get; }
    public String Lexeme { get; }
    public Token(TokenCategory category, String lexeme) {
        Category = category;
        Lexeme = lexeme;
    }
    public override string ToString() {
        return $"[{Category}, \"{Lexeme}\"]";
    }
}

public class Scanner {
    readonly String input;
    static readonly Regex regex = new Regex(
        @"(\d+)|([+])|([*])|([(])|([)])|(\s)|(\^)|(.)");
    public Scanner(String input) {
        this.input = input;
    }
    public IEnumerable<Token> Start() {
        foreach (Match m in regex.Matches(input)) {
            if (m.Groups[1].Success) {
                yield return new Token(TokenCategory.INT, m.Value);
            } else if (m.Groups[2].Success) {
                yield return new Token(TokenCategory.PLUS, m.Value);
            } else if (m.Groups[3].Success) {
                yield return new Token(TokenCategory.TIMES, m.Value);
            } else if (m.Groups[4].Success) {
                yield return new Token(TokenCategory.PAR_OPEN, m.Value);
            } else if (m.Groups[5].Success) {
                yield return new Token(TokenCategory.PAR_CLOSED, m.Value);
            } else if (m.Groups[6].Success) {
                continue; // Skip spaces
            } else if (m.Groups[7].Success) {
                yield return new Token(TokenCategory.POW, m.Value);
            } else if (m.Groups[8].Success) {
                yield return new Token(TokenCategory.BAD_TOKEN, m.Value);
            }
        }
        yield return new Token(TokenCategory.EOF, null);
    }
}

public class SyntaxError: Exception { }

public class Parser {
    IEnumerator<Token> tokenStream;
    public Parser(IEnumerator<Token> tokenStream) {
        this.tokenStream = tokenStream;
        this.tokenStream.MoveNext();
    }
    public TokenCategory Current {
        get { return tokenStream.Current.Category; }
    }
    public Token Expect(TokenCategory category) {
        if (Current == category) {
            Token current = tokenStream.Current;
            tokenStream.MoveNext();
            return current;
        } else {
            throw new SyntaxError();
        }
    }
    public Node Prog() {
        var n = new Prog() {
            Exp()  // Add method called automatically
        };
        Expect(TokenCategory.EOF);
        return n;
    }
    public Node Exp() {
        var n1 = Term();
        while (Current == TokenCategory.PLUS) {
            var n2 = new Plus() {
                AnchorToken = Expect(TokenCategory.PLUS)
            };
            n2.Add(n1);
            n2.Add(Term());
            n1 = n2;
        }
        return n1;
    }
    public Node Term() {
        var n1 = Pow();
        while (Current == TokenCategory.TIMES) {
            var n2 = new Times() {
                AnchorToken = Expect(TokenCategory.TIMES)
            };
            n2.Add(n1);
            n2.Add(Pow());
            n1 = n2;
        }
        return n1;
    }
    public Node Pow() {
        var n1 = Fact();
        if (Current == TokenCategory.POW) {
            var n2 = new Pow() {
                AnchorToken = Expect(TokenCategory.POW)
            };
            n2.Add(n1);
            n2.Add(Pow());
            n1 = n2;
        }
        return n1;
    }
    public Node Fact() {
        switch (Current) {

        case TokenCategory.INT:
            return new Int() {
                AnchorToken = Expect(TokenCategory.INT)
            };

        case TokenCategory.PAR_OPEN:
            Expect(TokenCategory.PAR_OPEN);
            var n = Exp();
            Expect(TokenCategory.PAR_CLOSED);
            return n;

        default:
            throw new SyntaxError();
        }
    }
}

public class Node: IEnumerable<Node> {

    IList<Node> children = new List<Node>();

    public Node this[int index] {
        get {
            return children[index];
        }
    }

    public Token AnchorToken { get; set; }

    public void Add(Node node) {
        children.Add(node);
    }

    public IEnumerator<Node> GetEnumerator() {
        return children.GetEnumerator();
    }

    System.Collections.IEnumerator
    System.Collections.IEnumerable.GetEnumerator() {
        throw new NotImplementedException();
    }

    public override string ToString() {
        return String.Format("{0} {1}", GetType().Name, AnchorToken);
    }

    public string ToStringTree() {
        var sb = new StringBuilder();
        TreeTraversal(this, "", sb);
        return sb.ToString();
    }

    static void TreeTraversal(Node node, string indent, StringBuilder sb) {
        sb.Append(indent);
        sb.Append(node);
        sb.Append('\n');
        foreach (var child in node.children) {
            TreeTraversal(child, indent + "  ", sb);
        }
    }
}

public class Prog  : Node { }
public class Plus  : Node { }
public class Times : Node { }
public class Pow   : Node { }
public class Int   : Node { }

public class Driver {
    public static void Main() {
        Console.Write("> ");
        var line = Console.ReadLine();
        var parser = new Parser(new Scanner(line).Start().GetEnumerator());
        try {
            var ast = parser.Prog();
            Console.WriteLine(ast.ToStringTree());
        } catch (SyntaxError) {
            Console.WriteLine("Bad syntax!");
        }
    }
}
