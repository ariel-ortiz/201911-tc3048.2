//---------------------------------------------------------
// Type your name and student ID here.
//---------------------------------------------------------

%{

#include <stdio.h>
#include <stdarg.h>

void yyerror(char *s, ...);
extern int yylineno;
int yylex(void);

%}

/* declare tokens */
%token INTEGER ADD MUL PAR_LEFT PAR_RIGHT EOL

%%

calclist:
    /* nothing */ { }                            /* matches at beginning of input */
    | calclist exp EOL { printf("%d\n> ", $2); } /* EOL is end of an expression */
;

exp:
    INTEGER /* default $$ = $1 */
    | PAR_LEFT ADD addlist PAR_RIGHT { $$ = $3; }
    | PAR_LEFT MUL mullist PAR_RIGHT { $$ = $3; }
;

addlist:
    /* empty */   { $$ = 0; }
    | addlist exp { $$ = $1 + $2; }
;

mullist:
    /* empty */   { $$ = 1; }
    | mullist exp { $$ = $1 * $2; }
;

%%

int main(int argc, char **argv) {
    printf("> ");
    yyparse();
    return 0;
}

void yyerror(char *s, ...) {
    va_list ap;
    va_start(ap, s);
    fprintf(stderr, "Line %d: ", yylineno);
    vfprintf(stderr, s, ap);
    fprintf(stderr, "\n");
}
