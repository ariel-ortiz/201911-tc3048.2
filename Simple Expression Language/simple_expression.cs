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
    public int Prog() {
        var result = Exp();
        Expect(TokenCategory.EOF);
        return result;
    }
    public int Exp() {
        var result = Term();
        while (Current == TokenCategory.PLUS) {
            Expect(TokenCategory.PLUS);
            result += Term();
        }
        return result;
    }
    public int Term() {
        var result = Pow();
        while (Current == TokenCategory.TIMES) {
            Expect(TokenCategory.TIMES);
            result *= Pow();
        }
        return result;
    }
    public int Pow() {
        var result = Fact();
        if (Current == TokenCategory.POW) {
            Expect(TokenCategory.POW);
            result = (int) Math.Pow(result, Pow());
        }
        return result;
    }
    public int Fact() {
        switch (Current) {
        case TokenCategory.INT:
            var token = Expect(TokenCategory.INT);
            return Convert.ToInt32(token.Lexeme);
        case TokenCategory.PAR_OPEN:
            Expect(TokenCategory.PAR_OPEN);
            var result = Exp();
            Expect(TokenCategory.PAR_CLOSED);
            return result;
        default:
            throw new SyntaxError();
        }
    }
}

public class Driver {
    public static void Main() {
        Console.Write("> ");
        var line = Console.ReadLine();
        var parser = new Parser(new Scanner(line).Start().GetEnumerator());
        try {
            var result = parser.Prog();
            Console.WriteLine(result);
        } catch (SyntaxError) {
            Console.WriteLine("Bad syntax!");
        }
    }
}
