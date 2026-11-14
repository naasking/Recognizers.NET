# Recognizers.NET

This is a simple combinator library for recognizing strings using only imperative constructs. It
features:

 1. no heap allocation
 2. no higher-order functions
 3. no virtual dispatching
 4. at least as powerful as regular expressions
 5. efficient variable capture

The one exception to the first three is left recursion, which needs one delegate per
left-recursive rule: see [Left recursion](#left-recursion).

This makes the library incredibly efficient. The API is also suitable
for use in low-level imperative languages, like C, which either lack the above features,
or are intended to be used without them.

# Atoms

Some simple "atom" recognizers provided by the library, ie. recognizers for basic types:

    // recognize one digit
    public static bool Digit(ref this Input x, ref Position pos, string capture = null) =>
        pos.Pos < x.Length && char.IsNumber(x[pos]) && pos.Advance();

    // recognize many digits
    public static bool Digits(ref this Input x, ref Position pos)
    {
        var i = pos;
        while (i.Pos < x.Length && char.IsNumber(x[i]))
            ++i.Pos;
        return pos.AdvanceTo(i);
    }

    // recognize many whitespace characters
    public static bool WhiteSpaces(ref this Input x, ref Position pos)
    {
        var i = pos;
        while (i.Pos < x.Length && char.IsWhiteSpace(x[i]))
            ++i.Pos;
        return pos.AdvanceTo(i);
    }

# Combinators

You can combine these simple recognizers using the ordinary boolean operators, && and ||,
where && will succeed if both recognizers match, and || will succeed if either recognizer
matches:

    // recognize strings like "(1234)"
    public static bool BracketedDigits(this ref Input x, ref Position pos) =>
        pos.Save(out var i) && x.Char('(', ref i) && x.Digits(ref i) && x.Char(')', ref i) && pos.AdvanceTo(i);

    // matches one character in the range [A-z] or [0-9]
    public static bool LetterOrDigit(ref this Input x, ref Position pos) =>
        x.Letter(ref pos) || x.Digit(ref pos);

Save/AdvanceTo perform the backtracking that's needed in case the rule fails.
Recognizer combinations can get quite sophisticated. Here's one for phone numbers:

    public static bool PhoneNumber(this ref Input x, ref Position pos) =>
           pos.Save(out var i)
        && x.Optional(x.WhiteSpaces(ref i))
        && x.Optional(x.Char('+', ref i))
        && x.Optional(x.WhiteSpaces(ref i))
        && x.Optional(x.Digits(ref i))
        && x.Optional(x.WhiteSpaces(ref i))
        && x.Optional(x.BracketedDigits(ref i))
        && x.Optional(x.WhiteSpaces(ref i))
        && x.Optional(x.Char('/', ref i))
        && x.Optional(x.WhiteSpaces(ref i))
        && x.DelimitedDigits(ref i, ' ', '-')
        && pos.AdvanceTo(i);

You can see that this is essentially a specification of what a phone number should
look like, and it's executable to recognize and extract phone numbers!

By convention, methods in plural form are greedy and will consume as many matching
characters as possible, where singular form matches only a single instance.

I'm not sure if this API has ever been done before, but it's kind of neat that you can
almost get to parser combinators without any dynamic allocation or higher-order
functions. It only requires a little discipline and repetition in some cases,
particularly around recursive combinators.

Finally, this API isn't just suitable for strings, but can be written to operate
on bits, bytes or tokens. This could make for some easy to read, but fast
binary pattern matching.

# Left recursion

A left-recursive rule like `e ::= e '-' n | n` can't be written directly as a
recognizer, because it would call itself at the same position forever. A single
left-recursive rule can be rewritten as a loop, `e ::= n ('-' n)*`, folding the
results to the left, which is how the lambda calculus example in
[Recognizers.Tests/LC.cs](Recognizers.Tests/LC.cs) parses application. But then every
left-recursive rule needs its own loop, and each loop has to get the same details
right by hand.

`Grow` solves left recursion once, for every rule. The rule is written as it reads in
the grammar, and makes its left-recursive call through the `LeftRecursion<T>` it's
given:

    // e ::= e '-' n | n, where Number recognizes n
    static readonly LeftRecursiveRule<int> Minus =
        (Input x, ref Position pos, LeftRecursion<int> e, out int value) =>
        {
            var i = pos;
            if (e.Recurse(x, ref i, out var left) && x.Char('-', ref i) && Number(x, ref i, out var right))
            {
                value = left - right;
                return pos.Seek(i);
            }
            i = pos;
            return !e.Seeded && Number(x, ref i, out value) && pos.Seek(i)
                || Fail(out value);
        };

    public static bool Expression(this Input x, ref Position pos, out int value) =>
        x.Grow(ref pos, Minus, out value);

`Grow` runs the rule repeatedly at the same position. On the first pass `Recurse`
fails, so only `Number` can match. On each later pass `Recurse` answers with the
previous pass's result, so the rule can extend it. Growth stops when a pass fails,
and the longest parse wins, so `8-4-2` is `(8-4)-2`.

Once `Seeded` is true, the rule must skip its alternatives that don't start with
`Recurse`. They can only repeat the first pass, and running them again would double
the work at every level of nesting. A pass that matches without growing means that
check is missing, so `Grow` throws `InvalidOperationException` on the first input the
rule matches, instead of silently making parsing exponential.

`Grow` takes the rule as a delegate, because it has to run the rule again with each
pass's result. That delegate is the only allocation, the only virtual dispatch and the
only higher-order function that the library needs to solve left recursion reliably:

 * Store the delegate in a static field, or write it as a lambda that captures
   nothing, and it's allocated once for the life of the program. `Grow` itself
   allocates nothing while parsing.
 * Invoking the delegate is the only indirect call, once per pass.
 * Passing a static method directly, as in `x.Grow(ref pos, MyRule, out v)`,
   allocates a new delegate on every call, unless the caller compiles with C# 11 or
   later, which caches it.

## Direct and indirect left recursion

`Grow` handles direct left recursion, where a rule's left-recursive call is to the rule
itself. A rule can have several left-recursive alternatives, as long as each one starts
with `Recurse`, and the others are skipped once `Seeded` is true.

Left recursion is indirect when a rule reaches itself through other rules, before
consuming any input:

    a ::= b 'x' | 'z'
    b ::= a 'y' | 'w'

Here `a` calls `b`, which calls `a` again at the same position. `Grow` can't see that
cycle: the inner call to `a` starts a new growth, which calls `b` again, and so on until
the stack overflows. In .NET a stack overflow ends the process, and it can't be caught.
Recursion that consumes input first, like `f ::= '(' e ')'`, isn't left recursion and
needs nothing special.

Indirect left recursion can always be rewritten as direct left recursion, or removed:

 1. **Substitute the cycle into the rule you call.** Where an alternative starts with
    another rule of the cycle, replace that rule with its alternatives, until the rule
    refers to itself. Replacing `b` in `a` gives `(a 'y' | 'w') 'x' | 'z'`, and
    distributing the `'x'` gives

        a ::= a 'y' 'x' | 'w' 'x' | 'z'

    which `Grow` handles, and which no longer calls `b`. `b ::= a 'y' | 'w'` can stay as
    it is: it calls `a`, but `a` no longer calls `b`, so there's no cycle.

    Grammars for calls and member access often come out indirectly left recursive:
    `e ::= call | member | name`, with `call ::= e '(' ')'` and
    `member ::= e '.' name`. Substituting `call` and `member` into `e` gives

        e ::= e '(' ')' | e '.' name | name

    which has two left-recursive alternatives, and parses `f().x()` as
    `call(member(call(f), x))`.

 2. **Substitute into each rule you call.** A grown rule is greedy, so the result
    depends on which rule of the cycle you call. With `b` left as in step 1, `b` fails
    on `zyx`, even though `zy` is a valid `b`: `a` takes all of `zyx`, and leaves no `y`
    for `b`. If `b` is called on its own too, substitute `a` into it as well:

        b ::= b 'x' 'y' | 'z' 'y' | 'w'

    This matches `zy` at the start of `zyx`. The two rules then don't refer to each
    other at all.

 3. **Remove the left recursion.** A directly left-recursive rule `a ::= a α | β`
    describes the same language as `a ::= β α*`, which a loop parses without `Grow`,
    the way [LC.cs](Recognizers.Tests/LC.cs) parses application. The rule from step 1
    becomes

        a ::= ('w' 'x' | 'z') ('y' 'x')*

    and folding each `'y' 'x'` onto the result so far, in the loop, builds the same tree
    as the left-recursive rule. Substituting and then removing direct left recursion,
    for every rule, is the textbook algorithm for removing left recursion from a
    grammar. Use a loop when it's simpler than the rule, and `Grow` when the rule is
    clearer as written.
