# Recognizers.NET

This is a simple combinator library for recognizing strings using only imperative constructs,
no allocation, and higher-order functions or other fancy features. This makes the API suitable
for use in imperative languages, like C.

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

    // recognize whitespace
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
        (Letter(ref x, ref pos) || Digit(ref x, ref pos)) && pos.Advance();

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

By convention, methods in plural form are greedy and will consume as many matching
characters as possible, where singular form matches only a single instance.

I'm not sure if this API has ever been done before, but it's kind of neat that you can
almost get to parser combinators without any dynamic allocation or higher-order
functions. It only requires a little discipline and repetition in some cases,
particularly around recursive combinators.

Finally, this API isn't just suitable for strings, but can be written to operate
on bits, bytes or tokens. This could make for some easy to read, but fast
binary pattern matching.
